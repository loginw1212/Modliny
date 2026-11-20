-- POJAZDY OBSLUGI NAZIEMNEJ: sterowanie + automatyczne trasy
-- Wejdz na siedzenie = sterujesz (W/S gaz, A/D kierownica). Zejscie = powrot na miejsce postoju.
-- Trasa patrolowa: atrybut "Route" modelu, format "x1,z1;x2,z2;..."

local model = script.Parent
local chassis = model:WaitForChild("Chassis")
local seat = model:WaitForChild("Kierowca")

local RunService = game:GetService("RunService")
local Players = game:GetService("Players")

local parkedCFrame = chassis.CFrame
local WELDED = false
local driving = false
local routeIdx = 1
local yaw = select(2, chassis.CFrame:ToOrientation())

local function weldAll()
	if WELDED then return end
	WELDED = true
	for _, p in ipairs(model:GetDescendants()) do
		if p:IsA("BasePart") and p ~= chassis then
			local weld = Instance.new("WeldConstraint")
			weld.Part0 = chassis
			weld.Part1 = p
			weld.Parent = chassis
		end
	end
end
weldAll()

local bodyVel = Instance.new("BodyVelocity")
bodyVel.MaxForce = Vector3.new(1e6, 1e6, 1e6)
bodyVel.Velocity = Vector3.zero
bodyVel.Parent = chassis

local gyro = Instance.new("BodyGyro")
gyro.MaxTorque = Vector3.new(3e6, 3e6, 3e6)
gyro.P = 4000
gyro.D = 400
gyro.CFrame = chassis.CFrame
gyro.Parent = chassis

local function parseRoute(attr)
	local pts = {}
	if type(attr) == "string" and attr ~= "" then
		for token in string.gmatch(attr, "[^;]+") do
			local x, z = string.match(token, "(-?%d+%.?%d*)%s*,%s*(-?%d+%.?%d*)")
			if x and z then
				table.insert(pts, Vector3.new(tonumber(x), 0, tonumber(z)))
			end
		end
	end
	return pts
end

local route = parseRoute(model:GetAttribute("Route"))
if #route == 0 then
	local DEFAULTS = {
		FollowMe = "-330,150;300,150;-330,150",
		Bagazowka = "-204,241;60,243;-204,241",
	}
	route = parseRoute(DEFAULTS[model.Name] or "")
end
local speed = 0

seat:GetPropertyChangedSignal("Occupant"):Connect(function()
	if seat.Occupant then
		driving = true
		weldAll()
		chassis.Anchored = false
		gyro.CFrame = chassis.CFrame
		yaw = select(2, chassis.CFrame:ToOrientation())
		speed = 0
	else
		driving = false
		bodyVel.Velocity = Vector3.zero
		task.wait(2)
		if not driving then
			chassis.Anchored = true
			model:PivotTo(parkedCFrame)
			yaw = select(2, chassis.CFrame:ToOrientation())
		end
	end
end)

RunService.Heartbeat:Connect(function(dt)
	if model:GetAttribute("Servicing") then return end

	if driving then
		local throttle = seat.Throttle
		local steer = seat.Steer
		local maxSpd = tonumber(model:GetAttribute("MaxSpeed")) or 26
		local target = 0
		if throttle > 0 then
			target = throttle * maxSpd
		elseif throttle < 0 then
			target = math.max(speed + throttle * 60 * dt, -maxSpd * 0.4)
		end
		speed = speed + (target - speed) * math.clamp(dt * 4, 0, 1)
		if math.abs(speed) > 0.5 then
			yaw -= steer * dt * 1.5 * math.clamp(math.abs(speed) / 10, 0.4, 1) * (speed >= 0 and 1 or -1)
		end
		gyro.CFrame = CFrame.Angles(0, yaw, 0)
		local forward = -gyro.CFrame.LookVector
		bodyVel.Velocity = forward * speed
	elseif chassis.Anchored and #route > 0 then
		local target = route[routeIdx]
		local pos = chassis.Position
		local flat = Vector3.new(target.X - pos.X, 0, target.Z - pos.Z)
		if flat.Magnitude < 3 then
			routeIdx = routeIdx % #route + 1
			return
		end
		local dir = flat.Unit
		local targetYaw = math.atan2(-dir.X, -dir.Z)
		local step = math.min(tonumber(model:GetAttribute("PatrolSpeed")) or 12, flat.Magnitude) * dt
		yaw = targetYaw
		model:PivotTo(CFrame.new(pos + dir * step, pos + dir * step + dir))
	end
end)

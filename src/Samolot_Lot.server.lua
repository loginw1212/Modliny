-- SAMOLOT: Boeing 737-800 (Lotnisko Modlin / WMI)
-- Sterowanie: W/S = przepustnica, A/D = ster kierunkowy
-- Komendy na czacie: /gear (podwozie), /flaps (klapy), /brakes (hamowanie), /reset (poziomowanie)

local model = script.Parent
local root = model:WaitForChild("Main")
local seat = model:WaitForChild("PilotSeat")

local RunService = game:GetService("RunService")
local Players = game:GetService("Players")

local headYaw = 0
local roll = 0
local GROUND_Y = 10
local gearDown = true
local flaps = false
local flying = false
local speed = 0
local WELDED = false
local pilot = nil

local function weldAll()
	if WELDED then return end
	WELDED = true
	for _, p in ipairs(model:GetDescendants()) do
		if p:IsA("BasePart") and p ~= root then
			local weld = Instance.new("WeldConstraint")
			weld.Part0 = root
			weld.Part1 = p
			weld.Parent = root
		end
	end
end
weldAll()
headYaw = select(2, root.CFrame:ToOrientation())

local bodyVel = Instance.new("BodyVelocity")
bodyVel.MaxForce = Vector3.new(1e6, 1e6, 1e6)
bodyVel.Velocity = Vector3.zero
bodyVel.Parent = root

local gyro = Instance.new("BodyGyro")
gyro.MaxTorque = Vector3.new(4e6, 4e6, 4e6)
gyro.P = 6000
gyro.D = 500
gyro.CFrame = root.CFrame
gyro.Parent = root

local function setGear(down)
	gearDown = down
	local gear = model:FindFirstChild("Podwozie")
	if gear then
		for _, g in ipairs(gear:GetChildren()) do
			if g:IsA("BasePart") then g.Transparency = down and 0.25 or 1 end
		end
	end
end

local function setFlaps(on)
	flaps = on
	local f = model:FindFirstChild("Klapy")
	if f then
		for _, p in ipairs(f:GetChildren()) do
			if p:IsA("BasePart") then p.Transparency = on and 0.1 or 1 end
		end
	end
end

Players.PlayerAdded:Connect(function(plr)
	plr.Chatted:Connect(function(msg)
		if plr ~= pilot then return end
		local cmd = string.lower(msg):gsub("%s+", "")
		if cmd == "/gear" then
			setGear(not gearDown)
		elseif cmd == "/flaps" then
			setFlaps(not flaps)
		elseif cmd == "/brakes" then
			speed = 0
		elseif cmd == "/reset" and not flying then
			root.CFrame = CFrame.new(root.Position) * CFrame.Angles(0, headYaw, 0)
		end
	end)
end)

seat:GetPropertyChangedSignal("Occupant"):Connect(function()
	if seat.Occupant then
		pilot = Players:GetPlayerFromCharacter(seat.Occupant and seat.Occupant.Parent or nil)
		flying = true
		weldAll()
		root.Anchored = false
		bodyVel.Velocity = Vector3.zero
		gyro.CFrame = root.CFrame
	else
		pilot = nil
		flying = false
		bodyVel.Velocity = Vector3.zero
		task.wait(1.5)
		if not flying then
			root.CFrame = CFrame.new(root.Position) * CFrame.Angles(0, headYaw, 0)
			root.Anchored = true
		end
	end
end)

RunService.Heartbeat:Connect(function(dt)
	if not flying then return end
	local throttle = seat.Throttle
	local steer = seat.Steer

	local maxSpd = 235
	if flaps then maxSpd = maxSpd * 0.8 end

	local target = 0
	if throttle > 0 then
		target = 45 + throttle * (maxSpd - 45)
	elseif throttle < 0 then
		target = math.max(speed + throttle * 250 * dt, 0)
	end
	local accel = (target > speed) and 0.35 or 1.2
	speed = speed + (target - speed) * math.clamp(dt * accel, 0, 1)
	if math.abs(speed) < 0.4 and target == 0 then speed = 0 end

	if speed > 4 then
		headYaw -= steer * dt * (1.2 - math.clamp(speed / 420, 0, 0.8))
	end
	local localRoll = -steer * math.clamp(speed / 150, 0, 1) * 24
	roll = roll + (localRoll - roll) * math.clamp(dt * 3, 0, 1)

	local vy = 0
	local pos = root.Position
	if pos.Y <= GROUND_Y + 1 then
		if speed > 82 then
			vy = math.clamp((speed - 82) / 90, 0, 1) * 55 + (flaps and 12 or 0)
		else
			vy = 0
			speed = speed * 0.995
		end
		if throttle < 0 then speed = math.max(speed - 140 * dt, 0) end
	else
		if speed > 82 then
			vy = math.clamp((speed - 82) / 90, 0, 1) * 45 + (flaps and 10 or 0)
		elseif speed < 58 then
			vy = flaps and -13 or -19
		end
	end
	if pos.Y <= GROUND_Y and vy <= 0 then
		vy = 0
		root.CFrame = CFrame.new(Vector3.new(pos.X, GROUND_Y, pos.Z)) * CFrame.Angles(0, headYaw, 0)
	end

	local pitch = math.clamp(vy, 0, 45) * 0.22
	gyro.CFrame = CFrame.Angles(0, headYaw, 0) * CFrame.Angles(math.rad(-pitch), 0, math.rad(roll))
	local forward = -gyro.CFrame.LookVector
	bodyVel.Velocity = forward * speed + Vector3.new(0, vy, 0)
end)

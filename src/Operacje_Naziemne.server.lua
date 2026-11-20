-- OBSLUGA NAZIEMNA (RAMP) - automatyczna petla na bramce A1:
-- schody -> autobus -> bagazowka -> tankowoz

local lotnisko = workspace:WaitForChild("Lotnisko_Modlin_WMI")
local pojazdy = lotnisko:WaitForChild("Pojazdy")
local samolot = lotnisko:WaitForChild("Boeing737_WMI_100")

local GATE = Vector3.new(-150, 0, 260)

local function busy(v)
	return v:GetAttribute("PlayerDriving") ~= nil
end

local function moveTo(vehicle, target, s)
	local pos = vehicle:GetPivot().Position
	local flat = Vector3.new(target.X - pos.X, 0, target.Z - pos.Z)
	local dist = flat.Magnitude
	if dist < 2 then return end
	local dir = flat.Unit
	local t = 0
	while t < dist / s do
		t += task.wait()
		if busy(vehicle) then return false end
		local p = pos + dir * s * math.min(t, dist / s)
		vehicle:PivotTo(CFrame.new(p.X, pos.Y, p.Z))
	end
	vehicle:PivotTo(CFrame.new(target.X, pos.Y, target.Z))
	return true
end

local function serve(vehicle, targetPos, waitTime, s)
	if busy(vehicle) then return end
	vehicle:SetAttribute("Servicing", true)
	local homeCf = vehicle:GetPivot()
	local home = Vector3.new(homeCf.X, 0, homeCf.Z)
	if moveTo(vehicle, targetPos, s or 16) then
		task.wait(waitTime)
		if not busy(vehicle) then moveTo(vehicle, home, s or 16) end
	end
	vehicle:SetAttribute("Servicing", nil)
end

local function offset(x, z)
	return GATE + Vector3.new(x, 0, z)
end

while true do
	task.wait(25)

	local planeMain = samolot and samolot:FindFirstChild("Main")
	if planeMain and (planeMain.Position - GATE).Magnitude < 80 then
		serve(pojazdy:FindFirstChild("Schody"), offset(14, 32), 12, 14)
		serve(pojazdy:FindFirstChild("Bus"), offset(-16, 40), 10)
		serve(pojazdy:FindFirstChild("Bagazowka"), offset(-20, -12), 8)
		serve(pojazdy:FindFirstChild("Tankowoz"), offset(34, -10), 8, 12)
		serve(pojazdy:FindFirstChild("BeltLoader"), offset(-26, -6), 8, 12)
		serve(pojazdy:FindFirstChild("Catering"), offset(-2, -26), 8, 12)
	end
end

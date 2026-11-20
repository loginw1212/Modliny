-- RUCH PASAZEROW: chodzacy pasazerowie na plycie (autobus <-> schody <-> terminal)

local lotnisko = workspace:WaitForChild("Lotnisko_Modlin_WMI")
local grupa = lotnisko:WaitForChild("Pasazerowie")

local Y = 4.2
local trasy = {
	{Vector3.new(-140, Y, 292), Vector3.new(-173, Y, 285), Vector3.new(-140, Y, 292)},
	{Vector3.new(-150, Y, 296), Vector3.new(-173, Y, 300), Vector3.new(-150, Y, 296)},
	{Vector3.new(-190, Y, 332), Vector3.new(-140, Y, 292), Vector3.new(-190, Y, 332)},
	{Vector3.new(-190, Y, 332), Vector3.new(-150, Y, 296), Vector3.new(-190, Y, 332)},
}

local i = 0
for _, p in ipairs(grupa:GetChildren()) do
	i += 1
	local trasa = trasy[i] or trasy[1]
	local idx = 1
	local speed = 5 + i
	task.spawn(function()
		while true do
			local pos = p:GetPivot().Position
			local cel = trasa[idx]
			local flat = Vector3.new(cel.X - pos.X, 0, cel.Z - pos.Z)
			if flat.Magnitude < 2 then
				idx = idx % #trasa + 1
				task.wait(math.random() * 2)
			else
				local dir = flat.Unit
				local step = math.min(speed * 0.05, flat.Magnitude)
				task.wait(0.05)
				p:PivotTo(CFrame.lookAt(pos + dir * step, pos + dir * step + dir))
			end
		end
	end)
end

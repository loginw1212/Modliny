-- OSWIETLENIE LOTNISKA: lampy podejsciowe (rabbit), latarnia PAPI, beacon wiezy, krawedzie pasa

local lotnisko = workspace:WaitForChild("Lotnisko_Modlin_WMI")
local oswietlenie = lotnisko:WaitForChild("Oswietlenie")

local strobes = {}
for i = 1, 8 do
	local s = oswietlenie:FindFirstChild("Podejscie" .. i)
	if s then table.insert(strobes, s) end
end
local papi = {}
for i = 1, 4 do
	local p = oswietlenie:FindFirstChild("PAPI" .. i)
	if p then table.insert(papi, p) end
end
local beacon = oswietlenie:FindFirstChild("Beacon")
local krawedzie = {}
for _, e in ipairs(oswietlenie:GetChildren()) do
	if e.Name:sub(1, 4) == "Kraw" then table.insert(krawedzie, e) end
end

task.spawn(function() -- sekwencja podejsciowa (rabbit)
	while true do
		for _, s in ipairs(strobes) do
			s.Material = Enum.Material.Neon
			s.Color = Color3.fromRGB(255, 255, 255)
			task.wait(0.09)
			s.Material = Enum.Material.SmoothPlastic
			s.Color = Color3.fromRGB(120, 120, 120)
		end
		task.wait(0.9)
	end
end)

task.spawn(function() -- PAPI: 2 biale / 2 czerwone
	while true do
		for _, p in ipairs(papi) do
			local num = tonumber(p.Name:sub(-1)) or 0
			p.Color = num <= 2 and Color3.fromRGB(255, 255, 255) or Color3.fromRGB(255, 40, 40)
		end
		task.wait(2)
		for _, p in ipairs(papi) do
			p.Color = Color3.fromRGB(255, 255, 255)
		end
		task.wait(2)
	end
end)

task.spawn(function() -- wieza: naprzemiennie zielony/bialy
	local white = true
	while true do
		if beacon then
			beacon.Color = white and Color3.fromRGB(255, 255, 255) or Color3.fromRGB(40, 255, 90)
		end
		white = not white
		task.wait(0.5)
	end
end)

task.spawn(function() -- krawedzie pasa: fala swiatla
	local lit = true
	while true do
		for i, e in ipairs(krawedzie) do
			if lit or i % 2 == 1 then
				e.Material = Enum.Material.Neon
				e.Color = Color3.fromRGB(255, 230, 160)
			else
				e.Material = Enum.Material.SmoothPlastic
				e.Color = Color3.fromRGB(110, 110, 110)
			end
		end
		lit = not lit
		task.wait(0.45)
	end
end)

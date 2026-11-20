-- SYSTEM BRAMEK (GATE SYSTEM) Lotnisko Modlin
-- - czujniki styania A1..A4: lapia samolot, przelaczaja tablice ZAJETA/WOLNA
-- - automatyczne drzwi terminala (ProximityPrompt)
-- - tablica odlotow

local lotnisko = workspace:WaitForChild("Lotnisko_Modlin_WMI")
local TerminalService = game:GetService("TweenService")

local stands = {}
for _, gate in ipairs(lotnisko:WaitForChild("Bramki"):GetChildren()) do
	local sensor = gate:FindFirstChild("Czujnik")
	local sign = gate:FindFirstChild("Tabliczka", true)
	if sensor then
		stands[gate.Name] = {sensor = sensor, sign = sign, busy = false}
	end
end

local function setText(label, text, color)
	if label and label:IsA("TextLabel") then
		label.Text = text
		label.TextColor3 = color
	end
end

task.spawn(function()
	while true do
		for name, st in pairs(stands) do
			local busy = false
			for _, m in ipairs(workspace:GetChildren()) do
				if m:IsA("Model") and m.Name:find("Boeing") then
					local pp = m.PrimaryPart or m:FindFirstChild("Main")
					if pp and (pp.Position - st.sensor.Position).Magnitude < 70 then
						busy = true
					end
				end
			end
			if busy ~= st.busy then
				st.busy = busy
				if busy then
					setText(st.sign, name .. " ZAJETA", Color3.fromRGB(255, 80, 80))
				else
					setText(st.sign, name .. " WOLNA", Color3.fromRGB(80, 255, 120))
				end
			end
		end
		task.wait(2)
	end
end)

-- drzwi terminala
local drzwi = lotnisko:FindFirstChild("Terminal")
if drzwi then
	for _, d in ipairs(drzwi:GetDescendants()) do
		if d:IsA("ProximityPrompt") then
			d.Triggered:Connect(function()
				local left = d.Parent:FindFirstChild("Lewa")
				local right = d.Parent:FindFirstChild("Prawa")
				local function slide(part, x)
					if not part then return end
					local tw = TerminalService:Create(part, TweenInfo.new(0.8, Enum.EasingStyle.Quad), {CFrame = part.CFrame * CFrame.new(x, 0, 0)})
					tw:Play()
				end
				slide(left, -6)
				slide(right, 6)
				task.delay(4, function()
					slide(left, 6)
					slide(right, -6)
				end)
			end)
		end
	end
end

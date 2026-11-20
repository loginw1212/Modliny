#!/usr/bin/env python3
"""Generuje ModlinAirport.rbxmx - model calego lotniska Modlin dla Roblox Studio."""
import math
import random
from xml.sax.saxutils import escape

OUT = "ModlinAirport.rbxmx"
SRC = "src"

_ref = [0]
def ref():
	_ref[0] += 1
	return f"RBX{_ref[0]}"

MATERIALS = {
	"plastic": 256, "smooth": 272, "neon": 288, "metal": 1080,
	"diamond": 1056, "concrete": 816, "grass": 1280, "glass": 1568,
	"wood": 512, "foil": 1072,
}
SHAPES = {"ball": 0, "block": 1, "cylinder": 2}

def prop_c3u(name, hexc):
	h = hexc.lstrip("#")
	r, g, b = int(h[0:2], 16), int(h[2:4], 16), int(h[4:6], 16)
	val = (r << 24) | (g << 16) | (b << 8) | 0xFF
	return f"<Color3uint8 name=\"{name}\">{val}</Color3uint8>"

def col(hexstr):
	h = hexstr.lstrip("#")
	r, g, b = int(h[0:2], 16), int(h[2:4], 16), int(h[4:6], 16)
	return (r << 24) | (g << 16) | (b << 8) | 0xFF

def rot_matrix(rx, ry, rz):
	rx, ry, rz = map(math.radians, (rx, ry, rz))
	cx, sx = math.cos(rx), math.sin(rx)
	cy, sy = math.cos(ry), math.sin(ry)
	cz, sz = math.cos(rz), math.sin(rz)
	# R = Rx * Ry * Rz (kolejnosc jak CFrame.Angles)
	_R = None
	# Rx
	Rx = ((1,0,0),(0,cx,-sx),(0,sx,cx))
	# Ry
	Ry = ((cy,0,sy),(0,1,0),(-sy,0,cy))
	# Rz
	Rz = ((cz,-sz,0),(sz,cz,0),(0,0,1))
	def mul(A,B):
		return tuple(tuple(sum(A[i][k]*B[k][j] for k in range(3)) for j in range(3)) for i in range(3))
	R = mul(mul(Rx,Ry),Rz)
	return R

def cf_xml(x, y, z, rx=0, ry=0, rz=0):
	R = rot_matrix(rx, ry, rz)
	rows = "".join(f"<R{i}{j}>{R[i][j]:.6f}</R{i}{j}>" for i in range(3) for j in range(3))
	return f"<CoordinateFrame name=\"CFrame\"><X>{x}</X><Y>{y}</Y><Z>{z}</Z>{rows}</CoordinateFrame>"

class Node:
	def __init__(self, cls, name, parent=None):
		self.cls = cls
		self.name = name
		self.parent = parent
		self.props = ""
		self.children = []
		self.id = ref()
		if parent is not None:
			parent.children.append(self)

	def add(self, xml):
		self.props += xml
		return self

def xml_item(node):
	props = f"<string name=\"Name\">{escape(node.name)}</string>"
	props += node.props
	kids = "".join(xml_item(c) for c in node.children)
	return f"<Item class=\"{node.cls}\" referent=\"{node.id}\"><Properties>{props}</Properties>{kids}</Item>"

def write(root_nodes):
	body = "".join(xml_item(n) for n in root_nodes)
	with open(OUT, "w", encoding="utf-8") as f:
		f.write('<roblox xmlns:xmime="http://www.w3.org/2005/05/xmlmime" version="4">' + body + "</roblox>")

def prop_bool(name, v): return f"<bool name=\"{name}\">{str(v).lower()}</bool>"
def prop_num(name, v): return f"<float name=\"{name}\">{v}</float>"
def prop_token(name, v): return f"<token name=\"{name}\">{v}</token>"
def prop_str(name, v): return f"<string name=\"{name}\">{escape(str(v))}</string>"
def prop_v3(name, x, y, z): return f"<Vector3 name=\"{name}\"><X>{x}</X><Y>{y}</Y><Z>{z}</Z></Vector3>"
def prop_c3(name, hexc):
	r, g, b = [int(hexc.lstrip('#')[i:i+2], 16) for i in (0, 2, 4)]
	return f"<Color3 name=\"{name}\"><R>{r/255}</R><G>{g/255}</G><B>{b/255}</B></Color3>"

def part(parent, name, size, pos, color="#c8c8c8", material="plastic", shape="block",
		 rot=(0, 0, 0), transparency=0, collide=True, anchored=True, cls="Part"):
	n = Node(cls, name, parent)
	n.add(prop_bool("Anchored", anchored))
	n.add(prop_num("BottomSurface", 0))
	n.add(prop_num("TopSurface", 0))
	n.add(prop_token("Material", MATERIALS[material]))
	shape_xml = prop_token("Shape", SHAPES[shape]) if cls == "Part" else ""
	n.add(prop_c3u("Color", color) + prop_num("ChannelCount", 0) + shape_xml)
	n.add(prop_num("Transparency", transparency))
	n.add(prop_bool("CanCollide", collide))
	n.add(prop_v3("Size", *size))
	n.add(prop_num("Reflectance", 0))
	n.add(cf_xml(*pos, *rot))
	if cls in ("Part", "WedgePart", "VehicleSeat"):
		n.add(prop_v3("Velocity", 0, 0, 0))
	if cls == "VehicleSeat":
		n.add(prop_num("MaxSpeed", 0) + prop_num("Torque", 0) + prop_num("TurnSpeed", 0))
		n.add(prop_bool("HeadsUpDisplay", False))
	return n

def surface_text(parent, text, size_px=(6, 1), text_color="#101010", face="Front", scaled=True, bg="#ffffff", bg_trans=0):
	gui = Node("SurfaceGui", "Gui", parent)
	gui.add(prop_token("Face", {"Front": 0, "Top": 1, "Back": 5, "Bottom": 4, "Left": 2, "Right": 3}[face]))
	gui.add(prop_num("LightInfluence", 0.6) + prop_num("Brightness", 1) + prop_num("PixelsPerStud", 50))
	lbl = Node("TextLabel", "Tabliczka", gui)
	lbl.add(
		"<UDim2 name=\"Size\"><XS><scale>1</scale><offset>0</offset></XS><YS><scale>1</scale><offset>0</offset></YS></UDim2>"
	)
	lbl.add(prop_num("BackgroundTransparency", bg_trans) + prop_c3("BackgroundColor3", bg))
	lbl.add(prop_str("Text", text) + prop_bool("TextScaled", scaled) + prop_c3("TextColor3", text_color))
	lbl.add(prop_bool("TextWrapped", False))
	return gui

def proximity_prompt(parent, action="Wejdz"):
	p = Node("ProximityPrompt", "Prompt", parent)
	p.add(prop_str("ActionText", action) + prop_str("ObjectText", "Drzwi"))
	p.add(prop_num("HoldDuration", 0.3))
	return p

def script_node(parent, name, path):
	n = Node("Script", name, parent)
	n.add(prop_bool("Disabled", False))
	with open(path, encoding="utf-8") as f:
		n.add(prop_str("Source", f.read()))
	return n

# ================= sceny =================
lot = Node("Model", "Lotnisko_Modlin_WMI", None)

# --- ziemia ---
ground = part(lot, "Ziemia", (2200, 1, 1400), (0, 0.5, 0), "#4a7c3a", "grass")
runway = part(lot, "Pas_Startowy", (1150, 1, 45), (0, 1, 0), "#46484d", "concrete")
taxi = part(lot, "Droga_Kolowania", (1150, 1, 35), (0, 1, 150), "#66696d", "concrete")
apron = part(lot, "Plyta", (420, 1, 120), (0, 1, 250), "#71747a", "concrete")
for i, sx in enumerate([-450, 450]):
	part(lot, f"Lacznik{i}", (28, 1, 110), (sx, 1, 77.5), "#66696d", "concrete")
	part(lot, f"LacznikPlyta{i}", (26, 1, 25), (sx * 330 / 450, 1, 178.75), "#71747a", "concrete")
# parkuje
part(lot, "Parking", (300, 0.6, 70), (0, 1.05, 430), "#4e5155", "concrete", collide=True)
for i in range(10):
	part(lot, f"ParkingLinia{i}", (0.5, 0.1, 40), (-135 + i * 30, 1.4, 430), "#ffffff", "smooth", transparency=0.2, collide=False)

# --- znaczniki pasa ---
for i, x in enumerate(range(-490, 491, 90)):
	part(lot, f"Srodek{i}", (30, 0.12, 3), (x, 1.56, 0), "#ffffff", "smooth", collide=False)
for end in (-1, 1):
	for i, z in enumerate(range(-18, 19, 6)):
		part(lot, f"Progowy{end}_{i}", (28, 0.12, 3.2), (end * 545, 1.56, z), "#ffffff", "smooth", collide=False)
for i, x in enumerate(range(-450, 451, 150)):
	for z in (-13, 13):
		part(lot, f"StrefaKontakt{i}{z}", (0.1, 0.1, 26), (x, 1.56, z), "#ffffff", "smooth", collide=False)

# --- drogi kolowania na plycie ---
for i, x in enumerate(range(-500, 501, 100)):
	part(lot, f"TaxiLinia{i}", (44, 0.12, 2.4), (x, 1.56, 150), "#f7c600", "smooth", collide=False)

# --- oznaczenia stanowisk (bramki A1..A4) ---
GATES_X = [-150, -50, 50, 150]
bramki = Node("Model", "Bramki", lot)
for idx, gx in enumerate(GATES_X):
	g = Node("Model", f"A{idx+1}", bramki)
	part(g, "Czujnik", (2, 2, 2), (gx, 8, 260), "#ffffff", "smooth", transparency=1, collide=False)
	sensor = part(g, f"Znacznik", (0.8, 0.12, 95), (gx, 1.56, 258), "#f7c600", "smooth", collide=False)
	part(g, "Lider", (60, 0.12, 0.8), (gx, 1.56, 215), "#f7c600", "smooth", collide=False)
	pole = part(g, "Slup", (1, 8, 1), (gx + 26, 5, 285), "#333333", "metal")
	board = part(g, "TabliczkaCzew", (10, 3.4, 1), (gx + 26, 10.5, 285), "#101418", "plastic")
	surface_text(board, f"A{idx+1} WOLNA", text_color="#50ff78", face="Front", bg_trans=1)

# --- rękawy (jet bridges) ---
for gx in GATES_X:
	part(lot, f"MostekSlup{gx}", (5, 13, 5), (gx, 7.5, 296), "#c0c4c8", "concrete")
	part(lot, f"MostekTunel{gx}", (10, 8, 58), (gx, 15.5, 297), "#d9dcdf", "metal")
	part(lot, f"MostekGlowica{gx}", (12, 9, 10), (gx, 15.5, 266), "#e87722", "metal")

# --- terminal ---
terminal = Node("Model", "Terminal", lot)
main_b = part(terminal, "TerminalBudynek", (320, 30, 60), (0, 16, 360), "#e6e8ea", "plastic")
roof = part(terminal, "TerminalDach", (320, 40, 40), (0, 31, 360), "#b03a3a", "metal", shape="cylinder", rot=(0, 0, 90))
part(terminal, "TerminalSzyb", (316, 17, 1.2), (0, 15.5, 330.2), "#7fb4c9", "glass", transparency=0.35)
name_board = part(terminal, "TerminalNapis", (120, 9, 1), (0, 26, 329.4), "#ffffff", "plastic")
surface_text(name_board, "WARSZAWA-MODLIN", text_color="#0a2c6b", face="Front", bg="#ffffff")
part(terminal, "PasPomarancz", (321, 2.5, 61.6), (0, 29.5, 360), "#e87722", "plastic")
for i, px in enumerate((-140, -70, 0, 70, 140)):
	part(terminal, f"Akcent{i}", (4, 26, 1.2), (px, 13.5, 331.2), "#e87722", "plastic", collide=False)
# drzwi (2 grupy)
for dx in (-60, 60):
	door = Node("Model", f"Drzwi{dx}", terminal)
	part(door, "Rama", (14, 12, 1.6), (dx, 6.6, 330.2), "#2a2d31", "metal").add("")
	pr = part(door, "RamaPrompt", (14, 12, 0.5), (dx, 6.4, 331.4), "#ffffff", "glass", transparency=1, collide=False)
	proximity_prompt(pr, "Otworz drzwi")
	part(door, "Lewa", (6, 10.5, 0.5), (dx - 3.2, 6, 329.6), "#8fd0e8", "glass", transparency=0.25)
	part(door, "Prawa", (6, 10.5, 0.5), (dx + 3.2, 6, 329.6), "#8fd0e8", "glass", transparency=0.25)
# tablica odlotow
dep = part(terminal, "TablicaOdlotow", (60, 14, 1.5), (0, 8, 389.3), "#0b0f14", "plastic")
surface_text(dep, "ODLOTY / DEPARTURES\nA1  WAW 10:30  ON TIME\nA2  LTN 11:05  BOARDING\nA3  HAM 12:40  ON TIME\nA4  BUD 13:15  CHECK-IN",
	text_color="#6cf07a", face="Back", bg_trans=1)

# --- wieża kontroli ---
tower = Node("Model", "Wieza", lot)
part(tower, "WiezaBaza", (20, 52, 20), (235, 27, 345), "#d8dadd", "plastic")
part(tower, "WiezaKabina", (14, 20, 14), (235, 64, 345), "#1c2b33", "glass", shape="cylinder", rot=(0, 0, 90), transparency=0.25, collide=True)
part(tower, "WiezaDal", (8, 8, 8), (235, 80, 345), "#e8e9eb", "plastic", shape="ball")
part(tower, "WiezaDach", (16, 2, 16), (235, 75.5, 345), "#98a0a6", "metal")

# --- hangar, straż, zbiorniki ---
part(lot, "Hangar", (90, 30, 70), (-430, 16, 330), "#9aa2a8", "plastic")
part(lot, "HangarDach", (94, 58, 58), (-430, 31, 330), "#7d858b", "metal", shape="cylinder", rot=(0, 0, 90))
part(lot, "HangarDrzwi", (88, 24, 1), (-430, 13, 294.4), "#5f686e", "metal")
part(lot, "Straż", (60, 16, 40), (-320, 9, 400), "#c33b34", "plastic")
for i in range(3):
	part(lot, f"Zbiornik{i}", (22, 11, 11), (-250 + i * 16, 12, 430), "#dfe3e6", "metal", shape="cylinder", rot=(0, 0, 90))

# --- oświetlenie ---
osw = Node("Model", "Oswietlenie", lot)
for i in range(17):
	x = -560 + i * 70
	part(osw, f"Kraw{i}L", (1, 1, 1), (x, 1.8, -23.5), "#efe0b4", "neon", shape="ball", collide=False)
	part(osw, f"Kraw{i}P", (1, 1, 1), (x, 1.8, 23.5), "#efe0b4", "neon", shape="ball", collide=False)
for i in range(8):
	x = -600 - i * 50
	part(osw, f"PodejscieSlup{i}", (0.5, 9, 0.5), (x, 5.5, 0), "#5a5f63", "metal")
	part(osw, f"Podejscie{i+1}", (1.6, 1.6, 1.6), (x, 10.6, 0), "#ffffff", "neon", shape="ball", collide=False)
for i in range(4):
	part(osw, f"PAPI{i+1}", (1.4, 1.4, 1.4), (-520, 1.8, 32 + i * 5), "#ffffff", "neon", shape="ball", collide=False)
part(osw, "Beacon", (2.2, 2.2, 2.2), (235, 85, 345), "#28ff5a", "neon", shape="ball", collide=False)
# maszt wiatrowskazu
part(lot, "WindsockMaszt", (0.8, 14, 0.8), (480, 8, 120), "#d0d3d6", "metal")
part(lot, "WindsockSkarpeta", (6, 1.8, 1.8), (483.4, 14, 120), "#ff7b00", "plastic")

# --- samolot Boeing 737-800 ---
plane = Node("Model", "Boeing737_WMI_100", lot)
main = part(plane, "Main", (13, 13, 130), (-150, 10, 260), "#f2f2f4", "metal", shape="cylinder", rot=(0, 90, 0), collide=True)
part(plane, "Nose", (13, 13, 13), (-150, 10, 321), "#f2f2f4", "metal", shape="ball", rot=(0, 90, 0), collide=True)
part(plane, "Ogon", (8, 8, 12), (-150, 10, 199), "#f2f2f4", "metal", shape="cylinder", rot=(0, 90, 0), collide=True)
part(plane, "KokpitSzyba", (10.5, 2.6, 1), (-150, 11.5, 314), "#14232c", "glass", rot=(0, 90, 0))
part(plane, "OknaPrawe", (0.8, 1.2, 96), (-143.4, 11.5, 258), "#182a34", "smooth", transparency=0.25, collide=False)
part(plane, "OknaLewe", (0.8, 1.2, 96), (-156.6, 11.5, 258), "#182a34", "smooth", transparency=0.25, collide=False)
part(plane, "KrytykPlatL", (52, 1.3, 17), (-183, 9.6, 252), "#ececee", "metal", rot=(0, -12, 0), collide=False)
part(plane, "KrytykPlatP", (52, 1.3, 17), (-117, 9.6, 252), "#ececee", "metal", rot=(0, 12, 0), collide=False)
part(plane, "SilnikL", (14, 6.4, 6.4), (-196, 5.6, 256), "#5b6066", "metal", shape="cylinder", rot=(0, 90, 0), collide=False)
part(plane, "SilnikP", (14, 6.4, 6.4), (-104, 5.6, 256), "#5b6066", "metal", shape="cylinder", rot=(0, 90, 0), collide=False)
part(plane, "ChwytSilnikL", (14.6, 5.4, 1), (-196, 5.6, 262.6), "#f4c542", "metal", rot=(0, 90, 0))
part(plane, "ChwytSilnikP", (14.6, 5.4, 1), (-104, 5.6, 262.6), "#f4c542", "metal", rot=(0, 90, 0))
fin = part(plane, "Statecznik", (1.6, 22, 18), (-150, 21, 199), "#0a2c6b", "metal", cls="WedgePart", rot=(0, 0, 0), collide=False)
part(plane, "StabilL", (13, 0.9, 9), (-159, 15, 197), "#0a2c6b", "metal", cls="WedgePart", rot=(0, 0, 0), collide=False)
part(plane, "StabilP", (13, 0.9, 9), (-141, 15, 197), "#0a2c6b", "metal", cls="WedgePart", rot=(0, 0, 0), collide=False)
pod = Node("Model", "Podwozie", plane)
part(pod, "PodwozieNoseGolen", (1.4, 6.5, 1.4), (-150, 4.6, 303), "#84888c", "metal", transparency=0.25, collide=False)
part(pod, "PodwozieNoseKolo", (6, 2.4, 2.4), (-150, 2.5, 303), "#232527", "metal", collide=False)
part(pod, "PodwozieMGolenL", (1.6, 6.5, 1.6), (-162, 4.6, 241), "#84888c", "metal", transparency=0.25, collide=False)
part(pod, "PodwozieMGolenP", (1.6, 6.5, 1.6), (-138, 4.6, 241), "#84888c", "metal", transparency=0.25, collide=False)
part(pod, "PodwozieMKoloL", (6.4, 2.6, 2.6), (-162, 2.6, 241), "#232527", "metal", collide=False)
part(pod, "PodwozieMKoloP", (6.4, 2.6, 2.6), (-138, 2.6, 241), "#232527", "metal", collide=False)
flaps = Node("Model", "Klapy", plane)
part(flaps, "KlapaL", (50, 0.7, 4), (-183, 9.2, 244), "#c9ccd0", "metal", rot=(0, -12, 0), transparency=1, collide=False)
part(flaps, "KlapaP", (50, 0.7, 4), (-117, 9.2, 244), "#c9ccd0", "metal", rot=(0, 12, 0), transparency=1, collide=False)
seat = part(plane, "PilotSeat", (3, 1, 3.5), (-150, 17.4, 302), "#11335f", "plastic", cls="VehicleSeat", collide=True)
part(plane, "KokpitPodest", (7, 7, 0.4), (-150, 16.6, 302), "#b8bcc2", "metal")
plane.add(f'<Ref name="PrimaryPart">{main.id}</Ref>')

# --- NPC: marshaller w kamizelce ---
def npc(parent, name, x, z, vest="#e87722", pants="#26324a"):
	n = Node("Model", name, parent)
	part(n, "Nogi", (0.9, 2.2, 0.9), (x, 2.1, z), pants, "plastic", collide=False)
	torso = part(n, "Korpus", (2, 2, 1), (x, 4.2, z), vest, "plastic", collide=False)
	part(n, "Glowa", (1.2, 1.2, 1.2), (x, 5.9, z), "#e0b48c", "plastic", shape="ball", collide=False)
	for sx in (-1.45, 1.45):
		part(n, f"Reka{sx}", (0.55, 2, 0.55), (x + sx, 5.1, z), "#ff8b3d", "neon", collide=False)
	n.add(f'<Ref name="PrimaryPart">{torso.id}</Ref>')
	return n

npc(lot, "Marshaller", -104, 232)

# --- pasazerowie idacy na poklad ---
pasazerowie = Node("Model", "Pasazerowie", lot)
random.seed(7)
for i in range(1, 5):
	npc(pasazerowie, f"Pasazer{i}", -166 + i * 6, 296, vest=random.choice(["#3a7bd5", "#c94f7c", "#4aa96c", "#d9a441"]), pants="#33363b")

# --- las sosnowy ---
las = Node("Model", "Las", lot)
placed = 0
while placed < 45:
	tx = random.uniform(-1050, 1050)
	tz = random.uniform(-650, 650)
	if -680 < tx < 680 and -560 < tz < 500:
		continue
	placed += 1
	s = random.uniform(0.8, 1.5)
	part(las, f"Pin{placed}", (1.5 * s, 5 * s, 1.5 * s), (tx, 1 + 2.5 * s, tz), "#6b4a2a", "wood", shape="cylinder")
	part(las, f"Pin{placed}Ig", (5 * s, 7 * s, 5 * s), (tx, 1 + 5.5 * s, tz), "#1e5c34", "plastic", shape="ball")
	part(las, f"Pin{placed}Ig2", (3.4 * s, 4.6 * s, 3.4 * s), (tx, 1 + 10.5 * s, tz), "#26794a", "plastic", shape="ball")

# --- pojazdy naziemne ---
pojazdy = Node("Model", "Pojazdy", lot)

def wheels(parent, x, z, w_off, z_off, r=1.3, ww=1.2):
	dxs = (-w_off, w_off) if w_off else (0,)
	for sx in dxs:
		for sz in (z_off, -z_off):
			part(parent, f"Kolo{sx}_{sz}", (ww, r * 2, r * 2), (x + sx, 1.5 + r, z + sz), "#1f2124", "metal", shape="cylinder")

def vehicle(name, x, z, color, route=None, maxspd=26):
	v = Node("Model", name, pojazdy)
	ch = part(v, "Chassis", (7, 3, 12), (x, 3, z), color, "metal")
	seat = part(v, "Kierowca", (3, 1, 3.5), (x + 1.6, 6.2, z), "#111316", "plastic", cls="VehicleSeat", collide=True)
	return v, ch, seat

v, ch, seat = vehicle("FollowMe", -195, 205, "#f2c219", route="-330,150;300,150;-330,150")
part(v, "Kabina", (5.5, 2.4, 4), (-195, 5.7, 205), "#f2c219", "plastic")
part(v, "Klosz", (4.5, 2.4, 1), (-195, 5.7, 210.4), "#ffffff", "glass", transparency=0.2)
wheels(v, -195, 205, 3.6, 4)

v, ch, seat = vehicle("PushbackTug", -195, 222, "#1c4fa0", maxspd=20)
part(v, "Kabina", (5.5, 2.2, 4), (-195, 5.6, 222), "#1c4fa0", "plastic")
part(v, "Uchwyt", (2, 2.4, 2), (-195, 3, 229), "#33363a", "metal")
wheels(v, -195, 222, 3.7, 4.4, r=1.6)

v, ch, seat = vehicle("Bagazowka", -204, 241, "#e8e9eb", route="-204,241;60,243;-204,241", maxspd=20)
part(v, "Kabina", (5, 3, 4.5), (-204, 6.6, 242.8), "#e8e9eb", "plastic")
part(v, "Przyczepa", (6, 0.8, 11), (-204, 3.2, 227.5), "#b9bdc1", "metal")
part(v, "Bagaż1", (3.4, 2.4, 5), (-204, 5, 227.5), "#7a4a21", "plastic")
part(v, "Bagaż2", (3.4, 2.4, 5), (-204, 5, 232.5), "#3a5c8f", "plastic")
wheels(v, -204, 241, 3.4, 3.4, r=1.1)

v, ch, seat = vehicle("BeltLoader", -195, 258, "#f2c219", maxspd=18)
part(v, "Podest", (6, 5, 7), (-195, 6.2, 258), "#f2c219", "plastic")
part(v, "Tasmo", (3.4, 1, 11), (-195, 8.8, 249), "#33363a", "plastic", cls="WedgePart", collide=False)
wheels(v, -195, 258, 3.5, 3.5, r=1.3)

v, ch, seat = vehicle("Tankowoz", -195, 275, "#c8ccd0", maxspd=18)
part(v, "ZbiornikPojazd", (9, 9, 26), (-195, 8.4, 275), "#dfe3e6", "metal", shape="cylinder", rot=(0, 90, 0))
part(v, "Kabina", (5.5, 3.4, 5), (-195, 6.4, 289), "#c8ccd0", "plastic")
part(v, "Waz", (2, 2, 6), (-195, 4.6, 265), "#586069", "metal")
wheels(v, -195, 275, 3.9, 8, r=1.5)

v, ch, seat = vehicle("Schody", -195, 296, "#5a6166", maxspd=18)
part(v, "Podstawa", (8, 1.6, 4), (-195, 2.6, 294), "#85898d", "metal")
for i in range(7):
	part(v, f"Stopien{i}", (7.5, 0.8, 2), (-195, 3.4 + i * 2.1, 296.2 - i * 2), "#a7abb0", "metal", collide=False)
part(v, "Barierka", (7.5, 0.6, 0.4), (-195, 17.4, 294.6), "#f2c219", "metal")
wheels(v, -195, 296, 3.4, 3, r=1.2)

v, ch, seat = vehicle("Bus", -173, 260, "#2f9e44", maxspd=22)
part(v, "Nadwozie", (8.8, 7, 32), (-173, 7, 260), "#2f9e44", "plastic")
part(v, "OknaBus", (9.2, 2.6, 26), (-173, 9, 259), "#9fd8ef", "glass", transparency=0.3, collide=False)
wheels(v, -173, 260, 4.1, 11, r=1.7)

v, ch, seat = vehicle("Catering", -173, 300, "#e8e9eb", maxspd=16)
part(v, "Nadwozie", (8.6, 7, 22), (-173, 8.2, 301), "#e8e9eb", "plastic")
part(v, "OknoCatering", (8.8, 2, 4), (-173, 10.4, 292), "#9fd8ef", "glass", transparency=0.3, collide=False)
part(v, "Lodka", (5.5, 3, 5), (-173, 5.6, 310), "#c8ccd0", "plastic")
wheels(v, -173, 300, 3.9, 8, r=1.5)

# --- skrypty: samolot ---
script_node(plane, "Samolot_Lot", f"{SRC}/Samolot_Lot.server.lua")

# --- skrypty: pojazdy ---
for vmodel in pojazdy.children:
	script_node(vmodel, "Pojazd_Sterowanie", f"{SRC}/Pojazdy_Sterowanie.server.lua")

# --- skrypty: systemy (folder Zarzadca) ---
z = Node("Model", "Zarzadca", lot)
script_node(z, "Bramki_System", f"{SRC}/Bramki_System.server.lua")
script_node(z, "Operacje_Naziemne", f"{SRC}/Operacje_Naziemne.server.lua")
script_node(z, "Oswietlenie", f"{SRC}/Oswietlenie.server.lua")
script_node(z, "Ruch_Pasazerow", f"{SRC}/Ruch_Pasazerow.server.lua")

lot.add(prop_str("Name", "Lotnisko_Modlin_WMI"))
write([lot])
print("OK, parts:", _ref[0])

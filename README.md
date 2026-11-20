# Lotnisko Modlin (WMI) — projekt do Roblox Studio 🛫

Kompletny, gotowy do grania projekt lotniska Warszawa-Modlin dla Roblox Studio:
pas startowy 08/26, terminal z rękawami (jet bridge), wieża kontroli, hangar,
straż pożarna, Boeing 737-800, pojazdy obsługi naziemnej i 10 skryptów systemowych.

## Szybka instalacja (2 minuty)

1. Pobierz repo: zielony przycisk **Code → Download ZIP** i rozpakuj.
2. Odtwórz plik modelu:
   - **Windows**: kliknij prawym na `Odtworz_Model.ps1` → **Uruchom za pomocą PowerShell** (powstanie plik `ModlinAirport.rbxmx`),
   - **Mac/Linux**: w terminalu uruchom `./Odtworz_Model.sh`.
3. Otwórz pusty plik (Baseplate) w **Roblox Studio**.
4. W Explorerze kliknij prawym na **Workspace → Insert From File...** i wybierz `ModlinAirport.rbxmx`.
5. Gotowe! Całe lotnisko pojawi się w Workspace razem ze skryptami.

## Co jest w środku

| Element | Opis |
|---|---|
| Pas startowy | 1150 studów, znaki, progi, strefy kontaktu, krawędziowe lampki (migają) |
| Terminal | Budynek z łukowym dachem, szyba, automatyczne drzwi (E), tablica odlotów, napis MODLIN AIRPORT |
| Wieża kontroli | Kabina, radar, migający beacon zielono-biały |
| Bramki A1–A4 | Stanowiska z czujnikami — tabliczka zmienia się na ZAJĘTA/WOLNA |
| Rękawy | Jet bridge przy każdej bramce |
| Samolot | Boeing 737-800 (malowanie WMI: niebieski statecznik, żółte chwty silników), wciągane podwozie i klapy |
| Pojazdy | Follow-Me, puszback, bagażówka z przyczepą, tankowóz, autobus, schody |
| Obsługa naziemna | Skrypt automatycznie: schody → autobus → bagażówka → tankowóz przy bramce A1 |
| Oświetlenie | Sekwencja świateł podejściowych (rabbit), PAPI 2/2, krawędź pasa |

## Sterowanie

**Samolot** (usiądź na siedzeniu pilota na nosie kadłuba):
- **W / S** — przepustnica (gaz)
- **A / D** — ster kierunkowy (skręt)
- Powyżej 85 prędkości = samolot startuje sam (siła nośna)
- Poniżej 55 prędkości = opada (lądowanie)
- Czat: `/gear` podwozie, `/flaps` klapy, `/brakes` hamulec, `/reset` wypoziomowanie

**Pojazdy naziemne** (usiądź na siedzeniu kierowcy):
- **W / S** — przód/tył, **A / D** — kierownica
- Bez kierowcy: Follow-Me i bagażówka jeżdżą automatycznie po swojej trasie patrolowej

**Drzwi terminala**: podejdź i wciśnij **E** — drzwi rozsuwają się automatycznie.

## Struktura repo

```
ModlinAirport.rbxmx.gz.b64 ← skompresowany model lotniska (rozpakowuje Odtworz_Model.ps1 / .sh)
Odtworz_Model.ps1        ← Windows: odtwarza plik ModlinAirport.rbxmx
Odtworz_Model.sh         ← Mac/Linux: odtwarza plik ModlinAirport.rbxmx
src/                     ← wszystkie skrypty Lua (źródła)
  Samolot_Lot.server.lua
  Pojazdy_Sterowanie.server.lua
  Bramki_System.server.lua
  Operacje_Naziemne.server.lua
  Oswietlenie.server.lua
tools/generate_rbxmx.py  ← generator modelu (zmień skrypty i przegeneruj)
default.project.json     ← projekt Rojo (opcjonalnie, dla zaawansowanych)
```

## Rozbudowa

Zmień coś w `src/` i przegeneruj model:
```bash
python3 tools/generate_rbxmx.py
```
albo edytuj skrypty bezpośrednio w Roblox Studio po imporcie.

## Koordynaty (dla twórców)

- Pas startowy: 1150×45 studów, na osi X (kierunek 08/26)
- Droga kołowania równoległa: oś Z = 150
- Płyta postojowa: Z = 190…310
- Terminal: Z = 330…390
- Bramki A1–A4: X = −150/−50/50/150, Z = 260

Miłej zabawy! ✈️

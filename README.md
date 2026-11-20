# Lotnisko Modlin (WMI) — projekt do Roblox Studio 🛫

Kompletny, gotowy do grania projekt lotniska Warszawa-Modlin dla Roblox Studio:
pas startowy 08/26, terminal z rękawami (jet bridge), wieża kontroli, hangar,
straż pożarna, Boeing 737-800, pojazdy obsługi naziemnej i 10 skryptów systemowych.

## Szybka instalacja (2 minuty)

1. Pobierz repo: zielony przycisk **Code → Download ZIP** i rozpakuj.
2. Otwórz pusty plik (Baseplate) w **Roblox Studio**.
3. W Explorerze kliknij prawym na **Workspace → Insert From File...** i wybierz `ModlinAirport.rbxmx` — **plik jest już w rozpakowanym ZIP-ie, gotowy!**
4. Gotowe! Całe lotnisko pojawi się w Workspace razem ze skryptami.

> `ModlinAirport.rbxmx.gz.b64` + `Odtworz_Model.bat` / `.sh` są potrzebne tylko, gdybyś pobrał pojedyncze pliki zamiast ZIP-a - wtedy na Windows kliknij dwukrotnie **Odtworz_Model.bat** (sam odblokuje PowerShell).

## Co jest w środku

| Element | Opis |
|---|---|
| Pas startowy | 1150 studów, znaki, progi, strefy kontaktu, krawędziowe lampki (migają) |
| Terminal | Budynek z łukowym dachem, **pomarańczowe akcenty (styl Modlin)**, szyba, automatyczne drzwi (E), tablica odlotów, napis **WARSZAWA-MODLIN** |
| Wieża kontroli | Kabina, radar, migający beacon zielono-biały |
| Bramki A1–A4 | Stanowiska z czujnikami — tabliczka zmienia się na ZAJĘTA/WOLNA |
| Rękawy | Jet bridge przy każdej bramce |
| Samolot | Boeing 737-800 (malowanie WMI: niebieski statecznik, żółte chwty silników), wciągane podwozie i klapy |
| Pojazdy | Follow-Me, puszback, bagażówka z przyczepą i bagażem, **taśmociąg bagażowy (belt loader)**, tankowóz, **lotniskówka (catering)**, autobus, schody |
| Obsługa naziemna | Skrypt automatycznie: schody → autobus → bagażówka → tankowóz → taśmociąg → lotniskówka przy bramce A1 |
| Marshaler | NPC w pomarańczowej kamizelce z lichtenami przy bramce A1 |
| Pasażerowie | 4 NPC idących na płycie: autotobus ↔ schody ↔ terminal |
| Oświetlenie | Sekwencja świateł podejściowych (rabbit), PAPI 2/2, krawędź pasa |
| Otoczenie | Las sosnowy wokół lotniska (45 drzew), zielone pola Mazowsza |

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
ModlinAirport.rbxmx      ← GOTOWY MODEL do importu w Roblox Studio (całe lotnisko + skrypty)
ModlinAirport.rbxmx.gz.b64 ← skompresowana kopia modelu (opcjonalna; rozpakowuje Odtworz_Model.ps1 / .sh)
Odtworz_Model.bat        ← Windows: odtwarza plik ModlinAirport.rbxmx z kopii .b64 (kliknij dwukrotnie)
Odtworz_Model.ps1        ← to samo co .bat, wersja PowerShell
Odtworz_Model.sh         ← Mac/Linux: odtwarza plik ModlinAirport.rbxmx z kopii .b64
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

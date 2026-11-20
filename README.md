# POLSKIE MIASTA — gra 3D w Unity 🚗📦

Wielki świat z **5 miastami połączonymi drogami**, w każdym: paczkomat, salon
samochodowy, magazyn, domki, latarnie, ruch uliczny. W Słonecznym Brzegu jest
też plaża ze **morzem, wieżowcami i palmami**.

## Miasta
1. **Kasztanowice**
2. **Słoneczny Brzeg** (bardzo duże miasto + plaża)
3. **Solina**
4. **Białowoda**
5. **Krasnogród** (centrum mapy — stąd startujesz)

## JAK WGRAĆ (Unity Hub + Unity 2022.3 LTS)

1. Zainstaluj [Unity Hub](https://unity.com/download) i pobierz wersję **2022.3 LTS** (element "3D Core" wystarczy).
2. Stwórz nowy projekt **3D (Built-in Render Pipeline)**.
3. Pobierz repo: zielony przycisk **Code → Download ZIP** i rozpakuj.
4. Skopiuj folder **`Unity/PolskieMiasta`** do katalogu **`Assets`** projektu
   (czyli będą ścieżki `Assets/PolskieMiasta/Scripts` i `Assets/PolskieMiasta/Editor`).
5. Poczekaj aż Unity skompiluje skrypty (bez czerwieni w konsoli).
6. **Gotowe!** Po skompilowaniu świat buduje się SAM (widać go już w scenie). Zapisz **Ctrl+S**.
7. Wciśnij **PLAY** — grasz! (Jeśli gdyby świat się nie pojawił: menu **Tools → Polskie Miasta - Zbuduj wielki swiat**.)

## STEROWANIE
- `W/A/S/D` — chodzenie i jazda autem, `Shift` — bieg
- `Mysz` — rozglądanie
- `E` — interakcja (wejście do auta, sprzedawca salonu, magazyn, paczkomat)
- `H` — pokazywanie/chowanie telefonu
- `ESC` — zamknięcie okienka

## JAK GRAĆ

### Salon — klikasz w ludzika i wybierasz auto
1. Znajdź **SALON SAMOCHODOWY** (szklany budynek z pomarańczowym pasem).
2. Celuj w **sprzedawcę** (ludzik przy wejściu) i wcisnij **E** — pojawia się menu aut.
3. **KUP** (za coins) albo **WYBIERZ** — auto pojawia się obok i OD RAZU w nim jedziesz.

### Kurier — zarabiasz COINS
1. Telefon (`H`) → **PACZKI** → **JESTEM KURIEREM** → **WEZ ZLECENIE**.
2. Dojedź do **MAGAZYNU** w wskazanym mieście, celuj w bramę magazynu, **E** → **WEZ PACZKE**.
3. Paczka jest widoczna przy tobie/na aucie. Dojedź do **PACZKOMATU** w innym mieście.
4. Przy paczkomacie **E** → **SKANUJ I WSADZ PACZKE** → skrytka otwiera się, paczka wchodzi,
   zamyka się. **Coins wpadają na konto!**

### Klient — zamawiasz paczkę (np. laptopa)
1. Telefon → **PACZKI** → **JESTEM KLIENTEM** → wybierz np. **Laptop** → **ZAMOW**.
2. Po ~20–35 s przyjdzie powiadomienie: **"PACZKA DOTARLA! Kod 123456"**.
3. Idź do paczkomatu swojego miasta, **E**, na klawiaturze wpisz 6-cyfrowy kod → **OK** →
   skrytka się otwiera → **ZABIERZ PACZKE** → skrytka się zamyka (+20 coins).

### Sklep w telefonie
Turbo silnik (+12 prędkości), stawka kuriera +50%, złoty lakier.

### Admin panel (tylko jeden login!)
Telefon → **ADMIN**. Login: **admin**, hasło: **admin123**.
Funkcje: +1000 coins, odblokowanie wszystkich aut, teleport do każdego miasta, wylogowanie.
Jak zmienić login/hasło: w `Scripts/PhoneController.cs` na samej górze są stałe `ADMIN_LOGIN` i `ADMIN_PASS`.

## PLIKI
```
Unity/PolskieMiasta/Scripts/   — GameManager, telefon, paczkomaty, magazyn, salon, auta, gracz
Unity/PolskieMiasta/Editor/    — CityWorldBuilder (menu Tools → buduj cały świat)
Unity/README-Unity.md          — szczegółowa instrukcja
```

## Ważne
- Świat jest stylizowany (bez fotorealistycznych tekstur) — tekstury można później podmienić na własne (darmowe z itch.io / Unity Asset Store).
- Jeśli w konsoli Unity pojawi się czerwień (błąd kompilacji) — wklej jej treść, a zostanie poprawiona.
- Coins i zapis gry trzymane są w PlayerPrefs (postęp zostaje po wyłączeniu gry).

Miłej jazdy! 🚗💨

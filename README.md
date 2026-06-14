# Quebec — demo gry horror

Projekt **Quebec** to prototyp gry horror w Unity, skupiony na eksploracji, napięciu i interakcjach środowiskowych w widoku FPP.

## Wymagania

- Unity **6000.4.7f1** (Unity 6.4)
- Pakiety używane w projekcie:
  - Input System
  - Universal Render Pipeline (URP)
  - UGUI / TextMeshPro

## Co zawiera demo

- **Ruch FPP i kamera** (chodzenie, obrót, przechyły kamery).
- **Interakcje z obiektami** przez raycast:
  - podświetlanie obiektów,
  - podpowiedź akcji nad obiektem,
  - interakcja klawiszem `F`.
- **Dynamiczny kursor** zmieniający wygląd, gdy gracz patrzy na obiekt interaktywny.
- **Ekwipunek 3-slotowy** (Lidar, Latarka, pusta ręka) z przełączaniem `1/2/3`.
- **Lidar**:
  - skan falą punktów,
  - cooldown,
  - gradient kolorów zależny od dystansu,
  - efekt grozy po wykryciu obiektu z tagiem `Monster` (dźwięk, shake kamery, winieta).
- **Latarka**:
  - włączanie/wyłączanie,
  - zużycie baterii,
  - uzupełnianie baterii przez pickupy.
- **System dialogów i napisów** (sekwencje audio + fragmenty tekstu).
- **Trigger jumpscare** uruchamiany po wejściu gracza w strefę.

## Sterowanie (domyślne)

- `W/A/S/D` — ruch
- Mysz — rozglądanie
- `F` — interakcja z obiektami
- `1`, `2`, `3` — zmiana aktywnego slotu
- `LPM` — użycie aktywnego narzędzia (np. Lidar / Latarka)

## Uruchomienie projektu

1. Otwórz projekt w Unity Hub.
2. Wybierz edytor w wersji **6000.4.7f1**.
3. Otwórz scenę startową:
   - `Assets/Scenes/SampleScene.unity`
4. Uruchom grę przyciskiem **Play**.

## Historia rozwoju (na podstawie commitów)

- **PR #13 / QBC-20 — LIDAR**  
  Dodanie i integracja mechaniki skanera Lidar wraz z elementami rozgrywki.

- **PR #14 / QBC-31 — interaktywny kursor**  
  Dodanie kursora zmieniającego kształt przy celowaniu w obiekty, z którymi można wejść w interakcję.

## Status

To projekt demonstracyjny/prototypowy rozwijany iteracyjnie.

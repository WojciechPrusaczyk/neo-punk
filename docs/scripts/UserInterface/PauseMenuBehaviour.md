[Powrót do spisu treści](../../contents.md)

### PauseMenuBehaviour

**Lokalizacja:** [`Assets/Code/Scripts/UserInterface/PauseMenuBehaviour.cs`](../../../Assets/Code/Scripts/UserInterface/PauseMenuBehaviour.cs)

Klasa `PauseMenuBehaviour` zarządza funkcjonalnością menu pauzy w grze, w tym zatrzymywaniem rozgrywki, wznawianiem jej oraz wyjściem do menu głównego lub zamknięciem gry.

#### Pola Prywatne

- `GameObject PauseUi` - główny obiekt UI dla menu pauzy, inicjalizowany jako aktualny obiekt (this game object).
- `GameObject PauseUiCanvas` - obiekt UI reprezentujący główną część menu pauzy, czyli Canvas, który zostaje wyłączony na początku.
- `PlayerInventory PlayerInv` - odniesienie do komponentu `PlayerInventory` gracza, wykorzystywane do sprawdzania, czy ekwipunek jest otwarty.
- `GameObject MainUi` - odniesienie do głównego interfejsu użytkownika gry, które zostaje ukryte, gdy menu pauzy jest aktywne.

#### Pole Publiczne

- `bool IsGamePaused` - informuje, czy gra jest aktualnie zatrzymana (pauza).

#### Metody

1. **Awake()**
    - Wywoływana podczas inicjalizacji komponentu.
    - Ustawia początkowe wartości i odniesienia dla obiektów oraz komponentów:
        - Ustawia `IsGamePaused` na `false`.
        - Ukrywa kursor (`Cursor.visible = false`).
        - Inicjalizuje `PauseUiCanvas` jako pierwsze dziecko obiektu `PauseUi` i ustawia jego aktywność na `false`.
        - Inicjalizuje `PlayerInv` poprzez znalezienie obiektu `Player` i pobranie komponentu `PlayerInventory`.
        - Inicjalizuje `MainUi` poprzez znalezienie obiektu `Main User Interface`.

2. **Update()**
    - Sprawdza stan gry oraz interakcji z klawiszem pauzy (`InputManager.PauseMenuKey` lub `KeyCode.Escape`):
        - Jeśli ekwipunek nie jest otwarty (`!PlayerInv.isEquipmentShown`) i gra jest w trybie pauzy, wywołuje metodę `buttonResume()`.
        - Jeśli ekwipunek nie jest otwarty, a gra nie jest w trybie pauzy, wywołuje metodę `Pause()`.

3. **Pause()**
    - Przełącza grę w tryb pauzy:
        - Ustawia `IsGamePaused` na `true`.
        - Ustawia widoczność kursora na `true`.
        - Ukrywa `MainUi`.
        - Uaktywnia `PauseUiCanvas`.
        - Zatrzymuje czas gry (`Time.timeScale = 0`).

4. **buttonResume()**
    - Wznawia grę z trybu pauzy:
        - Ustawia `IsGamePaused` na `false`.
        - Ukrywa kursor (`Cursor.visible = false`).
        - Wyłącza `PauseUiCanvas`.
        - Ponownie wyświetla `MainUi`.
        - Wznawia czas gry (`Time.timeScale = 1`).

5. **buttonOptions()**
    - (Pusta metoda) Przeznaczona do rozszerzenia o opcje ustawień w menu pauzy.

6. **buttonQuitToMenu()**
    - Wywoływana przy wyjściu do menu głównego:
        - Przywraca czas gry (`Time.timeScale = 1`).
        - Ładuje scenę o nazwie `MainMenu` za pomocą `SceneManager.LoadScene`.

7. **buttonQuitGame()**
    - Wywoływana przy zamknięciu gry:
        - Wywołuje `Application.Quit()` do zamknięcia aplikacji.

[Powrót do spisu treści](../../../codeContents.md)

### PauseMenuBehaviour

**Lokalizacja:** [`Assets/Code/Scripts/UserInterface/Dialog/DialogScript.cs`](../../../../Assets/Code/Scripts/UserInterface/Dialog/DialogScript.cs)

Klasa `DialogScript` obsługuje wyświetlanie dialogów w grze, w tym zarządzanie UI, przełączanie linii dialogowych, animację tekstu oraz podświetlanie aktywnych postaci.

---

## Pola Prywatne

### `DialogData dialogData`
- **Opis**: Dane dialogowe pobierane z zasobu `DialogData`.
- **Typ**: `DialogData`.

### `VisualElement root`
- **Opis**: Główny element interfejsu użytkownika, od którego rozpoczyna się hierarchia elementów UI.
- **Typ**: `VisualElement`.

### `VisualElement characterOnePictureElement`
- **Opis**: Element UI reprezentujący obrazek pierwszej postaci.
- **Typ**: `VisualElement`.

### `VisualElement characterTwoPictureElement`
- **Opis**: Element UI reprezentujący obrazek drugiej postaci.
- **Typ**: `VisualElement`.

### `Label dialogTextElement`
- **Opis**: Element tekstowy wyświetlający dialog.
- **Typ**: `Label`.

### `Label characterOneNameElement`
- **Opis**: Element UI wyświetlający imię pierwszej postaci.
- **Typ**: `Label`.

### `Label characterTwoNameElement`
- **Opis**: Element UI wyświetlający imię drugiej postaci.
- **Typ**: `Label`.

### `Coroutine typingCoroutine`
- **Opis**: Referencja do animacji tekstu wyświetlanego w dialogu.
- **Typ**: `Coroutine`.

### `int currentLineIndex`
- **Opis**: Indeks aktualnie wyświetlanej linii dialogowej.
- **Typ**: `int`.

---

## Metody

### `OnEnable()`
- **Opis**: Inicjalizuje elementy interfejsu użytkownika po aktywowaniu obiektu.
- **Działanie**:
    1. Pobiera komponent `UIDocument` i ustawia `root` jako główny element interfejsu.
    2. Wywołuje metodę `LoadUI()` do załadowania elementów UI.

### `Update()`
- **Opis**: Obsługuje interakcję użytkownika z dialogiem.
- **Działanie**:
    1. Reaguje na naciśnięcie spacji, lewego lub prawego przycisku myszy.
    2. Jeśli animacja tekstu jest aktywna, przerywa ją i wyświetla pełną linię dialogową.
    3. W przeciwnym wypadku przełącza na następną linię dialogową.

### `LoadUI()`
- **Opis**: Ładuje elementy interfejsu użytkownika i inicjalizuje dialog.
- **Działanie**:
    1. Pobiera elementy UI dla obrazków, imion postaci oraz tekstu dialogowego.
    2. Inicjalizuje ich wartości na podstawie danych `DialogData`.
    3. Wywołuje metodę `StartDialog()`.

### `SetActiveCharacter(VisualElement photoActiveElement, VisualElement photoInactiveElement, VisualElement textActiveElement, VisualElement textInactiveElement)`
- **Opis**: Ustawia aktywną postać dialogową, podświetlając jej obrazek i imię.
- **Parametry**:
    - `photoActiveElement`: Element UI aktywnej postaci.
    - `photoInactiveElement`: Element UI nieaktywnej postaci.
    - `textActiveElement`: Element tekstowy aktywnej postaci.
    - `textInactiveElement`: Element tekstowy nieaktywnej postaci.

### `StartDialog()`
- **Opis**: Rozpoczyna dialog, wyświetlając pierwszą linię dialogową.
- **Działanie**:
    - Sprawdza, czy dane dialogowe zawierają linie dialogowe.
    - Wywołuje `ShowLine(0)`.

### `ShowLine(int lineIndex)`
- **Opis**: Wyświetla linię dialogową na podstawie indeksu.
- **Parametry**:
    - `lineIndex`: Indeks linii dialogowej.
- **Działanie**:
    1. Pobiera dane tekstowe i mówcę z linii dialogowej.
    2. Wyświetla tekst z animacją.
    3. Podświetla aktywną postać.

### `NextLine()`
- **Opis**: Przełącza dialog na następną linię.
- **Działanie**:
    - Zwiększa `currentLineIndex`.
    - Jeśli indeks przekracza liczbę linii dialogowych, zamyka UI (`CloseUI()`).
    - Wyświetla kolejną linię dialogową.

### `TextAnimation(string textAnimation)`
- **Opis**: Wyświetla tekst dialogowy z animacją liter.
- **Parametry**:
    - `textAnimation`: Tekst do animacji.
- **Zwraca**: `IEnumerator`.

### `CloseUI()`
- **Opis**: Zamyka UI dialogowe po zakończeniu dialogu.
- **Działanie**:
    - Wyłącza komponent `UIDocument`.

---

## Klasa Obsługi UI

### Elementy UI i ich identyfikatory:
- **`CharacterOnePicture`**: Obrazek pierwszej postaci.
- **`CharacterTwoPicture`**: Obrazek drugiej postaci.
- **`CharacterName1`**: Imię pierwszej postaci.
- **`CharacterName2`**: Imię drugiej postaci.
- **`DialogText`**: Tekst dialogowy.

---

## Uwagi

- W przypadku braku danych lub elementów UI generowane są odpowiednie komunikaty błędów, co ułatwia diagnozowanie problemów.
- Klasa wymaga powiązania z zasobem `DialogData` oraz odpowiedniego skonfigurowania elementów UI w edytorze Unity.
- Należy upewnić się, że wszystkie komponenty są dodane, a elementy interfejsu zostały poprawnie przypisane do odpowiednich pól w skrypcie.

## Do Zrobienia!!!

1. Dodanie publicznej metody umożliwiającej wywoływanie funkcji inicjującej dialog z zewnątrz.
2. Stworzenie nowej funkcji zamykającej dialog, która nie będzie reagować na przyciski gracza podczas wyłączania interfejsu dialogowego.
3. Implementacja mechanizmu zatrzymywania gry podczas aktywnego dialogu.
4. Dodanie systemu triggerów umożliwiających automatyczne aktywowanie dialogów w określonych sytuacjach.
5. Sprawdzenie i poprawa potencjalnych błędów związanych z obsługą dialogów lub ich interfejsem.
a zmiana rozmiaru oraz koloru obiektów zdjęć oraz gracz
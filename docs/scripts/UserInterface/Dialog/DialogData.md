[Powrót do spisu treści](../../../codeContents.md)

### PauseMenuBehaviour

**Lokalizacja:** [`Assets/Code/Scripts/UserInterface/Dialog/DialogData.cs`](../../../../Assets/Code/Scripts/UserInterface/Dialog/DialogData.cs)

Klasa `DialogData` służy do zarządzania danymi dialogowymi w grze. Umożliwia tworzenie i przechowywanie dialogów pomiędzy dwoma postaciami, w tym ich imion, wizerunków oraz treści dialogów.

## WAŻNE
**Dialogi powinny być przechowywane w**: `Assets/Level/ScriptableObjects/Dialogs`

---

## Pola Publiczne

### `string characterOneName`
- **Opis**: Imię pierwszej postaci uczestniczącej w dialogu.
- **Typ**: `string`.

### `Sprite characterOnePicture`
- **Opis**: Wizerunek (obrazek) pierwszej postaci.
- **Typ**: `Sprite`.

### `string characterTwoName`
- **Opis**: Imię drugiej postaci uczestniczącej w dialogu.
- **Typ**: `string`.

### `Sprite characterTwoPicture`
- **Opis**: Wizerunek (obrazek) drugiej postaci.
- **Typ**: `Sprite`.

### `List<DialogLine> lines`
- **Opis**: Lista linii dialogowych, które tworzą pełny dialog pomiędzy postaciami.
- **Typ**: `List<DialogLine>`.
- **Szczegóły**: Każdy element listy reprezentuje pojedynczą linię dialogową, zawierającą informacje o mówcy oraz treści wypowiedzi.

---

## Klasa `DialogLine`

Klasa pomocnicza `DialogLine` reprezentuje pojedynczą linię dialogową w rozmowie.

### Pola Publiczne

#### `Talkers speakerName`
- **Opis**: Enum określający, która postać (CharacterOne lub CharacterTwo) wypowiada daną linię dialogową.
- **Typ**: `Talkers`.

#### `string text`
- **Opis**: Tekst wypowiedzi mówcy.
- **Typ**: `string`.

---

## Enum `Talkers`

Enum `Talkers` definiuje, która postać jest mówcą w danej linii dialogowej.

### Wartości:
- **`CharacterOne`**: Pierwsza postać.
- **`CharacterTwo`**: Druga postać.

---

## Tworzenie Obiektu `DialogData`

Klasa `DialogData` może być tworzona w edytorze Unity jako zasób ScriptableObject.  
**Menu Kontekstowe**: `Create > Dialog > DialogData`.
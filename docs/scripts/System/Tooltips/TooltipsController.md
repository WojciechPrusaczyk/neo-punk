[Go back to code contents](../../../codeContents.md)

Klasa `TooltipsController` znajduje się w [`Assets/Code/Scripts/System/Tooltips/TooltipsController.cs`](../../../../Assets/Code/Scripts/System/Tooltips/TooltipsController.cs)

`TooltipsController` zarządza wyświetlaniem podpowiedzi w grze. Przechowuje listę obiektów `Tooltip` z danymi podpowiedzi i zapewnia metody do wyświetlania i zamykania wybranej podpowiedzi. Interfejs graficzny podpowiedzi składa się z obrazu i tekstu, które są aktualizowane, gdy podpowiedź zostaje wyświetlona.

### Klasa `Tooltip`
- Klasa `Tooltip` przechowuje dane dla pojedynczej podpowiedzi, umożliwiając konfigurację obrazu oraz tekstu podpowiedzi.
- **Pola publiczne**:
    - `Sprite image` - obrazek wyświetlany w podpowiedzi.
    - `string text` - tekst podpowiedzi, wyświetlany pod obrazem; obsługuje różne formatowanie poprzez `TextArea`.
    - `bool wasTooltipShown` - określa, czy dana podpowiedź została już wyświetlona. Po pierwszym wyświetleniu jest ustawiane na `true`.

### Publiczne pola klasy `TooltipsController`
- `List<Tooltip> tooltips` - lista obiektów `Tooltip`, przechowująca wszystkie dostępne podpowiedzi. Można je konfigurować w inspektorze Unity.

### Skrypty powiązane i wymagane komponenty
Klasa `TooltipsController` wymaga obiektów UI:
- Obiekt `Image` (z komponentem `Image`) do wyświetlenia obrazka podpowiedzi, o nazwie `TooltipImage`.
- Obiekt `TextMeshProUGUI` do wyświetlenia tekstu podpowiedzi, o nazwie `TooltipText`.

### Metody

#### `void ShowTooltip(int tooltipNumber)`
Wyświetla wybraną podpowiedź, jeśli ustawienia gry zezwalają na pokazywanie podpowiedzi (`OptionsManager.GetShowTips()` zwraca `true`):
- Jeśli podpowiedź o podanym indeksie (`tooltipNumber`) istnieje i nie była jeszcze wyświetlana, jest ona ustawiana jako `shownTooltip`.
- Obraz i tekst podpowiedzi są przypisywane do odpowiednich obiektów UI.
- Ustawia `IsTooltipMenuShown` na `true` i aktywuje UI podpowiedzi.
- Ustawia `wasTooltipShown` na `true` dla danego `Tooltip`, aby nie wyświetlać tej samej podpowiedzi ponownie.

#### `void CloseTooltip()`
Zamyka aktualnie wyświetlaną podpowiedź:
- Dezaktywuje obiekt UI podpowiedzi.
- Resetuje obraz i tekst do wartości domyślnych.
- Ustawia `Time.timeScale` na `1`, aby wznowić czas gry.
- Ustawia `IsTooltipMenuShown` na `false`, aby zakończyć wyświetlanie podpowiedzi.

### Prywatne pola
- `bool IsTooltipMenuShown` - określa, czy podpowiedź jest aktualnie wyświetlana. Blokuje czas gry i umożliwia zamknięcie podpowiedzi przyciskiem.
- `Tooltip shownTooltip` - aktualnie wyświetlana podpowiedź.
- `Image imageObject` - komponent obrazu UI, który wyświetla obraz podpowiedzi.
- `TextMeshProUGUI textObject` - komponent tekstu UI, który wyświetla tekst podpowiedzi.

### Uwagi
- Podpowiedź może być zamknięta za pomocą klawiszy `F` lub `Escape`.
- Jeśli indeks podpowiedzi jest nieprawidłowy lub podpowiedź była już wyświetlana, metoda `ShowTooltip` wypisze ostrzeżenie do konsoli.

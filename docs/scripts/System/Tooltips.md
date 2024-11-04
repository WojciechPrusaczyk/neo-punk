[Go back to contents](../../contents.md)

### TooltipsController
**Lokalizacja:** [`Assets/Scripts/System/Tooltips/TooltipsController.cs`](../../../Assets/Code/Scripts/System/Tooltips/TooltipsController.cs)

Klasa `TooltipsController` to komponent interfejsu użytkownika, który zarządza wyświetlaniem podpowiedzi w grze. Klasa zawiera dodatkową wbudowaną klasę `Tooltip`, służącą do serializacji danych podpowiedzi.

### Konfiguracja Canvasu dla Podpowiedzi
Główny obiekt `TooltipsController` powinien znajdować się w scenie pod adresem `Scene/UserInterface/Tooltips`. Ten obiekt powinien być typu `Canvas` i zawierać przygotowany układ UI do wyświetlania danych podpowiedzi.

### Struktura Danych Podpowiedzi
`TooltipsController` zarządza listą podpowiedzi, gdzie każda z nich zawiera tekstowy opis oraz opcjonalny obrazek. Aby dodać nową podpowiedź, utwórz opis oraz przypisz odpowiedni obrazek.

### Formatowanie Tekstu: 
W tekście podpowiedzi można umieszczać ikony przycisków, używając formatu `<sprite=n>`, gdzie n to indeks wybranego obrazu w pliku `Keycaps.asset` znajdującym się w [`Assets/Art/UserInterface/Keys/Keycaps.asset`](../../../Assets/Art/UserInterface/Keys/Keycaps.asset).

Przykładowy tekst z ikonami przycisków:

```HTML
You can move sideways by using <sprite=14>
and <sprite=17>, and jump using <sprite=36> or <sprite=116>.
Some of the surroundings can be passed through by clicking
<sprite=32> while standing on top of them.
```

Wyświetlanie Podpowiedzi
Aby wyświetlić podpowiedź, wywołaj poniższą metodę:
```
public void ShowTooltip(int tooltipNumber)
```
Gdzie tooltipNumber to indeks podpowiedzi do wyświetlenia.

### Organizacja Plików i Folderów
Wszystkie obrazki podpowiedzi oraz skrypty wywołujące podpowiedzi powinny znajdować się w folderze [`Assets/Art/UserInterface/Tooltips`](../../../Assets/Art/UserInterface/Tooltips).
Dla każdej podpowiedzi uporządkuj pliki w następującej strukturze:

```
- TooltipName
    - image.png // Obrazek dla podpowiedzi
    - tipTooltipName.cs // Opcjonalny skrypt do wywołania podpowiedzi
```
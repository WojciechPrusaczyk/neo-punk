[Go back to code contents](../../../codeContents.md)

Klasa `TooltipTrigger` znajduje się w [`Assets/Code/Scripts/System/Tooltips/TooltipsTrigger.cs`](../../../../Assets/Code/Scripts/System/Tooltips/TooltipsTrigger.cs)

`TooltipTrigger` jest odpowiedzialny za wywoływanie podpowiedzi, gdy gracz wchodzi w obszar wyzwalacza (`Trigger Zone`). Dzięki polu `UnityEvent`, klasa pozwala skonfigurować reakcję, jaką system podejmie, gdy gracz (oznaczony tagiem "Player") wejdzie w strefę wyzwalacza.

### Publiczne pola klasy `TooltipTrigger`
- `UnityEvent onTriggerEnter` - zdarzenie, które można skonfigurować w inspektorze Unity, aby uruchomić podpowiedź lub inną akcję po wejściu w obszar wyzwalacza.

### Skrypty powiązane i wymagane komponenty
- Klasa `TooltipTrigger` wymaga obecności obiektu `TooltipsController` w hierarchii gry, zlokalizowanego w `UserInterface/Tooltips`. Obiekt ten powinien posiadać komponent `TooltipsController` do obsługi wywoływanych podpowiedzi.

### Metody

#### `void Awake()`
Inicjalizuje referencję do `TooltipsController`:
- Wyszukuje obiekt `UserInterface`, następnie przechodzi do jego potomka `Tooltips`, a na końcu przypisuje komponent `TooltipsController`.

#### `void OnTriggerEnter2D(Collider2D other)`
Wywoływana automatycznie, gdy gracz (`Player`) wchodzi w obszar wyzwalacza:
- Sprawdza, czy obiekt `other` posiada tag `Player`.
- Jeśli warunek jest spełniony, wywołuje `onTriggerEnter.Invoke()` - czyli konfigurację zdarzenia ustawionego w inspektorze.
- Dezaktywuje obiekt wyzwalacza (`gameObject.SetActive(false)`), aby uniemożliwić ponowne wywołanie zdarzenia.

### Uwagi
- Obiekt z komponentem `TooltipTrigger` powinien mieć przypisany tag `Player`, aby poprawnie wykrywać wejścia do strefy wyzwalacza.
- Klasa umożliwia dostosowanie akcji wykonywanej przy wejściu gracza w obszar wyzwalacza, co jest przydatne w przypadku systemów podpowiedzi.

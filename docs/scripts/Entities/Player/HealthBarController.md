[Go back to code contents](../../../codeContents.md)

### HitboxBehaviour

**Lokalizacja:** [`Assets/Code/Scripts/Entities/Player/HealthBarController.cs`](../../../../Assets/Code/Scripts/Entities/Player/HealthBarController.cs)

## Opis
Klasa `HealthBarController` odpowiada za wizualne przedstawienie poziomu zdrowia (`Health`) gracza na interfejsie użytkownika (UI) w grze. Obiekt wyposażony w tę klasę kontroluje szerokość paska zdrowia, proporcjonalnie do bieżącego poziomu zdrowia gracza, umożliwiając graczowi śledzenie zmian w swoim stanie zdrowia na bieżąco.

## Pola

- `public float CurrentHealthPercent`  
  Procentowy poziom zdrowia gracza, obliczany na podstawie stosunku aktualnego zdrowia do maksymalnego zdrowia gracza. Wartość ta jest aktualizowana co klatkę, aby zachować bieżący stan zdrowia.

- `private GameObject playerObject`  
  Obiekt gracza, którego zdrowie jest monitorowane i wyświetlane na pasku zdrowia.

- `private EntityStatus playerStatus`  
  Komponent klasy `EntityStatus` przypisany do gracza, który dostarcza informacji o bieżącym i maksymalnym zdrowiu gracza.

- `public GameObject healthDisplay`  
  Obiekt UI reprezentujący wizualizację paska zdrowia.

- `private Image healthDisplayImage`  
  Komponent `Image` z obiektu `healthDisplay`, który odpowiada za faktyczną grafikę paska zdrowia, której szerokość jest dynamicznie zmieniana.

- `private float initialWidth`  
  Początkowa szerokość paska zdrowia, zapisywana podczas inicjalizacji, aby umożliwić proporcjonalną zmianę rozmiaru w miarę spadku zdrowia.

## Metody

### `void Start()`
Metoda uruchamiana przy starcie komponentu. Inicjalizuje referencje do obiektu gracza i jego komponentu `EntityStatus`, a także konfigurację komponentu `Image` odpowiedzialnego za pasek zdrowia. Zapamiętuje początkową szerokość paska zdrowia, co jest niezbędne do proporcjonalnego skalowania paska na podstawie poziomu zdrowia.

### `void Update()`
Metoda wywoływana co klatkę. Oblicza procentowy poziom zdrowia gracza (`CurrentHealthPercent`) i na jego podstawie aktualizuje szerokość paska zdrowia, aby odzwierciedlać bieżący poziom zdrowia gracza. Jeśli zdrowie spada, pasek zdrowia będzie odpowiednio kurczyć się, zachowując proporcję początkowego rozmiaru.

## Przykłady użycia
1. **Wyświetlanie zdrowia na UI:** Klasa pozwala dynamicznie skalować szerokość paska zdrowia na interfejsie użytkownika, informując gracza o aktualnym stanie zdrowia.
2. **Integracja z mechaniką zdrowia gracza:** Może być używana w grze, gdzie zmniejszenie poziomu zdrowia gracza wizualnie przekłada się na skurczenie paska zdrowia, zapewniając intuicyjną informację zwrotną.

## Uwagi
- Aby klasa działała poprawnie, `healthDisplay` musi być przypisany do obiektu UI zawierającego komponent `Image`, a obiekt gracza musi mieć tag `Player`.
- Klasa `EntityStatus` musi zapewniać metody `GetHp()` oraz `GetMaxHp()`, które zwracają odpowiednio bieżące i maksymalne zdrowie gracza.

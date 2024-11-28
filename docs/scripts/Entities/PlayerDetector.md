[Go back to code contents](../../codeContents.md)

### PlayerDetector

**Lokalizacja:** [`Assets/Code/Scripts/Entities/PlayerDetector.cs`](../../../Assets/Code/Scripts/Entities/PlayerDetector.cs)

Klasa `PlayerDetector` jest odpowiedzialna za wykrywanie obecności gracza w określonym obszarze (zazwyczaj strefie wykrywania przypisanej do wrogiej jednostki). Kiedy gracz znajdzie się w zasięgu, komponent informuje nadrzędny obiekt (`parentEntity`) o wykryciu gracza. Umożliwia to przeciwnikom podjęcie odpowiednich działań, takich jak rozpoczęcie pościgu za graczem.

#### Pole Prywatne

- **parentEntity** (`GameObject`) - odniesienie do nadrzędnego obiektu, którego dotyczy wykrycie gracza. Jest ustawiane na początku działania skryptu i wykorzystywane do komunikacji z komponentem `EntityStatus`.

#### Metody

1. **Start()**
    - Wywoływana podczas inicjalizacji komponentu.
    - Ustawia `parentEntity` jako nadrzędny obiekt (`parent`) bieżącego obiektu, co umożliwia dostęp do jego komponentów.

2. **OnTriggerStay2D(Collider2D collision)**
    - Wywoływana przy każdym kolejnym wejściu obiektu do strefy wykrywania.
    - Sprawdza, czy kolidujący obiekt ma tag `Player`.
    - Jeśli tak, przypisuje obiekt gracza (`collidingObject`) jako cel wykrycia (`detectedTarget`) do komponentu `EntityStatus` nadrzędnego obiektu `parentEntity`.

3. **OnTriggerExit2D(Collider2D other)**
    - Wywoływana, gdy obiekt opuszcza strefę wykrywania.
    - Ustawia `detectedTarget` w `EntityStatus` nadrzędnego obiektu na `null`, co sygnalizuje, że gracz opuścił strefę wykrywania.

---

Ten komponent współpracuje z `EntityStatus`, aby zarządzać stanem wykrywania celu przez jednostkę. Jeśli gracz wejdzie do zasięgu wykrywania, `detectedTarget` jest ustawiane na gracza, co pozwala jednostce na reakcję, np. rozpoczęcie pościgu. Gdy gracz opuści strefę, cel zostaje wyzerowany (`null`), co może sygnalizować zakończenie ścigania.

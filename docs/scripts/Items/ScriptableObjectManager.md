[Go back to code contents](../../codeContents.md)

### ScriptableObjectManager

**Lokalizacja:** [`Assets/Code/Scripts/Managers/ScriptableObjectManager.cs`](../../../Assets/Code/Scripts/Managers/ScriptableObjectManager.cs)

Klasa `ScriptableObjectManager` pełni rolę globalnego menedżera danych przedmiotów w grze. Przechowuje listę obiektów `ItemData` (danych przedmiotów) i udostępnia metodę pozwalającą na ich pobieranie na podstawie unikalnego identyfikatora. Jest implementowana jako singleton, co zapewnia dostęp do tych danych z dowolnej części gry. Niweluje także występowanie błędu polegającego na kopiowaniu się przedmiotów między przejściami scen.

#### Pole Statyczne

- **Instance** (`ScriptableObjectManager`) - Statyczna instancja singletonu, która umożliwia globalny dostęp do menedżera danych przedmiotów.

#### Pola Publiczne

- **itemDataList** (`List<ItemData>`) - Lista obiektów `ItemData`, przechowująca dane wszystkich przedmiotów w grze, takich jak nazwa, ikona, rzadkość itp.

#### Metody

1. **Awake()**
    - Zapewnia, że klasa `ScriptableObjectManager` jest singletonem:
        - Jeśli instancja jeszcze nie istnieje, ustawia `Instance` na aktualny obiekt (`this`) i oznacza go jako niezniszczalny między scenami za pomocą `DontDestroyOnLoad`.
        - Jeśli instancja już istnieje, niszczy obiekt, aby uniknąć duplikatów.

2. **GetItemData(int id)**
    - Pobiera dane przedmiotu na podstawie jego identyfikatora (`id`).
    - Zwraca obiekt `ItemData` z listy `itemDataList` na podstawie indeksu `id`.
    - Używana przez inne komponenty, takie jak `ItemObject`, do uzyskania danych przedmiotu na podstawie przypisanego `ItemId`.

---

Ten menedżer jest kluczowy w mechanice gromadzenia przedmiotów, ponieważ centralizuje dostęp do informacji o przedmiotach. Przykładowo, komponent `ItemObject` używa `ScriptableObjectManager`, aby pobrać odpowiednie dane przedmiotu (`ItemData`) na podstawie identyfikatora `ItemId`, co umożliwia automatyczne przypisanie ikony i efektów wizualnych do każdego przedmiotu w grze.

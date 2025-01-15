# Klasa Wave

Klasa `Wave` znajduje się w [`Assets/Code/Scripts/System/InvasionTrial/Wave.cs`](../../../../Assets/Code/Scripts/System/InvasionTrial/Wave.cs)

Klasa `Wave` odpowiada za obsługę pojedynczej fali przeciwników w grze.

## Pola
- **waveStarted (bool)**: Flaga wskazująca, czy fala została rozpoczęta.
- **waveEnded (bool)**: Flaga wskazująca, czy fala została zakończona.
- **enemies (List<GameObject>)**: Lista przeciwników, którzy mają zostać pojawieni w tej fali.

## Metody

### SpawnEnemies
**Opis**: Rozpoczyna proces pojawiania przeciwników w określonych punktach spawn.

**Parametry**:
- `spawnPoints (List<Transform>)`: Lista punktów spawn, w których przeciwnicy mogą się pojawić.

### Update
**Opis**: Sprawdza, czy wszystkie dzieci obiektu (przeciwnicy) zostały usunięte. Jeśli tak, oznacza falę jako zakończoną.

---

### SpawnEnemiesWithDelay (prywatna)
**Opis**: Pojawia przeciwników z opóźnieniem czasowym między kolejnymi instancjami.

**Parametry**:
- `spawnPoints (List<Transform>)`: Lista punktów spawn.

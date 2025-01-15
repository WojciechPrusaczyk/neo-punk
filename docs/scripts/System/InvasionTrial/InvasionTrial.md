# Klasa InvasionTrial

Klasa `Wave` znajduje się w [`Assets/Code/Scripts/System/InvasionTrial/InvasionTrial.cs`](../../../../Assets/Code/Scripts/System/InvasionTrial/InvasionTrial.cs)

Klasa `InvasionTrial` obsługuje przebieg całego testu inwazji, składającego się z wielu fal przeciwników.

## Pola
- **trialStarted (bool)**: Flaga wskazująca, czy test inwazji został rozpoczęty.
- **trialFinished (bool)**: Flaga wskazująca, czy test inwazji został zakończony.
- **SpawnPoints (List<Transform>)**: Lista punktów spawn dla fal przeciwników.
- **waves (List<Wave>)**: Lista fal przeciwników.
- **currentWave (int)**: Numer aktualnie trwającej fali.
- **durationBetweenWaves (float)**: Czas w sekundach pomiędzy kolejnymi falami.

## Metody

### StartTrial
**Opis**: Rozpoczyna test inwazji, jeśli nie został wcześniej uruchomiony.

### Update
**Opis**: Nasłuchuje wciśnięcia klawisza `O`, aby uruchomić test inwazji.

---

### HandleWaves (prywatna)
**Opis**: Obsługuje kolejne fale przeciwników w teście inwazji.

**Proces**:
1. Dla każdej fali w liście `waves` wywołuje metodę `SpawnEnemies`.
2. Oczekuje na zakończenie obecnej fali (`waveEnded`).
3. Przechodzi do następnej fali po określonym czasie (`durationBetweenWaves`).
4. Po zakończeniu wszystkich fal ustawia `trialFinished` na `true`.

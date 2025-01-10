# Skrypt Timer


**Lokalizacja:** [`Assets/Code/Scripts/System/Timer.cs`](../../../Assets/Code/Scripts/System/Timer.cs)


Klasa `Timer` zarządza funkcjonalnością timera. Obejmuje opcje rozpoczynania od zera, odliczania w górę lub w dół oraz wywoływania UnityEvent po zakończeniu timera.

## Funkcje

- **Konfigurowalne zachowanie timera:**
    - Opcja rozpoczęcia timera od zera lub określonego czasu trwania.
    - Timer może odliczać w górę lub w dół w zależności od konfiguracji.

- **Metody sterowania timerem:**
    - Uruchamianie, pauzowanie, wznawianie i resetowanie timera.

- **Wyzwalanie zdarzenia po zakończeniu:**
    - Wywołuje UnityEvent (`onTimeout`) po zakończeniu timera.

## Użycie

1. **Dodaj skrypt:** Dodaj skrypt `Timer` do obiektu GameObject w swojej scenie Unity.

2. **Skonfiguruj ustawienia timera:**
    - Ustaw flagę `startFromZero`, aby określić, czy timer odlicza w górę czy w dół.
    - Ustaw `duration` (czas trwania) dla timera.

3. **Przypisz akcję po zakończeniu:**
    - Użyj Inspektora Unity, aby przypisać metodę do UnityEvent `onTimeout`.

4. **Steruj timerem:**
    - Użyj udostępnionych metod publicznych (`StartTimer`, `PauseTimer`, `ResumeTimer`, `ResetTimer`) do zarządzania timerem podczas rozgrywki.

## Metody publiczne

- **`StartTimer(float newDuration)`**
    - Uruchamia timer z określonym czasem trwania.

- **`PauseTimer()`**
    - Wstrzymuje timer.

- **`ResumeTimer()`**
    - Wznawia timer, jeśli był wstrzymany.

- **`ResetTimer()`**
    - Resetuje timer do jego początkowego stanu na podstawie konfiguracji.

## Uwagi

- Timer automatycznie ogranicza wartość `currentTime` do określonego zakresu (0 do `duration`), aby uniknąć przekroczenia wartości.
- Upewnij się, że przypisane metody do `onTimeout` obsługują logikę wymaganą po zakończeniu timera.

## Przykładowy scenariusz

Użyj tego timera do tworzenia odliczań dla mechanik rozgrywki, takich jak wzmocnienia, timery odrodzenia lub wyzwalacze zdarzeń w grach Unity.

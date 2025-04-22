[Go back to code contents](../../codeContents.md)

Klasa `WorldSoundFXManager` znajduje się w [`Assets/Code/Scripts/System/WorldSoundFXManager.cs`](../../../Assets/Code/Scripts/System/WorldSoundFXManager.cs) //tbm

Główna klasa odpowiadająca za zarządzanie dźwiękiem w całej grze.
Zawiera pola typu `float` przyjmujące wartości (0 - 1f):
- `masterVolume`
- `sfxVolume`
- `musicVolume`
- `dialogueVolume`

Te pola kontrolowane są w obiekcie `WorldSoundFXManager`, który znajduje się w scenie `MainMenu`.
Ze względu na wprowadzenie `WorldSoundFXManager` konieczne jest uruchomienie gry z poziomu scenu `MainMenu`, ponieważ jest to obiekt instancji statycznej, która jest przenoszona między scenami.

Najważniejsze metody to:
#### void PlaySoundFX()
- Przyjmuje argumenty: `AudioClip[] clip`\*, `Enums.SoundType soundType`, `float pitch`
#### void ChooseRandomSFXFromArray()
- Przyjmuje argumenty: `AudioClip[] clip`\*, `Enums.SoundType soundType`, `float pitch`

Przydatna informacja [Enums](Enums.md)

Przykładowe wykorzystanie metody `ChooseRandomSFXFromArray`:
```
if (WorldSoundFXManager.instance == null)
  return;

float randomPitch = Random.Range(0.8f, 1.2f);
WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.ShDogAttackSFX, Enums.SoundType.SFX, randomPitch);
```

### Pozostałe informacje

Pozostałe pola w klasie `WorldSoundFXManager` mogą być dowolnie dodawane w zależności od potrzeb, podobnie do aktualnej struktury.
Każde odwołanie w kodzie do WorldSoundFXManager należy wykonywać poprzez użycie skrótu: `WorldSoundFXManager.instance.<>`, gdzie `<>` jest konkretną publiczną metodą/zmienną.

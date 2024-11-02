[Go back to contents](../contents.md)

Klasa TooltipsController znajduje się w [Assets/Scripts/System/Tooltips/TooltipsController.cs](../../Assets/Scripts/System/Tooltips/TooltipsController.cs)

Zawiera ona dodatkową klasę Tooltip, służącą do serializacji danych podpowiedzi.

Sam kontroler powinien znajdować się w: "Scene/UserInterface/Tooltips",  
będącym obiektem typu Canvas z zawartym przygotowanym UI  
do wyświetlania serializowanych danych podpowiedzi.

Klasa zawiera listę, którą należy przygotować pod kontem wyświetlanych danych.  
Gdy tworzymy nową podpowiedź należy przygotować opis i zdjęcie.  
W tekście można używać także obrazki przycisków za pomocą "<sprite=numer>",  
gdzie numer to index obrazka znajdującego się w pliku [Assets/Sprites/UI/Keycaps.asset](../../Assets/Sprites/UI/Keycaps.asset)

Przykładowy opis:
```
You can move sideways by using <sprite=14>  
and <sprite=17>, and jump using <sprite=36> or <sprite=116>.  
Some of the surroundings can be passed through by clicking  
<sprite=32> while standing on top of them.
```

Gdy dodamy już nasz sprite, oraz opis, to należy go wywołać za pomocą metody:  
*public void ShowTooltip(int tooltipNumber)*

Gdzie *tooltipNumber* to index naszej podowiedzi.

Wszystkie obrazki, oraz skrypty wywołujace podpowiedzi powinny znajdować się w folderze [Assets/Scripts/System/Tooltips](../../Assets/Scripts/System/Tooltips)  
Całą podpowiedź umieszczamy w tym folderze w następującej hierarchii:
- nazwaPodpowiedzi
    - image.png -> obrazek dot. podpowiedzi
    - tipNazwaPodpowiedzi.cs -> skrypt wywołujący podpowiedź
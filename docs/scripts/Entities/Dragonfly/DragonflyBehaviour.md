[Go back to code contents](../../../codeContents.md)

### DragonflyBehavior

**Lokalizacja:** [`Assets/Code/Scripts/Entities/Dragonfly/DragonflyBehaviour.cs`](../../../../Assets/Code/Scripts/Entities/Dragonfly/DragonflyBehaviour.cs)

Klasa `DragonflyBehavior` zarządza zachowaniem ważki w grze, w tym jej ruchem, wykrywaniem gracza, atakami dystansowymi i bezpośrednimi, oraz reakcjami na przeszkody. Klasa ta korzysta z komponentów Unity takich jak `Transform`, `Rigidbody2D`, oraz `Collider2D` do realizacji swoich funkcji.

#### Pola Publiczne

- `Transform[] Positions` - tablica pozycji, pomiędzy którymi przemieszcza się ważka podczas patrolu.
- `float currentSpeed` - aktualna prędkość jednostki, obliczana na podstawie różnicy pozycji w czasie.
- `float distanceToPlayer` - odległość między ważką a graczem, używana do oceny zasięgu ataku.
- `LayerMask warstwaPrzeszkod` - maska kolizji dla przeszkód używana przy raycastingu w kierunku gracza.
- `Transform shootingPoint` - punkt wystrzału pocisków, na ogół przypisany do elementu Unity.
- `GameObject projectilePrefab` - prefabrykat pocisku, który ważka wystrzeliwuje.
- `float bulletSpeed` - prędkość pocisku wystrzeliwanego przez ważkę.

#### Pola Prywatne

- `float EntitySpeed` - prędkość poruszania się jednostki pobierana z `EntityStatus`.
- `int NextPositionIndex` - indeks kolejnej pozycji, do której zmierza ważka w patrolu.
- `Transform NextPosition` - kolejna pozycja z tablicy `Positions`, do której ważka zmierza.
- `EntityStatus entityStatus` - status jednostki zawierający informacje o zdrowiu, obrażeniach i kierunku.
- `Vector3 playerVector3` - pozycja gracza, używana do nawigacji i ataków.
- `bool isChasingPlayer` - wskaźnik, czy ważka podąża za graczem.
- `Vector2 previousPlayerDetectorRange` - poprzedni rozmiar detektora gracza, stosowany do zmiany rozmiaru po wykryciu gracza.
- `CapsuleCollider2D playerDetector` - detektor kolizji wykrywający obecność gracza w zasięgu ważki.
- `Vector3 previousPosition` - poprzednia pozycja ważki używana do obliczeń prędkości.
- `bool didRaycastFoundPlayer` - wskaźnik, czy raycast wykrył gracza bez przeszkód na linii widzenia.
- `bool isAttacking` - wskaźnik, czy ważka aktualnie wykonuje atak.
- `bool canAttack` - wskaźnik, czy ważka może wykonać atak kontaktowy.
- `bool isPlayerInAttackRange` - wskaźnik, czy gracz znajduje się w zasięgu ataku ważki.
- `GameObject collidingPlayer` - obiekt kolidujący reprezentujący gracza, na którym wykonywany jest atak.

#### Metody Publiczne

- `void Start()` - inicjalizuje prędkość jednostki oraz rozmiar detektora gracza. Przypisuje pierwszą pozycję patrolową.
- `void OnCollisionEnter2D(Collision2D collision)` - wywoływana przy zderzeniu z obiektem; zadaje obrażenia graczowi, jeśli wykryje obiekt gracza.
- `void OnCollisionStay2D(Collision2D collision)` - kontynuuje zadawanie obrażeń graczowi, jeśli nadal jest w zasięgu i może atakować.
- `void Update()` - zarządza logiką ruchu, kierunku, ataków oraz wykrywania gracza poprzez raycast.
- `void PerformShoot()` - tworzy pocisk, ustawia jego prędkość oraz kierunek i obraca pocisk, aby wskazywał kierunek ruchu.
- `IEnumerator PerformAttack()` - wywołuje atak dystansowy (strzał) co określony czas, jeśli gracz znajduje się w zasięgu.
- `void Move()` - zarządza ruchem ważki zarówno w trybie patrolowym, jak i podczas pościgu za graczem, oraz modyfikuje zasięg wykrywania po wykryciu gracza lub otrzymaniu obrażeń.

#### Opis Działania

- Klasa umożliwia ważce patrolowanie określonych punktów, a także reagowanie na obecność gracza, zmieniając tryb na pościg oraz zwiększając zasięg wykrywania.
- Jeśli ważka wykryje gracza i jest w zasięgu, może wykonać atak dystansowy przy użyciu pocisku, a przy kolizji z graczem zadaje obrażenia bezpośrednie.
- Ważka wykorzystuje raycast do oceny, czy gracz znajduje się na linii widzenia, co pozwala na bardziej realistyczne zachowanie w przypadku przeszkód.
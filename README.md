Лабораторная работа 3. Прототип Tower Defense.
Колобовников 461649

**Прошу прощения что так поздно сдаю проект. Я занимался им все эти 2 недели. Увлекся и ушел в разработку "Quality of life" аспекты. В следующие разы буду уделять время непосредственно заданию. 
Надеюсь игра будет хотя бы ощущаться приятно. {{{(>_<)}}}

Расширенный прототип Tower Defense из Практики 3, с игроком, ядром базы и различными башнями и врагами. Если ядро будет унечтожено - игра закончится.
UML-диаграмма - PDF в файле с проектом: Лабораторная 3 Колобовников 461649.drawio

Туториал:
- WASD - перемещение
- ЛКМ - стрельба 
- ПКМ - перещение камеры
- колесико - приближение/отдаление камеры
- Клавиша "Е" (англ) - строительство ВКЛ/ВЫКЛ
	В режиме строительства:
	- крутить колесико - переключать башни
	- ЛКМ - поставить башню

В проекте есть следующие сцены:
- Player - игрок. Перемещается по WASD, двигает камеру, стреляет через ShootComponent, имеет здоровье - HealthComponent. Может строить - выбирать тип башни колесом мыши и устанавливать её на карту.
Core - ядро. Имеет HealthComponent, не двигается и не стреляет. При уничтожении ядра GameManager показывает интерфейс конца игры и убивает игрока.
- Cannon - обычная башня. HealthComponent, VisibilityComponent и ShootComponent. Стреляет пулями с DamageComponent; вращается если рядом нет врагов.
- ElectroTower - электрическая башня. HealthComponent, VisibilityComponent и ShootComponent и AnimationPlayer. Стреляет пулями с ElectroDamageComponent; При попадании во врага пуля разлетается кольцом попадая во всех врагов рядом.
- ElectroTower - башня-пресс. HealthComponent, VisibilityComponent и ShootComponent и AnimationPlayer. Долго стреляет но отталкивает врагов.
- EnemyTower - вражеская башня. Имеет HealthComponent, MoveComponent, VisibilityComponent и ShootComponent. Двигается к центру мира, стреляет пулями.
- Crawler - ползающий нестреляющий враг. Имеет HealthComponent, MoveComponent, VisibilityComponent и DamageComponent для нанесения урона при контакте
- Bullet - пуля. MoveComponent и DamageComponent. Создаётся через ShootComponent.SpawnBullet(), получает направление, скорость, урон при выстреле.
- ElectroBullet - электрическая пуля. MoveComponent и ElectroDamageComponent, animationPlayer. Через анимацию появляется кольцо которое увеличивается и также увеличивает коллайдеры аттаки.
- PushingBullet - отталкивающая пуля. MoveComponent и PushingDamageComponent. Воздействует на movingComponent врага в которого попадает отталкивая его
- GhostTower (Cannon, PoundingTower, ElectroTower) - призрачная модель башни для предварительного размещения. Не имеет полноценных коллизий, используется для визуального выбора места постройки. Содержит CrossingObjComponent, который проверяет пересечения с другими CharacterBody3D.
DamageVisibility - отображение здоровья. Node3D с Label3D и AnimationPlayer. Закреплен за башнями, поворачивается по камере, масштабируется от высоты камеры и появляется при получении врагом урона.

- CrossingObjComponent - зона проверки строительства. Area3D. Отслеживает вход и выход CharacterBody3D, меняет цвет материала строительства на красный при запрете и на зелёный при разрешении, хранит флаг buildingIsAllowed и счётчик пересекающихся тел.

В проекте есть следующие C# файлы:
- Core - задаёт поведение ядра. Получает GearMesh и просто вращает его в _Process
- CrossingObjComponent - Area3D-компонент возможности строительства. Воздействует на материал строительства и на флаг buildingIsAllowed, считает количество пересекающихся тел и если оно равно 0 то можно строить. В OnBodyEntered запрещает постройку, добавляет счетчик врагов и красит материал в красный, в OnBodyExited вычитает врагов и разрешает постройку, когда пересечений больше нет
- DamageComponent - компонент нанесения урона. Наследуется от Area3D. Содержит DamageAmount, таймер повторной проверки коллизий, destroyable, методы OnBulletBodyEntered, SetDamage, SetTargetLayer, ApplyDamage. Наносит урон воздействуя на HealthComponent цели
- ElectroDamageComponent - наследник DamageComponent. При первом попадании наносит урон и запускает анимацию ElectroRing, при последующих попаданиях наносит урон через ElectroRing()
- PushingDamageComponent - наследник DamageComponent. Наносит урон и отталкивает цель через MoveComponent.AddVector в направлении движения пули. Использует _bulletMoveComponent и PushDistance.
- DamageVisibility - отображение здоровья. Получает Label3D, AnimationPlayer и GameManager, поворачивает надпись по камере, масштабирует её и запускает анимацию TakingDamage
- MoveComponent - компонент движения. Node. Хранит скорость, направление, добавочный вектор и радиус остановки. В _Process применяет Velocity и MoveAndSlide. Содержит методы SetDirection, AddVector, GetDirection, SetSpeed, GetSpeed
- DashComponent - наследник MoveComponent. Добавляет таймер рывков, множитель дальности dashLength и ссылку на VisibilityComponent. В _Process применяет скорость и MoveAndSlide, а по таймеру выполняет рывок и поворачивает родителя к цели
- GameManager - AutoLoad скрипт. Управляет камерой, спавнит врагов из _enemyList, вычисляет позицию мыши в 3D мире через raycast, "возвращает"(return) поворот и уровень камеры, проверяет наличие игрока и ядра, показывает CanvasLayer и завершает игру при уничтожении ядра, смерти игрока
- HealthComponent - компонент здоровья. Node. Хранит MaxHealth и CurrentHealth, получает родителя, вызывает DamageVisibility.DamageAnimation, при нулевом здоровье удаляет родительский узел через QueueFree.
- Player - задаёт управление игроком. CharacterBody3D. Обрабатывает WASD-движение, поворот к мыши, стрельбу, режим строительства, выбор башни колесом мыши, создание призрачной башни и установку реальной башни при разрешении CrossingObjComponent.
- ShootComponent - компонент стрельбы. Node3D. Хранит сцену пули, маркер появления, анимацию, таймер, урон, скорость и слои целей. Создаёт пулю, задаёт ей направление, скорость, урон и маску коллизий. В _Process наводится на цель либо вращается по кругу.
- VisibilityComponent - компонент обнаружения целей. Area3D. Отслеживает тела через BodyEntered и BodyExited, игнорирует объекты своей группы, проверяет наличие HealthComponent, хранит список обнаруженных тел и находит ближайшую цель. Передаёт цель в ShootComponent и направление в MoveComponent.
# GodotTowerDefence_Laboratory3

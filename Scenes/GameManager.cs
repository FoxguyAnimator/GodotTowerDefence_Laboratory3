using Godot;
using System;

public partial class GameManager : Node
{
	// Позиция мыши в предыдущем кадре.
	private Vector2 PreviousFrameMousePos;

	// Радиус появления врагов вокруг центра карты.
    private float spawnEnemyRadius = 40f;

	// Интервал появления врагов в секундах.
    private float spawnEnemyTimeInSeconds = 5f;

	// Список сцен врагов, доступных для создания.
    private Godot.Collections.Array<PackedScene> _enemyList = new Godot.Collections.Array<PackedScene>();

	// Таймер автоматического появления врагов.
    private Timer EnemySpawnTimer = new Timer();

	// Загружает сцены врагов, настраивает таймер и скрывает интерфейс.
	public override void _Ready()
    {
        _enemyList.Add(GD.Load<PackedScene>("res://Scenes/Towers/EnemyTowers/EnemyTower.tscn"));
        _enemyList.Add(GD.Load<PackedScene>("res://Scenes/Towers/EnemyTowers/Crawler.tscn"));

        EnemySpawnTimer.WaitTime = spawnEnemyTimeInSeconds;
		EnemySpawnTimer.OneShot = false;
		AddChild(EnemySpawnTimer);
		EnemySpawnTimer.Timeout += () => SpawnEnemies();
        EnemySpawnTimer.Start();

        Node currentScene = GetTree().CurrentScene;
        CanvasLayer CanvasLayer = currentScene.GetNode<CanvasLayer>("CanvasLayer");
        CanvasLayer.Visible = false;
    }

	// Обрабатывает управление камерой и проверяет состояние игрока и ядра.
	public override void _Process(double delta)
	{
        Node currentScene = GetTree().CurrentScene;
        Camera3D camera3D = currentScene.GetNode<Camera3D>("Camera3D");
        Player player = currentScene.GetNodeOrNull<Player>("Player");
        CharacterBody3D core = currentScene.GetNodeOrNull<CharacterBody3D>("Core");
        if (player != null && core != null)
        {
            if (!player.isBuilding)   // если игрок строит то камеру не двигать
            {
                if (Input.IsActionJustPressed("wheel_up"))
                camera3D.TranslateObjectLocal(Vector3.Forward);
                if (Input.IsActionJustPressed("wheel_down"))
                    camera3D.TranslateObjectLocal(Vector3.Back);
                if (Input.IsActionPressed("RMB")) 
                {
                    Vector2 MousePosDifference = GetViewport().GetMousePosition() - PreviousFrameMousePos; // расчитываем как сдвинулась мышка и от этого двигаем камеру
                    camera3D.TranslateObjectLocal(new Vector3(-MousePosDifference.X, MousePosDifference.Y, 0.0f)*0.05f); // двигаем камеру в локальных координатах
                }
            }
        } 
        else
        {
            CanvasLayer CanvasLayer = currentScene.GetNode<CanvasLayer>("CanvasLayer");
            CanvasLayer.Visible = true;
            HealthComponent healthComponent = player?.GetNodeOrNull<HealthComponent>("HealthComponent");
            healthComponent?.TakeDamage(200); // если ядро уничтожено убивает игрока
        }

        PreviousFrameMousePos = GetViewport().GetMousePosition();
	}

	// Возвращает позицию курсора в 3д сцене с помощью raycast
	public Vector3 GetMouse3DPosition(uint collisionMask = 1)
    {
		Node currentScene = GetTree().CurrentScene;
		Camera3D camera = currentScene.GetNodeOrNull<Camera3D>("Camera3D");

        //GD.Print(camera);
        if (camera == null)
        {
            GD.PrintErr("GameManager: Передана пустая ссылка на камеру!");
            return Vector3.Zero;
        }

        Vector2 mousePos = GetViewport().GetMousePosition(); 

        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePos); //точка начала рейкаста
        Vector3 rayEnd = rayOrigin + camera.ProjectRayNormal(mousePos) * 1000.0f; //точка конца рейкаста

        var spaceState = GetViewport().GetWorld3D().DirectSpaceState;

        var query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
        query.CollisionMask = collisionMask; 

        var result = spaceState.IntersectRay(query);

        if (result.Count > 0 && result.ContainsKey("position"))
        {
            return (Vector3)result["position"];
        }

        // Если луч никуда не попал (например, улетел в небо), возвращаем нулевой вектор
        return Vector3.Zero;
    }

	// Возвращает текущий поворот камеры.
    public Vector3 GetCameraRotation()
    {
        Node currentScene = GetTree().CurrentScene;
        Camera3D camera3D = currentScene.GetNode<Camera3D>("Camera3D");
        return camera3D.Rotation;
    }

	// Возвращает Y камеры (для размера надписей ХП)
    public float GetCameraScrollLevel()
    {
        Node currentScene = GetTree().CurrentScene;
        Camera3D camera3D = currentScene.GetNode<Camera3D>("Camera3D");
        return camera3D.Position.Y;
    }

	// Создаёт случайного врага и размещает его на некотором радиусе от центра мира
    private void SpawnEnemies()
    {
        //GD.Print(_enemyList);
        Vector3 randomSpawnPos = Vector3.Zero;
        randomSpawnPos.X = Vector2.FromAngle((float)GD.RandRange(0.0, Mathf.Tau)).X;
        randomSpawnPos.Z = Vector2.FromAngle((float)GD.RandRange(0.0, Mathf.Tau)).Y;
        int randomEnemyIndex = GD.RandRange(0, _enemyList.Count - 1);
        Node3D enemy = _enemyList[randomEnemyIndex].Instantiate<Node3D>();
		GetTree().CurrentScene.AddChild(enemy);
        randomSpawnPos = randomSpawnPos.Normalized();
		enemy.GlobalPosition = randomSpawnPos*spawnEnemyRadius;
    }
}
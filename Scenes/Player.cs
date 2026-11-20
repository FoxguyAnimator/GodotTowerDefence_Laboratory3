using Godot;
using System;

public partial class Player : CharacterBody3D
{
	// Скорость игрока
	[Export]
	public float _PlayerSpeed = 1.2f;

	// Список сцен-призраков башен для предварительного размещения.
	[Export]
	private Godot.Collections.Array<PackedScene> GhostTowers = new Godot.Collections.Array<PackedScene>();

	// Список доступных для строительства башен
	[Export]
	private Godot.Collections.Array<PackedScene> Towers = new Godot.Collections.Array<PackedScene>();

	private GameManager _gameManager;

	private ShootComponent shootComponent;

	// находится ли игрок в режиме строительства
	public bool isBuilding;

	// Индекс выбранной для строительства башни
	private int _buildingTowerIndex = 0;

	// размещаемая башня
	private Node3D _buildingTower;

	private Node3D parentScene;

	// ссылки на объекты.
    public override void _Ready()
    {
        base._Ready();
		_gameManager = GetNode<GameManager>("/root/GameManager");
		parentScene = GetParentNode3D();
		shootComponent = GetNode<ShootComponent>("ShootComponent");
    }

	// Обрабатывает движение, поворот, стрельбу и строительство башен.
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity*0.9f;

		if (Input.IsActionPressed("left")) 
		{
			velocity.X -= _PlayerSpeed;
		}
		if (Input.IsActionPressed("right")) 
		{
			velocity.X += _PlayerSpeed;
		}
		if (Input.IsActionPressed("up")) 
		{
			velocity.Z -= _PlayerSpeed;
		}
		if (Input.IsActionPressed("down")) 
		{
			velocity.Z += _PlayerSpeed;
		}

		Velocity = velocity;
		MoveAndSlide();

		Vector3 targetPosition = _gameManager.GetMouse3DPosition();
		targetPosition.Y = Position.Y;

		LookAt(targetPosition);

		if (Input.IsActionJustPressed("shoot") && (!isBuilding))
		{
			Vector3 shootingDirection = targetPosition-GlobalPosition;
			shootingDirection.Y = 0;
			shootComponent.SetDirection(shootingDirection);
			shootComponent.SpawnBullet();
		}

		if (isBuilding)
		{
			if (Input.IsActionJustPressed("wheel_up"))   {_buildingTowerIndex += 1; building();}			
			if (Input.IsActionJustPressed("wheel_down")) {_buildingTowerIndex -= 1; building();}	
			_buildingTower.Position = targetPosition;	
			if (Input.IsActionJustPressed("shoot"))
			{
				CrossingObjComponent cross = _buildingTower.GetNode<CrossingObjComponent>("CrossingObjComponent"); // не пересекается ли строящаяся башня с чем либо
				if (cross.buildingIsAllowed)
				{
					_buildingTower.QueueFree();
					_buildingTower = (Node3D)Towers[_buildingTowerIndex].Instantiate(); 
					AddSibling(_buildingTower);
					_buildingTower.Position = targetPosition;
					_buildingTower = null;
					isBuilding = false;
				}
			}
		}
	}

	public override void _Input(InputEvent @event) // инпут игрока
	{

		if (@event.IsActionPressed("build"))
		{
			if (isBuilding == false)
			{
				isBuilding = true;
				building();
			} 
			else
			{
				isBuilding = false;
				_buildingTower.QueueFree();
				_buildingTower = null;
			}

		}
	}

	// Создаёт модель-призрак выбранной башни
	private void building()
	{
		_buildingTower?.QueueFree();
		if (_buildingTowerIndex >= GhostTowers.Count) _buildingTowerIndex = 0;
		if (_buildingTowerIndex < 0) _buildingTowerIndex = GhostTowers.Count-1;
		//GD.Print(_buildingTowerIndex);
		_buildingTower = (Node3D)GhostTowers[_buildingTowerIndex].Instantiate(); 
		AddSibling(_buildingTower);
	}
}
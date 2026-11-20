using Godot;
using System;
using System.Runtime;

[GlobalClass]
public partial class ShootComponent : Node3D
{
	// Ссылка на родительский объект.
	private Node3D parentNode;

	// Сцена пули, которая будет создаваться при выстреле.
	[Export]
	private PackedScene BulletScene;

	// Маркер, определяющий точку появления пули.
	[Export]
	private Marker3D marker;

	// Анимация выстрела.
	[Export]
	private AnimationPlayer ShootAnimation;

	// Интервал между выстрелами в секундах.
	[Export]
	private float ShootTimerInSeconds = 0.5f;

	// Урон, передаваемый пуле.
	[Export]
	private float bulletDamage = 10f;

	// Скорость полёта пули.
	[Export]
	private float bulletSpeed = 10f;

	// Слои объектов, с которыми может взаимодействовать пуля.
	[Export]
	public Godot.Collections.Array<int> TargetGroups = new Godot.Collections.Array<int>();

	// Текущая цель, по которой стреляем.
	private Node3D Target;

	// Направление выстрела.
	private Vector3 ShootDirection;

	// частота стрельбы
	private Timer shootTimer = new Timer();

	// Настраивает таймер стрельбы и получает родительский объект.
	public override void _Ready()
	{
		parentNode = GetParent<Node3D>();

		shootTimer.WaitTime = ShootTimerInSeconds;
		shootTimer.OneShot = false;
		AddChild(shootTimer);
		shootTimer.Timeout += () => TryShoot();
	}

	// Проверяет наличие сцены пули и производит выстрел + анимация
	private void TryShoot()
	{
		if (BulletScene != null)
		{
			ShootAnimation?.Play("Shooting");
			SpawnBullet();
		}
	}

	// Устанавливает направление выстрела
	public void SetDirection(Vector3 InputShootDirection)
	{
		ShootDirection = InputShootDirection;
	}

	// Создаёт пулю, добавляет её в сцену и задаёт параметры
	public void SpawnBullet()
	{
		Node3D bullet = BulletScene.Instantiate<Node3D>();
		MoveComponent moveComponent = bullet.GetNode<MoveComponent>("MoveComponent");
		DamageComponent damageComponent = bullet.GetNode<DamageComponent>("DamageComponent");
		GetTree().CurrentScene.AddChild(bullet);
		moveComponent.SetDirection(ShootDirection.Normalized());
		moveComponent.SetSpeed(bulletSpeed);
		bullet.GlobalPosition = marker.GlobalPosition; //спавнит ее в точке расположения маркера
		damageComponent.SetDamage(bulletDamage);
		damageComponent.SetTargetLayer(TargetGroups);
	}

	// Устанавливает цель для наведения.
	public void SetTarget(Node3D target)
	{
		Target = target;
	}

	// Запускает таймер стрельбы.
	private void TurnOnCanon()
	{
		if (shootTimer.IsStopped()) shootTimer.Start();
	}

	// Останавливает таймер стрельбы.
	private void TurnOffCanon()
	{
		if (!shootTimer.IsStopped()) shootTimer.Stop();
	}

	// Каждый кадр поворачивает пушку к цели либо вращает по кругу.
	public override void _Process(double delta)
	{

		if (Target != null && GodotObject.IsInstanceValid(Target)) /// тут тоже 
		{
			Vector3 TrackerToTarget = Target.GlobalPosition;
			TrackerToTarget.Y = GlobalPosition.Y;
			LookAt(TrackerToTarget);
			Vector3 positionDifference;
			positionDifference = (Target.GlobalPosition - marker.GlobalPosition);
			positionDifference.Y = 0;
			ShootDirection = positionDifference.Normalized();
			//GD.Print(TrackerToTarget);
			TurnOnCanon();
		}
		else
		{
			TurnOffCanon();
			Vector3 newRotation = Rotation;
			newRotation.Y += (float)delta * 0.5f;
			Rotation = newRotation;
		}
	}
}
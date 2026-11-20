using Godot;
using System;
using System.Runtime.Serialization;

[GlobalClass]
public partial class VisibilityComponent : Area3D
{
	// Нода родителя.
	private Node3D parentNode;

	// Текущая ближайшая обнаруженная цель.
	private Node3D Target;

	// Компонент стрельбы.
	[Export]
	protected ShootComponent shootComponent;

	// Компонент движения.
	[Export]
	public MoveComponent MoveComponent;

	// Список обнаруженных врагов в зоне видимости.
	private Godot.Collections.Array<Node3D> detectBodies = new Godot.Collections.Array<Node3D>();

	// Задает события входа/выхода тел и проверяет список уже находящихся внутри объектов.
	public override void _Ready()
	{
		parentNode = GetParent<Node3D>();
		if (this.GetNodeOrNull<CollisionShape3D>("CollisionShape3D") != null)
		{
			this.BodyEntered += (Node3D body) => OnBodyEntered(body);
			this.BodyExited += (Node3D body) => OnBodyExited(body);
			Godot.Collections.Array<Node3D> overlappingBodies = GetOverlappingBodies();
			for (int i = 0; i < overlappingBodies.Count; i++)
		 		OnBodyEntered(overlappingBodies[i]);
		}
		// if (DetectShape != null)
		// {
		// 	Godot.Collections.Array<Node3D> overlappingBodies = GetOverlappingBodies();
		// 	for (int i = 0; i < overlappingBodies.Count; i++)
		// 		OnBodyEntered(overlappingBodies[i]);
		// }
	}

	// Добавляет тело в список целей, если оно не из своей группы и имеет HealthComponent.
	public void OnBodyEntered(Node3D body)
	{
		bool islnSameGroup = false;
		if ((body.IsInGroup("Enemy") && parentNode.IsInGroup("Enemy"))
			||
			(body.IsInGroup("Tower") && parentNode.IsInGroup("Tower"))
			)
			islnSameGroup = true;
		if (!islnSameGroup)
		{
			HealthComponent h = body.GetNodeOrNull<HealthComponent>("HealthComponent");
			if (h != null) 
				detectBodies.Add(body);
		}
		// Godot.Collections.Array<Node> children = body.GetChildren();
		// for (int i = 0; i < children.Count; i++)
		// {
		// 	if (children[i] is HealthComponent)
		// 	{
		// 		detectBodies.Add(body);
		// 	}
		// }
	}

	// Удаляет тело из списка целей при выходе из зоны.
	public void OnBodyExited(Node3D body)
	{
		detectBodies.Remove(body);
	}

	// Возвращает список обнаруженных объектов.
	public Godot.Collections.Array<Node3D> GetDetectedBodies()
	{
		return detectBodies;
	}

	// Возвращает позицию текущей цели.
	public Vector3 GetTargetPosition()
	{
		if (Target == null) return Vector3.Zero;
		return Target.GlobalPosition;
	}

	// Каждый кадр находит ближайшую цель и передаёт её в shootComponent и MoveComponent
	public override void _Process(double delta)
	{
		Vector3 MovingDirection;
		ClosestEnemy();
		if (Target == null)
		{
			MovingDirection = (Vector3.Zero - parentNode.GlobalPosition);
		}
		else MovingDirection = (Target.GlobalPosition - parentNode.GlobalPosition);
		MovingDirection.Y = 0.0f;

		shootComponent?.SetTarget(Target);
		MoveComponent?.SetDirection(MovingDirection);
	}

	// возвращает ближайший объект по индексу
	public Node3D ClosestEnemy(int index = 0)
	{
		if (detectBodies == null || detectBodies.Count == 0)
		{
			Target = null;
			return null;
		}

		if (index < 0 || index >= detectBodies.Count)
			return null;

		System.Collections.Generic.List<Node3D> sortedBodies = new System.Collections.Generic.List<Node3D>(detectBodies);
		sortedBodies.Sort((a, b) =>
			(GlobalPosition - a.GlobalPosition).LengthSquared()
			.CompareTo((GlobalPosition - b.GlobalPosition).LengthSquared()));

		Target = sortedBodies[0];
		return sortedBodies[index];
	}
}
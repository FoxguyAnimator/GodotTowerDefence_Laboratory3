using Godot;
using System;
using System.Runtime;

[GlobalClass]
public partial class MoveComponent : Node
{
	// нода родителя
	protected CharacterBody3D parentNode;

	// Скорость передвижения
	[Export]
	protected float Speed = 50f;

	// Цель, к которой движется родитель
	protected Vector3 MovingVector;

	// Дополнительный вектор движения.
	protected Vector3 AdditionVector;

	// Радиус, при достижении которого движение останавливается
	[Export]
	protected float StopRadius = 0.5f;

	// Устанавливает родительский объект
	public override void _Ready()
	{
		if (GetParent() is CharacterBody3D parentNode) this.parentNode = parentNode;
	}

	// Каждый кадр вызывает движение, если родитель корректный
	public override void _Process(double delta)
	{
		AdditionVector *= 0.9f;
		if (parentNode == null)
		{
			GD.Print("MoveComp is not attachet to body");
		}
		else Move();
	}

	// Устанавливает направление движения
	public void SetDirection(Vector3 direction)
	{
		MovingVector = direction;
	}

	// Устанавливает добавочный вектор
	public void AddVector(Vector3 InputAdditionVector)
	{
		AdditionVector = InputAdditionVector;
	}

	// Возвращает текущее направление движения
	public Vector3 GetDirection()
	{
		return MovingVector;
	}

	// Устанавливает скорость 
	public void SetSpeed(float speed)
	{
		Speed = speed;
	}

	// Возвращает текущую скорость 
	public float GetSpeed()
	{
		return Speed;
	}

	// двигает объект к цели (или к началу координат если цель не задана)
	protected virtual void Move()
	{
		if (MovingVector.Length() > StopRadius)
		{
			MovingVector = MovingVector.Normalized();
		}
		else
		{
			MovingVector = Vector3.Zero;
		}
		parentNode.Velocity = MovingVector * Speed;
		parentNode.Velocity += AdditionVector; // добавочный вектор
		parentNode.MoveAndSlide();
	}
}
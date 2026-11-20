using Godot;
using System;

[GlobalClass]
public partial class DashComponent : MoveComponent
{
    // таймер рывков
    private Timer dashTimer = new Timer();

    // интервал между рывками в секундах
    [Export]
	private float dashTimerInSeconds = 1f;

	// множитель дальности рывка
	[Export]
	private float dashLength = 15f;

	// VisibilityComponent для поворота объекта по направлению 
	[Export]
	private VisibilityComponent visibilityComponent;

    public override void _Ready() // родительская нода + таймер
    {
        if (GetParent() is CharacterBody3D parentNode) this.parentNode = parentNode;

		dashTimer.WaitTime = dashTimerInSeconds;
		dashTimer.OneShot = false;
		AddChild(dashTimer);
		dashTimer.Timeout += () => Dash();
		dashTimer.Start();
    }

    // обновляет движение персонажа и применяет скорость
    public override void _Process(double delta)
    {
		parentNode.Velocity *= 0.9f;
		AdditionVector *= 0.8f;
		if (parentNode == null)
		{
			GD.Print("MoveComp is not attachet to body");
		}
		else
		{
			parentNode.Velocity += AdditionVector;
			parentNode.MoveAndSlide();
		}
    }

    // Рывок в направлении + смотрит в сторону врага
    private void Dash()
    {
        if (AdditionVector.Length() < StopRadius)
		{
			MovingVector = MovingVector.Normalized();
		}
		else
		{
			MovingVector = Vector3.Zero;
		}
		parentNode.Velocity = MovingVector * Speed * dashLength;
		Vector3 LookingTo = visibilityComponent.GetTargetPosition();
		LookingTo.Y = parentNode.GlobalPosition.Y;
		parentNode.LookAt(LookingTo);
    }
}
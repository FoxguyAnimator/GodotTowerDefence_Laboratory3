using Godot;
using System;

[GlobalClass]
public partial class DamageComponent : Area3D
{
	// родительская нода
	private Node3D parentNode;

	// колво урона.
	[Export]
	protected float DamageAmount = 10f;

	// Время проверки коллизии.
	[Export]
	private float checkCollisionTimerInSeconds = 1f;

	// уничтожается ли объект после нанесения урона
	[Export]
	protected bool destroyable = true;

	// таймер проверки столкновений
	protected Timer checkCollisionTimer = new Timer();

    public override void _Ready() // родительская нода + таймер
    {
        base._Ready();
		parentNode = GetParent<Node3D>();

		checkCollisionTimer.WaitTime = checkCollisionTimerInSeconds;
		checkCollisionTimer.OneShot = false;
		AddChild(checkCollisionTimer);
		checkCollisionTimer.Timeout += () => checkCollision(); // запускает проверку коллизии каждые N секунд
		checkCollisionTimer.Start();
    }

    // наносит урон пересеченному объекту
	public virtual void OnBulletBodyEntered(Node3D body)
	{
		//GD.Print("bullet collided with " + body.Name + body.GetType());
		if (body is CharacterBody3D character)
		{
		    ApplyDamage(character);
		}
	}

	// установить колво урона
	public void SetDamage(float damage)
	{
		DamageAmount = damage;
	}

	// установить слои коллизии для Area
	public void SetTargetLayer(Godot.Collections.Array<int> InputTargetGroups)
	{
		parentNode = GetParent<Node3D>();
		foreach(int c in InputTargetGroups)
		{
			SetCollisionMaskValue(c, true);
		}
	}

	// Проверяет коллизию с объектами и наносит им урон (если враг продолжает находистся вплотную к башне)
	private void checkCollision()
	{
		foreach (Node3D body in GetOverlappingBodies())
		{
			if (body is CharacterBody3D character)
				ApplyDamage(character);
		}
	}

	// Наносит урон цели + уничтожает объект если destroyable
	public virtual void ApplyDamage(CharacterBody3D target)
    {
		if (target != null)
		{
			HealthComponent health_comp = target.GetNodeOrNull<HealthComponent>("HealthComponent");
       		health_comp?.TakeDamage(DamageAmount);
		}
		if (destroyable == true) parentNode.QueueFree(); 
    }
}
using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : Node
{
	// Ссылка на родительский узел (владельца здоровья)
	private Node3D parentNode;

	// Максимальное здоровье
	[Export]
	protected float MaxHealth = 100f;
	[Export]
	// отображение урона damageVisibility
	private DamageVisibility damageVisibility;

	// Текущее здоровье
	protected float CurrentHealth;

	// получаем родителя и задаем здоровье
	public override void _Ready()
	{
		parentNode = GetParent<Node3D>();
		CurrentHealth = MaxHealth;
	}

	public override void _Process(double delta) // прикрепляет damageVisibility за объектом
	{
		damageVisibility.Position = parentNode.Position;
	}

	// Наносит урон и проверяет смерть
	public void TakeDamage(float damage)
	{
		CurrentHealth -= damage;
		damageVisibility?.DamageAnimation(CurrentHealth);
		if (CurrentHealth <=0)
		{
			Die();
			damageVisibility.QueueFree();
		}
	}

	// Удаляет родительский узел при смерти
	private void Die()
	{
		GD.Print("died");
		parentNode.QueueFree();
	}
}
using Godot;
using System;

[GlobalClass]
public partial class PushingDamageComponent : DamageComponent
{
	// Сила отталкивания цели
	private float PushDistance = 20f;

	// MoveComponent пули для определения нааправления отталкивания врага
	[Export]
	private MoveComponent _bulletMoveComponent;

	// Наносит урон цели и отталкивает её
	public override void ApplyDamage(CharacterBody3D target)
	{
		if (target != null)
		{
			base.ApplyDamage(target); 
			MoveComponent moveComponent = target.GetNodeOrNull<MoveComponent>("MoveComponent");
			if (moveComponent != null) PushingEnemies(moveComponent);
		}

	}

	// Отталкивает цель в направлении движения пули
	private void PushingEnemies(MoveComponent moveComponent)
	{
		moveComponent.AddVector(_bulletMoveComponent.GetDirection() * _bulletMoveComponent.GetSpeed() * PushDistance);
	}
}
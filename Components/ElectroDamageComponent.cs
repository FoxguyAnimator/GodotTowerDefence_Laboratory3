using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ElectroDamageComponent : DamageComponent
{
	// родительская нода
	private Node3D parentNode;

    // анимация
    [Export]
    private AnimationPlayer animationPlayer;

    // если попал в одного пулей то другим урон от кольца
    [Export]
    private bool firstHit = true;

    public override void _Ready()
    {
        base._Ready();
		parentNode = GetParent<Node3D>();
    }

	// наносит урон и запускает анимацию кольца
	public override void ApplyDamage(CharacterBody3D target)
    {
        if (target != null)
        {
            HealthComponent health_comp = target.GetNodeOrNull<HealthComponent>("HealthComponent");
            health_comp?.TakeDamage(DamageAmount);

            animationPlayer.Play("ElectroRing");

        }
    }

    // обрабатывает попадание пули и применяет урон в зависимости от firstHit
    public override void OnBulletBodyEntered(Node3D body)
	{
		
		if (body is CharacterBody3D character)
		{
            //GD.Print("bullet collided with " + body.Name + body.GetType());
            if (firstHit) 
            {
                ApplyDamage(character);
                firstHit = false;
            }
            else 
            {
                ElectroRing(character);
            }
            
		}
	}

    // Наносит урон электрическим кольцом
    private void ElectroRing(Node3D EnemyTower)
    {
        
        if (EnemyTower == null || EnemyTower.GetNodeOrNull<HealthComponent>("HealthComponent") == null) return;

        HealthComponent health_comp = EnemyTower.GetNodeOrNull<HealthComponent>("HealthComponent");
        health_comp?.TakeDamage(DamageAmount);

    }

    private void AnimationFinished(StringName AnimationName)
    {
        animationPlayer.Stop();
        firstHit = true;
    }
}
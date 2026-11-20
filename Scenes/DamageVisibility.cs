using Godot;
using System;

public partial class DamageVisibility : Node3D
{
	// надпись с колвом здоровья
	private Label3D _label3D;

	// анимация
	private AnimationPlayer _animationPlayer;

	// GameManager
	private GameManager _gameManager;

	// ссылки на компоненты и GameManager
	public override void _Ready()
	{
		_label3D = GetNode<Label3D>("Label3D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_gameManager = GetNode<GameManager>("/root/GameManager");

	}

	// поворот и размер надписи в зависимости от камеры
	public override void _Process(double delta)
	{
		Vector3 cameraRot = _gameManager.GetCameraRotation();
		Rotation = cameraRot;
		float labelScaleMultiplier = _gameManager.GetCameraScrollLevel();
		Scale = Vector3.One*labelScaleMultiplier*0.3f;
	}

	// отображает текущее здоровье и запускает анимацию урона
	public void DamageAnimation(float CurrentHealth)
	{
		_label3D.Text = CurrentHealth.ToString();
		if (_animationPlayer.IsAnimationActive())
		{
			_animationPlayer.Seek(0.1, true);
		}
		_animationPlayer.Play("TakingDamage");

	}
}
using Godot;
using System;

public partial class Core : CharacterBody3D
{
	private MeshInstance3D _gear;
	public override void _Ready()
	{
		_gear = GetNodeOrNull<MeshInstance3D>("GearMesh");
	}

	public override void _Process(double delta)
	{
		_gear.RotateY(0.01f); // просто крутит шестеренку

	}
}

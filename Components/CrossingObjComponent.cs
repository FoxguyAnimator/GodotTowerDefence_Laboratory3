using Godot;
using System;

[GlobalClass]
public partial class CrossingObjComponent : Area3D
{
	// Материал отображения возможности строительства.
	StandardMaterial3D buildingMaterial = GD.Load<StandardMaterial3D>("res://Assets/Material/building material.tres");

	// разрешено ли строительство для данной точки
	public bool buildingIsAllowed = true;

	// Колво объектов пересекающихся с зоной строительства
	private int overlappingBodiesCount = 0;

	// Настраивает начальный цвет материала строительства (зеленый)
	public override void _Ready()
	{
		buildingMaterial.AlbedoColor = Color.FromHsv(0.33f, 1.0f, 1.0f, 0.5f);
	}

	// Обрабатывает пересечение(вход) объектов со строящейся башней
	public void OnBodyEntered(Node3D body)
	{
		if (body is CharacterBody3D character)
		{
		    GD.Print("Can not build here");
			buildingMaterial.AlbedoColor = Color.FromHsv(0.0f, 1.0f, 1.0f, 0.5f); //делает объект красным
			buildingIsAllowed = false; 
			overlappingBodiesCount+=1;
		}
	}

	// Обрабатывает пересечение(выход) объектов со строящейся башней
	public void OnBodyExited(Node3D body)
	{
		if (body is CharacterBody3D character)
			overlappingBodiesCount-=1;
		if (overlappingBodiesCount == 0) // пока есть хотя бы одно пересекающееся тело - строить нельзя
		{
			//GD.Print("Can build here");
			buildingMaterial.AlbedoColor = Color.FromHsv(0.33f, 1.0f, 1.0f, 0.5f); // возвращает зеленый цвет
			buildingIsAllowed = true;
		}
	}
}
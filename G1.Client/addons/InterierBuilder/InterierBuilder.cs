#if TOOLS
using Godot;
using Godot.Collections;
using System;
using System.Linq;

[Tool]
public partial class InterierBuilder : Node
{

	[Export]
	public float TileWidth { get; set; } = 2;
	[Export]
	public float TileLength { get; set; } = 2;
	[Export]
	public float TileHeight { get; set; } = 2;

	private MeshInstance3D floor;
	[Export]
	public MeshInstance3D Floor
	{
		get => floor;
		set
		{
			floor = value;
			UpdateConfigurationWarnings();
		}
	}

	private InterierMapResource interierMap;
	[Export]
	public InterierMapResource InterierMap
	{
		get => interierMap;
		set
		{
			interierMap = value;
			UpdateConfigurationWarnings();
		}
	}


	public InterierMapTile[][] FloorMap = new InterierMapTile[][]{
			new InterierMapTile[]{
				new InterierMapTile{X=-1, Y=0, Floor = true},new InterierMapTile{X=0, Y=0, Floor = true},
				new InterierMapTile{X=-1, Y=-1, Floor = true},new InterierMapTile{X=0, Y=-1, Floor = true},
				new InterierMapTile{X=-1, Y=-2, Floor = true},new InterierMapTile{X=0, Y=-2, Floor = true},
			}
		};

	public override void _EnterTree()
	{
		ChildEnteredTree += HideChildren;
	}
	public override void _ExitTree()
	{
		ChildEnteredTree -= HideChildren;
	}

	private static void HideChildren(Node child)
	{
		GD.Print($"Disabling node '{child.Name}'");
		child.SetAllProcessing(false);
		if (child is Node3D visible)
		{
			visible.Hide();
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void Build()
	{
		var output = GetParent() as Node3D;

		if (output is null) return;
		if (Floor is null) return;
		GD.Print("Building scene.");


		var floorMeshes = new MultiMesh();
		floorMeshes.Mesh = Floor.Mesh;
		floorMeshes.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
		floorMeshes.InstanceCount = FloorMap.Sum(floor => floor.Count(t => t.Floor));


		var meshIndex = 0;
		for (int floorIndex = 0; floorIndex < FloorMap.Length; floorIndex++)
		{
			var floorTiles = FloorMap[floorIndex];
			for (int tileIndex = 0; tileIndex < floorTiles.Length; tileIndex++)
			{
				var tile = floorTiles[tileIndex];
				if (tile.Floor)
				{
					var transform = output.Transform
						.Translated(new Vector3(tile.X * TileWidth, floorIndex * TileHeight, tile.Y * TileLength));
					floorMeshes.SetInstanceTransform(meshIndex, transform);
					meshIndex++;
				}
			}
		}

		var floorNode = new MultiMeshInstance3D();
		floorNode.Name = "Floor";
		floorNode.Multimesh = floorMeshes;


		foreach (var child in output.GetChildren())
		{
			if (child is InterierBuilder)
				continue;

			output.RemoveChild(child);
		}

		output.AddChild(floorNode);
		floorNode.Owner = GetTree().EditedSceneRoot;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override string[] _GetConfigurationWarnings()
	{
		var result = new System.Collections.Generic.List<string>();

		if (Floor is null)
			result.Add("Floor mesh was not found.");

		if (InterierMap is null)
			result.Add("Interier map resource is not set.");

		return result.ToArray();
	}

	public void UpdateTiles(Dictionary<Vector2I, InterierMapTile> tiles)
	{
		if (this.InterierMap is null)
		{
			GD.Print("Interier map resource is not set. Skip.");
			return;
		}

		this.InterierMap.Tiles = tiles;
		GD.Print("Saving interier map.");
		ResourceSaver.Save(this.InterierMap);
	}
}
#endif

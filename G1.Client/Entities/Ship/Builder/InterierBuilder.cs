using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class InterierBuilder : Node
{
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

	public float TileWidth { get; set; } = 2;
	public float TileLength { get; set; } = 2;
	public float TileHeight { get; set; } = 2;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var output = GetParent() as Node3D;

		if (output is null) return;
		if (Floor is null) return;


		var floorMap = new InterierMapTile[][]{
			new InterierMapTile[]{
				new InterierMapTile{X=-1, Y=0, Floor = true},new InterierMapTile{X=0, Y=0, Floor = true},
				new InterierMapTile{X=-1, Y=-1, Floor = true},new InterierMapTile{X=0, Y=-1, Floor = true},
			}
		};
		var floorMeshes = new MultiMesh();
		floorMeshes.Mesh = Floor.Mesh;
		floorMeshes.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
		floorMeshes.InstanceCount = floorMap.Sum(floor => floor.Count(t => t.Floor));


		var meshIndex = 0;
		for (int floorIndex = 0; floorIndex < floorMap.Length; floorIndex++)
		{
			var floorTiles = floorMap[floorIndex];
			for (int tileIndex = 0; tileIndex < floorTiles.Length; tileIndex++)
			{
				var tile = floorTiles[tileIndex];
				if (tile.Floor)
				{
					var transform = output.Transform
						.Translated(new Vector3(tile.X * TileWidth, tile.Y * TileLength, floorIndex * TileHeight));
					floorMeshes.SetInstanceTransform(meshIndex, transform);
					meshIndex++;
				}
			}
		}

		var floorNode = new MultiMeshInstance3D();
		floorNode.Multimesh = floorMeshes;
		output.AddChild(floorNode);
		floorNode.Owner = output.Owner;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override string[] _GetConfigurationWarnings()
	{
		var result = new List<string>();
		if (Floor is null)
			result.Add("Floor mesh was not found");
		return result.ToArray();
	}


	private enum WallType { None, Wall, Door }
	private class InterierMapTile
	{
		public int X { get; set; }
		public int Y { get; set; }
		public bool Floor { get; set; }
		public bool Ceiling { get; set; }
		public WallType Front { get; set; }
		public WallType Back { get; set; }
		public WallType Left { get; set; }
		public WallType Right { get; set; }
	}
}

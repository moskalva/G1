#if TOOLS
using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
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

	private MeshInstance3D ceiling;
	[Export]
	public MeshInstance3D Ceiling
	{
		get => ceiling;
		set
		{
			ceiling = value;
			UpdateConfigurationWarnings();
		}
	}

	private MeshInstance3D door;
	[Export]
	public MeshInstance3D Door
	{
		get => door;
		set
		{
			door = value;
			UpdateConfigurationWarnings();
		}
	}

	private MeshInstance3D doorUpper;
	[Export]
	public MeshInstance3D DoorUpper
	{
		get => doorUpper;
		set
		{
			doorUpper = value;
			UpdateConfigurationWarnings();
		}
	}

	private MeshInstance3D doorBottom;
	[Export]
	public MeshInstance3D DoorBottom
	{
		get => doorBottom;
		set
		{
			doorBottom = value;
			UpdateConfigurationWarnings();
		}
	}

	private MeshInstance3D wall;
	[Export]
	public MeshInstance3D Wall
	{
		get => wall;
		set
		{
			wall = value;
			UpdateConfigurationWarnings();
		}
	}

	private MeshInstance3D wallUpper;
	[Export]
	public MeshInstance3D WallUpper
	{
		get => wallUpper;
		set
		{
			wallUpper = value;
			UpdateConfigurationWarnings();
		}
	}

	private MeshInstance3D wallBottom;
	[Export]
	public MeshInstance3D WallBottom
	{
		get => wallBottom;
		set
		{
			wallBottom = value;
			UpdateConfigurationWarnings();
		}
	}

	private MeshInstance3D corner;
	[Export]
	public MeshInstance3D Corner
	{
		get => corner;
		set
		{
			corner = value;
			UpdateConfigurationWarnings();
		}
	}

	private MeshInstance3D cornerUpper;
	[Export]
	public MeshInstance3D CornerUpper
	{
		get => cornerUpper;
		set
		{
			cornerUpper = value;
			UpdateConfigurationWarnings();
		}
	}

	private MeshInstance3D cornerBottom;
	[Export]
	public MeshInstance3D CornerBottom
	{
		get => cornerBottom;
		set
		{
			cornerBottom = value;
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
		Build();
	}

	public void Build()
	{
		var output = GetParent() as Node3D;

		if (output is null) return;
		if (_GetConfigurationWarnings().Length > 0) return;

		GD.Print("Building scene.");
		RemoveChildren(output);

		var floorTransforms = new List<Transform3D>();
		var ceilingTransforms = new List<Transform3D>();
		var doorTransforms = new List<Transform3D>();
		var wallTransforms = new List<Transform3D>();
		var cornerTransforms = new List<Transform3D>();
		var doorUpperTransforms = new List<Transform3D>();
		var wallUpperTransforms = new List<Transform3D>();
		var cornerUpperTransforms = new List<Transform3D>();
		var doorBottomTransforms = new List<Transform3D>();
		var wallBottomTransforms = new List<Transform3D>();
		var cornerBottomTransforms = new List<Transform3D>();

		var baseTransform = output.Transform
			.Rotated(Vector3.Up, -Mathf.Pi / 2); // blender models are rotated 90 degrees because of xyz order differences
		foreach (var (index, tile) in this.InterierMap.Tiles)
		{
			var position = GetTilePosition(index);
			if (tile.Floor)
			{
				floorTransforms.Add(baseTransform.Translated(position));
			}
			if (tile.Ceiling)
			{
				ceilingTransforms.Add(baseTransform.Translated(position));
			}
			if (tile.Back == WallType.Door)
			{
				var transforms = !tile.Ceiling ? doorBottomTransforms :
								 !tile.Floor ? doorUpperTransforms :
								 doorTransforms;
				transforms.Add(baseTransform.Translated(position));
			}
			else if (tile.Back == WallType.Wall)
			{
				var transforms = !tile.Ceiling ? wallBottomTransforms :
								 !tile.Floor ? wallUpperTransforms :
								 wallTransforms;
				transforms.Add(baseTransform.Translated(position));
			}

			if (tile.Front == WallType.Door)
			{
				var transforms = !tile.Ceiling ? doorBottomTransforms :
								 !tile.Floor ? doorUpperTransforms :
								 doorTransforms;
				transforms.Add(baseTransform.Rotated(Vector3.Up, Mathf.Pi).Translated(position));
				
			}
			else if (tile.Front == WallType.Wall)
			{
				var transforms = !tile.Ceiling ? wallBottomTransforms :
								 !tile.Floor ? wallUpperTransforms :
								 wallTransforms;
				transforms.Add(baseTransform.Rotated(Vector3.Up, Mathf.Pi).Translated(position));
			}

			if (tile.Left == WallType.Door)
			{
				var transforms = !tile.Ceiling ? doorBottomTransforms :
								 !tile.Floor ? doorUpperTransforms :
								 doorTransforms;
				transforms.Add(baseTransform.Rotated(Vector3.Up, Mathf.Pi / 2).Translated(position));
			}
			else if (tile.Left == WallType.Wall)
			{
				var transforms = !tile.Ceiling ? wallBottomTransforms :
								 !tile.Floor ? wallUpperTransforms :
								 wallTransforms;
				transforms.Add(baseTransform.Rotated(Vector3.Up, Mathf.Pi / 2).Translated(position));
			}

			if (tile.Right == WallType.Door)
			{
				var transforms = !tile.Ceiling ? doorBottomTransforms :
								 !tile.Floor ? doorUpperTransforms :
								 doorTransforms;
				transforms.Add(baseTransform.Rotated(Vector3.Up, -Mathf.Pi / 2).Translated(position));
			}
			else if (tile.Right == WallType.Wall)
			{
				var transforms = !tile.Ceiling ? wallBottomTransforms :
								 !tile.Floor ? wallUpperTransforms :
								 wallTransforms;
				transforms.Add(baseTransform.Rotated(Vector3.Up, -Mathf.Pi / 2).Translated(position));
			}

			// corners
			if (tile.Back != WallType.None)
			{
				if (tile.Left == WallType.None)
				{
					var positionToLeft = new Vector3I(index.X - 1, index.Y, index.Z);
					if (!this.InterierMap.Tiles.TryGetValue(positionToLeft, out var leftTile) ||
						leftTile.Back == WallType.None)
					{
						var cornerPosition = GetTilePosition(positionToLeft);
						cornerTransforms.Add(baseTransform.Rotated(Vector3.Up, -Mathf.Pi / 2).Translated(cornerPosition));
					}
				}
				if (tile.Right == WallType.None)
				{
					var positionToRight = new Vector3I(index.X + 1, index.Y, index.Z);
					if (!this.InterierMap.Tiles.TryGetValue(positionToRight, out var rightTile) ||
						rightTile.Back == WallType.None)
					{
						var cornerPosition = GetTilePosition(positionToRight);
						cornerTransforms.Add(baseTransform.Rotated(Vector3.Up, 0).Translated(cornerPosition));
					}
				}
			}

			if (tile.Front != WallType.None)
			{
				if (tile.Left == WallType.None)
				{
					var positionToLeft = new Vector3I(index.X - 1, index.Y, index.Z);
					if (!this.InterierMap.Tiles.TryGetValue(positionToLeft, out var leftTile) ||
						leftTile.Front == WallType.None)
					{
						var cornerPosition = GetTilePosition(positionToLeft);
						cornerTransforms.Add(baseTransform.Rotated(Vector3.Up, Mathf.Pi).Translated(cornerPosition));
					}
				}
				if (tile.Right == WallType.None)
				{
					var positionToRight = new Vector3I(index.X + 1, index.Y, index.Z);
					if (!this.InterierMap.Tiles.TryGetValue(positionToRight, out var rightTile) ||
						rightTile.Front == WallType.None)
					{
						var cornerPosition = GetTilePosition(positionToRight);
						cornerTransforms.Add(baseTransform.Rotated(Vector3.Up, Mathf.Pi / 2).Translated(cornerPosition));
					}
				}
			}

			if (tile.Left != WallType.None)
			{
				if (tile.Back == WallType.None)
				{
					var positionToBack = new Vector3I(index.X, index.Y - 1, index.Z);
					if (!this.InterierMap.Tiles.TryGetValue(positionToBack, out var frontTile) ||
						frontTile.Left == WallType.None)
					{
						var cornerPosition = GetTilePosition(positionToBack);
						cornerTransforms.Add(baseTransform.Rotated(Vector3.Up, Mathf.Pi / 2).Translated(cornerPosition));
					}
				}
				if (tile.Front == WallType.None)
				{
					var positionToFront = new Vector3I(index.X, index.Y + 1, index.Z);
					if (!this.InterierMap.Tiles.TryGetValue(positionToFront, out var backTile) ||
						backTile.Left == WallType.None)
					{
						var cornerPosition = GetTilePosition(positionToFront);
						cornerTransforms.Add(baseTransform.Rotated(Vector3.Up, 0).Translated(cornerPosition));
					}
				}
			}

			if (tile.Right != WallType.None)
			{
				if (tile.Back == WallType.None)
				{
					var positionToBack = new Vector3I(index.X, index.Y - 1, index.Z);
					if (!this.InterierMap.Tiles.TryGetValue(positionToBack, out var frontTile) ||
						frontTile.Right == WallType.None)
					{
						var cornerPosition = GetTilePosition(positionToBack);
						cornerTransforms.Add(baseTransform.Rotated(Vector3.Up, Mathf.Pi).Translated(cornerPosition));
					}
				}
				if (tile.Front == WallType.None)
				{
					var positionToFront = new Vector3I(index.X, index.Y + 1, index.Z);
					if (!this.InterierMap.Tiles.TryGetValue(positionToFront, out var backTile) ||
						backTile.Right == WallType.None)
					{
						var cornerPosition = GetTilePosition(positionToFront);
						cornerTransforms.Add(baseTransform.Rotated(Vector3.Up, -Mathf.Pi / 2).Translated(cornerPosition));
					}
				}
			}
		}


		AddMultimesh(output, "Floor", Floor.Mesh, floorTransforms);
		AddMultimesh(output, "Ceiling", Ceiling.Mesh, ceilingTransforms);
		AddMultimesh(output, "Door", Door.Mesh, doorTransforms);
		AddMultimesh(output, "Wall", Wall.Mesh, wallTransforms);
		AddMultimesh(output, "Corner", Corner.Mesh, cornerTransforms);
		AddMultimesh(output, "DoorUpper", DoorUpper.Mesh, doorUpperTransforms);
		AddMultimesh(output, "WallUpper", WallUpper.Mesh, wallUpperTransforms);
		AddMultimesh(output, "CornerUpper", CornerUpper.Mesh, cornerUpperTransforms);
		AddMultimesh(output, "DoorBottom", DoorBottom.Mesh, doorBottomTransforms);
		AddMultimesh(output, "WallBottom", WallBottom.Mesh, wallBottomTransforms);
		AddMultimesh(output, "CornerBottom", CornerBottom.Mesh, cornerBottomTransforms);
	}
	private Vector3 GetTilePosition(Vector3I tileIndex)
	{
		return new Vector3(tileIndex.X * TileWidth, tileIndex.Z * TileHeight, tileIndex.Y * TileLength);
	}

	private void AddMultimesh(Node3D output, string name, Mesh mesh, ICollection<Transform3D> tranforms)
	{
		if (tranforms.Count == 0)
			return;

		var meshes = new MultiMesh();
		meshes.Mesh = mesh;
		meshes.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
		meshes.InstanceCount = tranforms.Count;

		var meshIndex = 0;
		foreach (var transform in tranforms)
		{
			meshes.SetInstanceTransform(meshIndex, transform);
			meshIndex++;
		}

		var child = new MultiMeshInstance3D();
		child.Name = name;
		child.Multimesh = meshes;

		output.AddChild(child);
		child.Owner = output.GetTree().EditedSceneRoot;
	}

	private static void RemoveChildren(Node3D output)
	{
		foreach (var child in output.GetChildren())
		{
			if (child is InterierBuilder)
				continue;

			output.RemoveChild(child);
		}
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
		if (Ceiling is null)
			result.Add("Ceiling mesh was not found.");
		if (Door is null)
			result.Add("Door mesh was not found.");
		if (Wall is null)
			result.Add("Wall mesh was not found.");
		if (Corner is null)
			result.Add("Corner mesh was not found.");

		if (InterierMap is null)
			result.Add("Interier map resource is not set.");

		return result.ToArray();
	}

	public void UpdateTiles(Godot.Collections.Dictionary<Vector3I, InterierMapTile> tiles)
	{
		if (this.InterierMap is null)
		{
			GD.Print("Interier map resource is not set. Skip.");
			return;
		}

		this.InterierMap.Tiles = tiles;
		GD.Print($"Saving interier map. Tilescount: {tiles.Count}");
		ResourceSaver.Save(this.InterierMap);
	}
}
#endif

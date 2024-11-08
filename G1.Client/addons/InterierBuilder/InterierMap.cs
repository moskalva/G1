using Godot;
using System;
using System.Collections.Generic;

public enum CellChangeTarget { Floor, WallLeft, WallRight, WallBack, WallFront, Ceiling }
public enum CellChangeType { Primary, Secondary }

[Tool]
public partial class InterierMap : Node2D
{
	[Signal]
	public delegate void CellChanedEventHandler(Vector2I cellIndex, CellChangeTarget target, CellChangeType type);

	[Export]
	public ushort GridSize { get; set; } = 50;

	[Export]
	public float GridCellSize { get; set; } = 50;

	private Vector2 zoom = new Vector2(0.2f, 0.2f);
	[Export]
	public float ZoomDistance
	{
		get => zoom.X;
		set => zoom = new Vector2(value, value);
	}

	[Export]
	public Color GridColor { get; set; } = Colors.White;
	[Export]
	public Color BackgroundColor { get; set; } = Colors.LightGray;
	[Export]
	public Texture2D WallTexture { get; set; }
	[Export]
	public Texture2D DoorTexture { get; set; }
	[Export]
	public Texture2D FloorTexture { get; set; }
	[Export]
	public Texture2D CeilingTexture { get; set; }

	private Dictionary<Vector2I, InterierMapTile> tiles = new Dictionary<Vector2I, InterierMapTile>();
	private Camera2D camera;
	private Stencil stencil;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode<Camera2D>("Camera");
		stencil = GetNode<Stencil>("Stencil");
		stencil.CellSize = this.GridCellSize;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch (mouseEvent.ButtonIndex)
			{
				case MouseButton.WheelUp:
					camera.Zoom += zoom;
					break;
				case MouseButton.WheelDown:
					var newZoom = camera.Zoom - zoom;
					if (newZoom.X > 0.1)
						camera.Zoom = newZoom;
					break;
			}
		}
		else if (@event is InputEventMouseMotion mouseMove)
		{
			if (Input.IsMouseButtonPressed(MouseButton.Middle))
			{
				GD.Print("Mouse moove");
				camera.Position -= mouseMove.Relative;
			}
			else
			{
				var cellIndex = GetCellIndex();
				stencil.Position = new Vector2(
					cellIndex.X * GridCellSize - GridCellSize / 2,
					cellIndex.Y * GridCellSize - GridCellSize / 2);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Draw()
	{
		var min = -GridSize / 2 * GridCellSize;
		var max = GridSize / 2 * GridCellSize;
		var gridSize = GridSize * GridCellSize;

		// background
		DrawRect(new Rect2(new Vector2(min, min), new Vector2(gridSize, gridSize)), BackgroundColor, filled: true);

		// grid
		for (var x = -GridSize / 2; x <= GridSize / 2; x++)
		{
			var xPosition = x * GridCellSize;
			var from = new Vector2(xPosition, min);
			var to = new Vector2(xPosition, max);
			DrawLine(from, to, GridColor, -1, false);
		}

		for (var y = -GridSize / 2; y <= GridSize / 2; y++)
		{
			var yPosition = y * GridCellSize;
			var from = new Vector2(min, yPosition);
			var to = new Vector2(max, yPosition);
			DrawLine(from, to, GridColor, -1, false);
		}

		// tiles
		var cellSize = new Vector2(GridCellSize, GridCellSize);
		var cellRelativePosition = new Vector2(-GridCellSize / 2, -GridCellSize / 2);
		foreach (var (index, tile) in this.tiles)
		{
			var position = new Vector2(index.X * GridCellSize, index.Y * GridCellSize);
			var center = position + (cellSize / 2);

			if (tile.Ceiling)
			{
				DrawSetTransform(center, 0);
				DrawTextureRect(CeilingTexture, new Rect2(cellRelativePosition, cellSize), false);
			}
			
			if (tile.Floor)
			{
				DrawSetTransform(center, 0);
				DrawTextureRect(FloorTexture, new Rect2(cellRelativePosition, cellSize), false);
			}


			if (tile.Back == WallType.Wall)
			{
				DrawSetTransform(center, 0);
				DrawTextureRect(WallTexture, new Rect2(cellRelativePosition, cellSize), false);
			}
			else if (tile.Back == WallType.Door)
			{
				DrawSetTransform(center, 0);
				DrawTextureRect(DoorTexture, new Rect2(cellRelativePosition, cellSize), false);
			}

			if (tile.Front == WallType.Wall)
			{
				DrawSetTransform(center, Mathf.Pi);
				DrawTextureRect(WallTexture, new Rect2(cellRelativePosition, cellSize), false);
			}
			else if (tile.Front == WallType.Door)
			{
				DrawSetTransform(center, Mathf.Pi);
				DrawTextureRect(DoorTexture, new Rect2(cellRelativePosition, cellSize), false);
			}

			if (tile.Left == WallType.Wall)
			{
				DrawSetTransform(center, -Mathf.Pi / 2);
				DrawTextureRect(WallTexture, new Rect2(cellRelativePosition, cellSize), false);
			}
			else if (tile.Left == WallType.Door)
			{
				DrawSetTransform(center, -Mathf.Pi / 2);
				DrawTextureRect(DoorTexture, new Rect2(cellRelativePosition, cellSize), false);
			}

			if (tile.Right == WallType.Wall)
			{
				DrawSetTransform(center, Mathf.Pi / 2);
				DrawTextureRect(WallTexture, new Rect2(cellRelativePosition, cellSize), false);
			}
			else if (tile.Right == WallType.Door)
			{
				DrawSetTransform(center, Mathf.Pi / 2);
				DrawTextureRect(DoorTexture, new Rect2(cellRelativePosition, cellSize), false);
			}
		}
	}

	private void OnFloorInputEvent(Node viewPort, InputEvent @event, int shapeIndex)
	{
		OnInputEvent(@event, CellChangeTarget.Floor);
	}
	private void OnCeilingInputEvent(Node viewPort, InputEvent @event, int shapeIndex)
	{
		OnInputEvent(@event, CellChangeTarget.Ceiling);
	}

	private void OnWallLeftInputEvent(Node viewPort, InputEvent @event, int shapeIndex)
	{
		OnInputEvent(@event, CellChangeTarget.WallLeft);
	}
	private void OnWallRightInputEvent(Node viewPort, InputEvent @event, int shapeIndex)
	{
		OnInputEvent(@event, CellChangeTarget.WallRight);
	}

	private void OnWallBackInputEvent(Node viewPort, InputEvent @event, int shapeIndex)
	{
		OnInputEvent(@event, CellChangeTarget.WallBack);
	}

	private void OnWallFrontInputEvent(Node viewPort, InputEvent @event, int shapeIndex)
	{
		OnInputEvent(@event, CellChangeTarget.WallFront);
	}

	private void OnInputEvent(InputEvent @event, CellChangeTarget target)
	{
		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed)
		{
			CellChangeType? type = mouseButtonEvent.ButtonIndex switch
			{
				MouseButton.Left => CellChangeType.Primary,
				MouseButton.Right => CellChangeType.Secondary,
				_ => null
			};
			if (!type.HasValue)
				return;


			var cellIndex = GetCellIndex();
			UpdateTile(cellIndex, target, type.Value);
			QueueRedraw();
			EmitSignal(SignalName.CellChaned, cellIndex, (int)target, (int)type.Value);
		}
	}

	private void UpdateTile(Vector2I cellIndex, CellChangeTarget target, CellChangeType type)
	{
		var tile = GetOrCreateTile(cellIndex);

		if (target == CellChangeTarget.Floor)
			tile.Floor = !tile.Floor;
		else if (target == CellChangeTarget.Ceiling)
			tile.Ceiling = !tile.Ceiling;
		else if (target == CellChangeTarget.WallBack)
			tile.Back = ChangeWallType(tile.Back);
		else if (target == CellChangeTarget.WallLeft)
			tile.Left = ChangeWallType(tile.Left);
		else if (target == CellChangeTarget.WallRight)
			tile.Right = ChangeWallType(tile.Right);
		else if (target == CellChangeTarget.WallFront)
			tile.Front = ChangeWallType(tile.Front);


		WallType ChangeWallType(WallType current) => current switch
		{
			WallType.None => WallType.Wall,
			WallType.Wall => WallType.Door,
			_ => WallType.None
		};
	}

	private InterierMapTile GetOrCreateTile(Vector2I cellIndex)
	{
		if (this.tiles.TryGetValue(cellIndex, out var existing))
			return existing;
		else
		{
			var newTile = new InterierMapTile()
			{
				X = cellIndex.X,
				Y = cellIndex.Y,
			};
			tiles[cellIndex] = newTile;
			return newTile;
		};
	}

	private Vector2I GetCellIndex()
	{
		var localPosition = GetLocalMousePosition();
		return new Vector2I(
			Mathf.FloorToInt(localPosition.X / GridCellSize),
			Mathf.FloorToInt(localPosition.Y / GridCellSize)
		);
	}
}

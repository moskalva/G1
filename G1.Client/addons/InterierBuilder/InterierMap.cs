using Godot;
using Godot.Collections;
using System;

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


	private int currentFloorIndex;
	[Export]
	public int CurrentFloorIndex
	{
		get => currentFloorIndex;
		set
		{
			value = currentFloorIndex;
			QueueRedraw();
		}
	}


	private Dictionary<Vector3I, InterierMapTile> tiles = new Dictionary<Vector3I, InterierMapTile>();
	public Dictionary<Vector3I, InterierMapTile> Tiles
	{
		get => tiles;
		set
		{
			tiles = value;
			QueueRedraw();
		}

	}
	private Camera2D camera;
	private Stencil stencil;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode<Camera2D>("Camera");
		stencil = GetNode<Stencil>("Stencil");
		stencil.CellSize = this.GridCellSize;
	}

	public override void _UnhandledInput(InputEvent @event)
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
				camera.Position -= mouseMove.Relative * zoom * 5;
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

	#region Drawing
	public override void _Draw()
	{
		var min = -GridSize / 2 * GridCellSize;
		var max = GridSize / 2 * GridCellSize;
		var gridSize = GridSize * GridCellSize;

		DrawBackground(min, gridSize);
		DrawGrid(min, max);
		DrawTiles();
	}

	private void DrawTiles()
	{
		var cellSize = new Vector2(GridCellSize, GridCellSize);
		var cellRelativePosition = new Vector2(-GridCellSize / 2, -GridCellSize / 2);
		foreach (var (index, tile) in this.Tiles)
		{
			if (index.Z != CurrentFloorIndex)
				continue;

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

	private void DrawBackground(float min, float gridSize)
	{
		DrawRect(new Rect2(new Vector2(min, min), new Vector2(gridSize, gridSize)), BackgroundColor, filled: true);
	}

	private void DrawGrid(float min, float max)
	{
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
	}

	#endregion

	#region Input Handlers
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
		if (@event is InputEventMouseMotion mouse)
		{
			CellChangeType? type = Input.IsMouseButtonPressed(MouseButton.Left) ? CellChangeType.Primary
						 		 : Input.IsMouseButtonPressed(MouseButton.Right) ? CellChangeType.Secondary
						 		 : null;
			if (!type.HasValue)
				return;


			var cellIndex = GetCellIndex();
			UpdateTile(cellIndex, target, type.Value);
			QueueRedraw();
			EmitSignal(SignalName.CellChaned, cellIndex, (int)target, (int)type.Value);
		}
	}

	private void UpdateTile(Vector3I cellIndex, CellChangeTarget target, CellChangeType type)
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

		if (tile.IsEmpty)
			this.Tiles.Remove(cellIndex);

		WallType ChangeWallType(WallType current) => current switch
		{
			WallType.None => WallType.Wall,
			WallType.Wall => WallType.Door,
			_ => WallType.None
		};
	}

	private InterierMapTile GetOrCreateTile(Vector3I cellIndex)
	{
		if (this.Tiles.TryGetValue(cellIndex, out var existing))
			return existing;
		else
		{
			var newTile = new InterierMapTile();
			this.Tiles[cellIndex] = newTile;
			return newTile;
		};
	}

	private Vector3I GetCellIndex()
	{
		var localPosition = GetLocalMousePosition();
		return new Vector3I(
			Mathf.FloorToInt(localPosition.X / GridCellSize),
			Mathf.FloorToInt(localPosition.Y / GridCellSize),
			CurrentFloorIndex
		);
	}
	#endregion
}

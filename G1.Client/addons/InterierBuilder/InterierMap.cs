using Godot;
using System;

public enum CellChangeTarget { Floor, WallLeft, WallRight, WallBack, WallFront, Ceiling }
public enum CellChangeType { Primary, Secondary }

[Tool]
public partial class InterierMap : Node2D
{
	[Signal]
	public delegate void CellChanedEventHandler(Vector2I cellIndex, CellChangeTarget target, CellChangeType type);

	private Camera2D camera;
	private Stencil stencil;

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
		var cellSize = GridSize * GridCellSize;

		DrawRect(new Rect2(new Vector2(min, min), new Vector2(cellSize, cellSize)), BackgroundColor, filled: true);

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
			EmitSignal(SignalName.CellChaned, cellIndex, (int)target, (int)type.Value);
		}
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

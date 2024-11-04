using Godot;
using System;

[Tool]
public partial class InterierMap : Node2D
{
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
				var localPosition = GetLocalMousePosition();
				var cellIndex = new Vector2(
					Mathf.FloorToInt(localPosition.X / GridCellSize),
					Mathf.FloorToInt(localPosition.Y / GridCellSize)
				);
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
		for (var x = -GridSize / 2; x <= GridSize / 2; x++)
		{
			var xPosition = x * GridCellSize;
			var from = new Vector2(xPosition, min);
			var to = new Vector2(xPosition, max);
			DrawLine(from, to, Colors.White, -1, false);
		}

		for (var y = -GridSize / 2; y <= GridSize / 2; y++)
		{
			var yPosition = y * GridCellSize;
			var from = new Vector2(min, yPosition);
			var to = new Vector2(max, yPosition);
			DrawLine(from, to, Colors.White, -1, false);
		}
	}
}

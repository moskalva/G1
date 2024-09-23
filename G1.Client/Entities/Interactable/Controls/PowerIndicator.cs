using Godot;
using System;
using System.Xml.Schema;

public partial class PowerIndicator : Node2D
{
	private TextureRect progressBar;

	[Export]
	public float PowerUnitHeight { get; private set; } = 75;

	[Export]
	public uint MaxValue { get; set; } = 5;

	private uint currentValue = 0;
	[Export]
	public uint CurrentValue
	{
		get => currentValue;
		set
		{
			if (currentValue != value)
				DrawProgress(value);
			currentValue = value;
		}
	}


	private bool isActive = true;
	[Export]
	public bool IsActive
	{
		get => isActive;
		set
		{
			if (isActive != value)
				ChangeIsActive(value);
			isActive = value;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		progressBar = GetNode<TextureRect>("Indicator");

		if (this.GetParent() is null) Init();
	}

	public void Init()
	{
		DrawMaxProgress(MaxValue);
		DrawProgress(CurrentValue);
	}

	private void ChangeIsActive(bool isActive)
	{
		var progressBarMaterial = (ShaderMaterial)progressBar.Material;
		progressBarMaterial.SetShaderParameter("is_active", isActive);
	}

	private void DrawMaxProgress(uint value)
	{
		if (!this.IsNodeReady()) return;

		var progressBarMaterial = (ShaderMaterial)progressBar.Material;
		progressBarMaterial.SetShaderParameter("max_progress", value);

		var height = PowerUnitHeight * value;
		progressBar.Position = new Vector2(progressBar.Position.X, progressBar.Position.Y + progressBar.Size.Y - height);
		progressBar.Size = new Vector2(progressBar.Size.X, height);
	}

	private void DrawProgress(uint value)
	{
		if (!this.IsNodeReady()) return;
		var progressBarMaterial = (ShaderMaterial)progressBar.Material;
		progressBarMaterial.SetShaderParameter("current_progress", value);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

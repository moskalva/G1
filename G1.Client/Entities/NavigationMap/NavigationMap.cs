using Godot;
using System;

public partial class NavigationMap : Node3D
{
	[Export]
	public Exterier PayerShip { get; set; }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _OnRemoteStateChanged(ShipState remoteState)
	{
		if (PayerShip.Id.Equals(remoteState.Id))
		{
			PayerShip.Position = remoteState.Position;
			PayerShip.Velocity = remoteState.Velocity;
		}
		else
		{
			throw new NotImplementedException();
		}
	}
}

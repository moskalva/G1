

using System;
using System.Collections.Generic;
using G1.Model;
using Godot;

public partial class BaseShip : Node
{
	public WorldEntityId Id { get; set; }

    public ShipController Controller { get; private set; }
    public override void _Ready()
    {
        Controller = this.FindNode<ShipController>();
    }
}
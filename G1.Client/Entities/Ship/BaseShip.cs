

using System;
using System.Collections.Generic;
using G1.Model;
using Godot;

public partial class BaseShip : Node
{
    public WorldEntityId Id { get; set; }

    public ShipController Controller { get; private set; }

    public Exterier Exterier { get; private set; }
    public Node ExternalWorld { get; private set; }

    public override void _EnterTree()
    {
        this.Controller = this.FindNode<ShipController>();
        this.ExternalWorld = this.GetNode<Node>("ExternalWorld");
        this.Exterier = this.FindNode<Exterier>();
    }
}
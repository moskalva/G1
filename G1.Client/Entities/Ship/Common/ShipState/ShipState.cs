using G1.Model;
using Godot;
using System;

public partial class ShipState : GodotObject
{
    public WorldEntityId Id { get; set; }
	public WorldEntityType Type { get; set; }
	public uint SystemId { get; set; }
	public Vector3I ReferencePoint { get; set; }
	public Vector3 Position { get; set; }
	public Vector3 Velocity { get; set; }
    public Vector3 AngularVelocity;

}

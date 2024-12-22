using G1.Model;
using Godot;
using System;

public partial class ShipState : GodotObject, IEquatable<ShipState>
{
    public WorldEntityId Id { get; set; }
    public WorldEntityType Type { get; set; }
    public uint SystemId { get; set; }
    public Vector3I ReferencePoint { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Velocity { get; set; }
    public Vector3 AngularVelocity { get; set; }
    public float ThermalEmission { get; set; }
    public float EmEmission { get; set; }
    public float ParticleEmission { get; set; }
    

    public bool Equals(ShipState other)
    {
        return other != null
            && this.Id.Equals(other.Id)
            && this.Position.Equals(other.Position)
            && this.Velocity.Equals(other.Velocity)
            && this.Rotation.Equals(other.Rotation)
            && this.AngularVelocity.Equals(other.AngularVelocity)
            && this.ThermalEmission.Equals(other.ThermalEmission)
            && this.EmEmission.Equals(other.EmEmission)
            && this.ParticleEmission.Equals(other.ParticleEmission); 
    }
    public override bool Equals(object obj) => Equals(obj as ShipState);
    public override int GetHashCode()
        => HashCode.Combine(
            this.Id,
            this.Position,
            this.Velocity,
            this.Rotation,
            this.AngularVelocity,
            this.ThermalEmission,
            this.EmEmission,
            this.ParticleEmission);
    public override string ToString()
        => $"ShipState: Id: '{Id}', Position: '{Position}', Velocity: '{Velocity}', Rotation: '{Rotation}', AngularVelocity: '{AngularVelocity}', ThermalEmission: '{ThermalEmission}', EmEmission: '{EmEmission}', ParticleEmission: '{ParticleEmission}'";
}

public partial class IdWrap : GodotObject
{
    public WorldEntityId Id { get; set; }
}
using System;
using Godot;

public abstract partial class ControlPlace : StaticBody3D
{
    public bool IsActive { get; set; }

    public abstract Transform3D CharacterPosition { get; }
    public abstract CharacterPosture CharacterPosture { get; }
}
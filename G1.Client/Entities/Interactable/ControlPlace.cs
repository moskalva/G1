using System;
using Godot;

public enum CharacterPosture { Standing, Sitting }
public abstract partial class ControlPlace : StaticBody3D
{
    public abstract Transform3D CharacterPosition { get; }
    public abstract CharacterPosture CharacterPosture { get; }
}
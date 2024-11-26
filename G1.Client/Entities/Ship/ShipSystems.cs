using System;
using Godot;

public static class ShipSystems
{
    public static BaseShip Register<T>(T system) where T : Node
    {
        var ship = system.GetAccendant<BaseShip>();
        var key = typeof(T).Name;
        GD.Print($"Registering ship system '{key}'");
        ship.SetMeta(key, system);

        return ship;
    }

    public static T GetRegistered<T>(Node node) where T : Node
    {
        var ship = node.GetAccendant<BaseShip>();
        var key = typeof(T).Name;
        GD.Print($"Retrieving ship system '{key}'");
        var system = (T)ship.GetMeta(key);

        if (system == null)
            throw new InvalidOperationException($"Cannot find '{key}' in ship '{ship.Name}'");

        return system;
    }
}
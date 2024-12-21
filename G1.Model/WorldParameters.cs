namespace G1;

public static class WorldParameters
{
    public static Physics Physics { get; } = new();
    public static ShipsCatalog Ships { get; } = new();
    public static ThrustersCatalog Thrusters { get; } = new();
}

public class Physics
{
    public int SectorSize { get; } = 100_000_000;
    public float MinimalThermalEmission { get; } = 300f;
    public float ThermalEmissionCoefficient { get; } = 0.0001f;
    public float ThermalEmissionDumpSpeed { get; } = 100;
    public int FogOfWarDistance { get; } = 1000;
}

public class ShipsCatalog
{
    public Mark1 Mark1 { get; } = new();
}

public class Mark1
{

}
public class ThrustersCatalog
{
    public ThrustersCatalogL1 L1 { get; } = new();
}
public class ThrustersCatalogL1
{
    public ManeuvereThrustersL1 Maneuvere { get; } = new();
    public DragThrusterL1 Drag { get; } = new();
}
public class ManeuvereThrustersL1
{
    public float Power { get; } = 1000_000;
    public uint Levels { get; } = 3;
}

public class DragThrusterL1
{
    public float Power { get; } = 100_000_000;
    public uint Levels { get; } = 5;
}

using System.Buffers.Binary;

namespace G1.Server.Agents;

public static class WorldPositionTools
{
    private static readonly double SectorSize = Settings.SectorSize;
    private static readonly double Q = SectorSize / 2 / Math.Tan(60 * (Math.PI / 180));
    private static readonly double W = Math.Sqrt(Math.Pow(SectorSize, 2) - Math.Pow(SectorSize / 2, 2));

    public static AgentPosition GetSectorPosition(WorldSectorId baseSector, WorldSectorId sector)
    {
        if (baseSector.SystemId != sector.SystemId)
            throw new InvalidOperationException($"Cannot calculate relative position of sectors in different systems");

        var diffX = sector.X - baseSector.X;
        var diffY = sector.Y - baseSector.Y;
        var diffZ = sector.Z - baseSector.Z;

        return new AgentPosition
        {
            SectorId = baseSector,
            X = (float)(diffX * SectorSize + diffY * SectorSize / 2 + diffZ * Q),
            Y = (float)(diffY * W + diffZ * Q),
            Z = (float)(diffZ * W),
        };
    }

    public static bool TryGetUpdatedSector(AgentPosition currentPosition, out WorldSectorId updatedSector)
    {
        throw new NotImplementedException();
    }

    public static AgentPosition RelativePosition(WorldSectorId baseSector, AgentPosition currentPosition)
    {
        var previousSectorPosition = GetSectorPosition(baseSector, currentPosition.SectorId);
        return new AgentPosition
        {
            SectorId = baseSector,
            X = currentPosition.X + previousSectorPosition.X,
            Y = currentPosition.Y + previousSectorPosition.Y,
            Z = currentPosition.Z + previousSectorPosition.Z
        };
    }

    public static bool TryNormalizePosition(AgentPosition currentPosition, out AgentPosition newPosition)
    {
        throw new NotImplementedException();
    }

    public static WorldSectorId[] GetNearSectors(AgentPosition currentPosition)
    {
        throw new NotImplementedException();
    }
}
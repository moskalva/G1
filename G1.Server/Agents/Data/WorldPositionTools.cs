
using System.Buffers.Binary;

namespace G1.Server.Agents;

public static class WorldPositionTools
{
    private static readonly double SectorSize = Settings.SectorSize;
    private static readonly double VisibilityDistance = SectorSize * 0.6;
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
        newPosition = currentPosition;
        var distanceToSector = GetDistanceToSector(currentPosition);

        var candidates = GetSectorsAround(currentPosition.SectorId);
        foreach (var candidate in candidates)
        {
            var distanceToCandidate = GetDistance(currentPosition, GetSectorPosition(currentPosition.SectorId, candidate));
            if (distanceToCandidate < distanceToSector)
            {
                newPosition = RelativePosition(candidate, currentPosition);
            }
        }

        TryNormalizePosition(newPosition, out newPosition);
        return !newPosition.Equals(currentPosition);
    }

    public static WorldSectorId[] GetNearSectors(AgentPosition currentPosition)
    {
        var candidates = from candidate in GetSectorsAround(currentPosition.SectorId)
                         let candidatePosition = RelativePosition(candidate, currentPosition)
                         let distanceToCandidate = GetDistance(currentPosition, candidatePosition)
                         where distanceToCandidate <= VisibilityDistance
                         select candidate;

        return candidates.ToArray();
    }

    private static IEnumerable<WorldSectorId> GetSectorsAround(WorldSectorId baseSector)
    {
        var plusMinusOne = new int[] { 1, -1 };
        foreach (int x in plusMinusOne)
            foreach (int y in plusMinusOne)
                foreach (int z in plusMinusOne)
                {
                    yield return new WorldSectorId(
                        baseSector.SystemId,
                        baseSector.X + x,
                        baseSector.Y + y,
                        baseSector.Z + z);
                }
    }

    private static double GetDistance(AgentPosition position1, AgentPosition position2)
    {
        if (position1.SectorId != position2.SectorId)
        {
            position2 = RelativePosition(position1.SectorId, position2);
        }

        return Math.Sqrt(
            Math.Pow(position2.X - position1.X, 2) +
            Math.Pow(position2.Y - position1.Y, 2) +
            Math.Pow(position2.Z - position1.Z, 2));
    }

    private static double GetDistanceToSector(AgentPosition position)
    {
        return Math.Sqrt(Math.Pow(position.X, 2) + Math.Pow(position.Y, 2) + Math.Pow(position.Z, 2));
    }
}
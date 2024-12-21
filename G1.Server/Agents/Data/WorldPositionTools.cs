
using System.Buffers.Binary;
using G1.Model;

namespace G1.Server.Agents;

public static class WorldPositionTools
{
    private static readonly double SectorSize = WorldParameters.Physics.SectorSize;
    private static readonly double NearSectorDistance = SectorSize * 0.71;
    private static readonly double SectorSwitchDistance = (NearSectorDistance - SectorSize / 2) / 2 + SectorSize / 2;
    private static readonly double R = Math.Sqrt(Math.Pow(SectorSize, 2) - Math.Pow(SectorSize / 2, 2) * 2);
    // private static readonly double L = Math.Sqrt(Math.Pow(R, 2) - Math.Pow(SectorSize / 2, 2));
    // private static readonly double Q = SectorSize / 2 / Math.Sin(60 * (Math.PI / 180));
    // private static readonly double M = (Q * SectorSize / 2) / R;
    // private static readonly double K = (Q * L) / R;
    // private static readonly double W = Math.Sqrt(Math.Pow(SectorSize, 2) * 2) / 2;


    public static AgentPosition GetSectorPosition(WorldSectorId baseSector, WorldSectorId sector)
    {
        if (baseSector.SystemId != sector.SystemId)
            throw new InvalidOperationException($"Cannot calculate relative position of sectors in different systems");

        var diffX = sector.X - baseSector.X;
        var diffY = sector.Y - baseSector.Y;
        var diffZ = sector.Z - baseSector.Z;
        var x = diffX * SectorSize + diffZ * SectorSize / 2;
        var y = diffY * SectorSize + diffZ * SectorSize / 2;
        var z = diffZ * R;
        return new AgentPosition
        {
            SectorId = baseSector,
            X = (float)x,
            Y = (float)y,
            Z = (float)z,
        };
    }

    public static bool TryNormalizePosition(AgentPosition currentPosition, out AgentPosition newPosition)
    {
        var distanceToSector = GetDistanceToSector(currentPosition);
        if (distanceToSector < SectorSwitchDistance)
        {
            newPosition = currentPosition;
            return false;
        }

        newPosition = FindClosestAgentPosition(currentPosition);
        return !newPosition.Equals(currentPosition);

        AgentPosition FindClosestAgentPosition(AgentPosition currentPosition)
        {
            var newPosition = currentPosition;
            var distanceToSector = GetDistanceToSector(currentPosition);

            var candidates = GetSectorsAround(currentPosition.SectorId);
            foreach (var candidate in candidates)
            {
                var distanceToCandidate = GetDistance(currentPosition, GetSectorPosition(currentPosition.SectorId, candidate));
                if (distanceToCandidate < distanceToSector)
                {
                    newPosition = RelativePosition(candidate, currentPosition);
                    distanceToSector = distanceToCandidate;
                }
            }

            return newPosition.Equals(currentPosition) ? currentPosition : FindClosestAgentPosition(newPosition);
        }
    }

    public static WorldSectorId[] GetNearSectors(AgentPosition currentPosition)
    {
        var candidates = from candidate in GetSectorsAround(currentPosition.SectorId)
                         let candidatePosition = GetSectorPosition(currentPosition.SectorId, candidate)
                         let distanceToCandidate = GetDistance(currentPosition, candidatePosition)
                         where distanceToCandidate <= NearSectorDistance
                         select candidate;

        return candidates.Append(currentPosition.SectorId).ToArray();
    }

    public static AgentPosition GetReferencePosition(AgentPosition relativePosition)
    {
        var center = WorldSectorId.SystemCenter(relativePosition.SectorId.SystemId);
        return GetSectorPosition(center, relativePosition.SectorId);
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

    private static IEnumerable<WorldSectorId> GetSectorsAround(WorldSectorId baseSector)
    {
        var plusMinusOne = new int[] { 2, 1, 0, -1, -2 };
        foreach (int x in plusMinusOne)
            foreach (int y in plusMinusOne)
                foreach (int z in plusMinusOne)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    yield return new WorldSectorId(
                        baseSector.SystemId,
                        baseSector.X + x,
                        baseSector.Y + y,
                        baseSector.Z + z);
                }
    }

    public static double GetDistance(AgentPosition position1, AgentPosition position2)
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
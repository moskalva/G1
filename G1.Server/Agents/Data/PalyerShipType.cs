
namespace G1.Server.Agents;

public enum ShipType { Mark1 }

[GenerateSerializer, Alias(nameof(PalyerShipType))]
public class PalyerShipType
{
    [Id(0)]
    public Guid Id { get; set; }

    public ShipType ShipType { get; set; }
}
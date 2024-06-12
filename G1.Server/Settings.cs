namespace G1.Server;

public class Settings
{
    public static readonly int OutgoingConnectionBufferSize = 1024 * 4;
    public static readonly int IncomingConnectionBufferSize = 1024 * 4;
    public static readonly TimeSpan IdleClientTimeout = TimeSpan.FromSeconds(20);
    public static readonly TimeSpan StateCheckTimeout = TimeSpan.FromSeconds(1);
    public static readonly TimeSpan EmptyWriteQueueTimeout = TimeSpan.FromMicroseconds(500);
    public static readonly int SectorSize = 100_000_000;
}
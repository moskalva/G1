namespace G1.Server;

public class Settings
{
    public static readonly int OutgoingConnectionBufferSize = 1024 * 4;
    public static readonly int IncomingConnectionBufferSize = 1024 * 4;
    public static readonly TimeSpan IdleClientTimeout = TimeSpan.FromSeconds(30);
    public static readonly TimeSpan HeartBeatInterval = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan EmptyWriteQueueTimeout = TimeSpan.FromMicroseconds(500);
}
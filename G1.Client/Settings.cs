using System;

public static class Settings{

	public static readonly TimeSpan HeartBeatInterval = TimeSpan.FromSeconds(10);
	public static string WebSocketURLFormat { get; set; } = "ws://localhost:9080/ws/{0}/client";
	public static readonly TimeSpan ConnectionWaitTime = TimeSpan.FromSeconds(5);
}
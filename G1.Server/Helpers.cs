
namespace G1.Server;

public static class Helpers
{
    public static bool IsClientConnection(string requestPath, out string clientId)
    {
        const string prefix = "/ws/";
        const string ending = "/client";
        const int minimalLength = 3 + 8; // prefix.Length + ending.Length
        if (requestPath.StartsWith(prefix) && requestPath.EndsWith(ending) && requestPath.Length > minimalLength)
        {
            clientId = requestPath.Substring(prefix.Length, requestPath.Length - minimalLength);
            return true;
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        clientId = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        return false;
    }
}
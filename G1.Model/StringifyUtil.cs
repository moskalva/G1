using System.Text.Json;

namespace G1.Model;

public static class StringifyUtil
{
    public static string Stringify<T>(this T obj) => JsonSerializer.Serialize<T>(obj);
}
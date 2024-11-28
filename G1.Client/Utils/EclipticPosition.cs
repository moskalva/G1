using System;
using G1.Model;
using Godot;

/// <summary>
/// Represents position of object in Ecliptic heliocentric coordinates.
/// </summary>
public class EclipticPosition
{
    /// <summary>
    /// Angle between object and primary direction along ecliptic. Can be 0 -360 degrees.
    /// </summary>
    public double Longitude { get; set; }
    /// <summary>
    /// Angle between object and ecliptic towards poles. Can be -90 - 90 degrees.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Distance of an object from the barucenter of the star(s).
    /// </summary>
    public ulong Radius { get; set; }

    public override string ToString() => $"R:{Radius}, β: {Latitude}, λ: {Longitude}";

    public static EclipticPosition Get(Vector3I referencePoint, Vector3 position)
    {
        const double degree = 180 / Math.PI;
        var x = (double)referencePoint.X + position.X;
        var y = (double)referencePoint.Y + position.Y;
        var z = (double)referencePoint.Z + position.Z;
        var shadow = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2));

        var radius = (ulong)Math.Abs(Math.Sqrt(Math.Pow(shadow, 2) + Math.Pow(y, 2)));
        var latitude = Math.Atan(y / shadow);
        var longitude = Math.Abs(Math.Atan(x / z));
        longitude = x > 0 ?
            z > 0 ? Math.PI - longitude : longitude :
            z > 0 ? Math.PI + longitude : -longitude;

        return new EclipticPosition
        {
            Radius = radius,
            Latitude = double.IsNaN(latitude) ? 0 : degree * latitude,
            Longitude = double.IsNaN(longitude) ? 0 : degree * longitude,
        };
    }
}

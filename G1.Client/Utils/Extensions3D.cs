using Godot;

public static class Extensions3D
{
	public static Transform3D Teleported(this Transform3D transform, Vector3 position)
	{
		return new Transform3D(transform.Basis, position);
	}

	public static Transform3D RotatedAs(this Transform3D transform, Basis basis)
	{
		return new Transform3D(basis, transform.Origin);
	}

	public static Transform3D LookTowards(this Transform3D transform, Vector3 direction)
	{
		var up = direction == Vector3.Up ? Vector3.Back
			   : direction == Vector3.Down ? Vector3.Forward
			   : Vector3.Up;
		return transform.RotatedAs(Basis.LookingAt(direction, up));
	}
}
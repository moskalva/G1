using Godot;

public static class Extensions3D
{
	public static Transform3D Teleport(this Transform3D transform, Vector3 position)
	{
		return new Transform3D(transform.Basis, position);
	}

    
	public static Transform3D Rotate(this Transform3D transform, Basis basis)
	{
		return new Transform3D(basis, transform.Origin);
	}
}
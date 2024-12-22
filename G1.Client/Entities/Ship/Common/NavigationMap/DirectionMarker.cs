using Godot;
using System;

public partial class DirectionMarker : Node
{
	[Export]
	public float MinHeight { get; set; } = 5;
	[Export]
	public float MinDistance { get; set; } = 500;
	[Export]
	public float MaxHeight { get; set; } = 20;
	[Export]
	public float MaxDistance { get; set; } = 10_000;

	[Export]
	public float Radius { get; set; } = 30;


	private Node3D marker;
	private CylinderMesh makerMesh;
	private float scale;

	public override void _EnterTree()
	{
		this.marker = GetNode<Node3D>("Container");
		this.makerMesh = GetNode<MeshInstance3D>("Container/MarkerMesh").Mesh as CylinderMesh;
	}

	public override void _Ready()
	{
		this.scale = (MaxHeight - MinHeight) / (MaxDistance - MinDistance);
	}

	public void Update(Vector3 targetDirection, float distanceToTarget)
	{
		var reverseDistance = MaxDistance - distanceToTarget; // closer is bigger line
		var notmalizedDistance = reverseDistance < MinDistance ? MinDistance
								   : reverseDistance > MaxDistance ? MaxDistance
								   : reverseDistance;
		var height = scale * notmalizedDistance + MinHeight;

		makerMesh.Height = height;
		this.marker.Transform = Transform3D.Identity
			.LookTowards(targetDirection)
			.TranslatedLocal(new Vector3 { Z = -1 * (Radius + height / 2) });
	}
}

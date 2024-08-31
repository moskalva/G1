using Godot;

public enum CharacterPosture { Standing, Sitting }
public enum ViewType { FirstPerson, ThirdPerson }
public class PlayerStateProperties
{
    public CharacterPosture CharacterPosture { get; private set; }
    public Transform3D? CharacterPosition { get; private set; }
    public ViewType ViewType { get; private set; }

    public static PlayerStateProperties WorkDesk(Transform3D position) => new PlayerStateProperties
    {
        ViewType = ViewType.FirstPerson,
        CharacterPosture = CharacterPosture.Sitting,
        CharacterPosition = position,
    };

    public static PlayerStateProperties ControlPanel(Transform3D position) => new PlayerStateProperties
    {
        ViewType = ViewType.FirstPerson,
        CharacterPosture = CharacterPosture.Standing,
        CharacterPosition = position,
    };

    public static PlayerStateProperties Free() => new PlayerStateProperties
    {
        ViewType = ViewType.ThirdPerson,
        CharacterPosture = CharacterPosture.Standing,
        CharacterPosition = null,
    };

    public bool IsCharacterInState(Character character) => 
        character.Posture == this.CharacterPosture && 
        (!this.CharacterPosition.HasValue ||
            (character.Transform.Origin.IsEqualApprox(this.CharacterPosition.Value.Origin) &&
            character.Basis.IsEqualApprox(this.CharacterPosition.Value.Basis)));
}
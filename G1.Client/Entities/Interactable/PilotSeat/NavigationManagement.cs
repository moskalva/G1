using Godot;

public partial class NavigationManagement : ShipManagement
{
    private NavigationMap navigationMap;

    public override void _Ready()
    {
        base._Ready();
        var ship = this.GetAccendant<BaseShip>();
        this.Viewport = ship.NavigationMapView;
        this.navigationMap = ShipSystems.GetRegistered<NavigationMap>(this);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsAction("PilotSeat.Navigation.MoveLeft", true))
        {
            this.navigationMap.MoveCamera(Vector2.Left);
        }
        else if (@event.IsAction("PilotSeat.Navigation.MoveRight", true))
        {
            this.navigationMap.MoveCamera(Vector2.Right);
        }
        else if (@event.IsAction("PilotSeat.Navigation.MoveUp", true))
        {
            this.navigationMap.MoveCamera(Vector2.Down);
        }
        else if (@event.IsAction("PilotSeat.Navigation.MoveDown", true))
        {
            this.navigationMap.MoveCamera(Vector2.Up);
        }
        else if (@event.IsAction("PilotSeat.Navigation.MoveFront", true))
        {
            this.navigationMap.ZoomCamera(true);
        }
        else if (@event.IsAction("PilotSeat.Navigation.MoveBack", true))
        {
            this.navigationMap.ZoomCamera(false);
        }
    }
}


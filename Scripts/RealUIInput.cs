using Godot;
using System;

public partial class RealUIInput : Node
{
    [Signal]
    public delegate void OpenChangedEventHandler(bool openChanged);

    [Export]
    Node3D interactable;
    [Export]
    Control overlay;
    [Export]
    Camera3D screenCam;

    PlayerController player;
    public override void _Ready()
    {
        player = GameplayManager.Player;
        GameplayManager.AddTaskOverlay(overlay);
        overlay.Visible = false;
    }

    public void EnableInteraction()
    {
        screenCam.MakeCurrent();
        GameplayManager.SetMouseVisible(true);
        GameplayManager.Player.InputActive = false;
        GameplayManager.HudVisible = false;
        overlay.Visible = true;
        InputActive = true;
        if (interactable is not null)
            interactable.Visible = false;
        EmitSignal(SignalName.OpenChanged, true);
    }

    public void DisableInteraction()
    {
        player.MakeCamCurrent();
        GameplayManager.SetMouseVisible(false);
        GameplayManager.Player.InputActive = true;
        GameplayManager.HudVisible = true;
        overlay.Visible = false;
        InputActive = false;
        if (interactable is not null)
            interactable.Visible = true;
        EmitSignal(SignalName.OpenChanged, false);
    }

    bool InputActive = false;
    bool InputAllowed => InputActive && GameplayManager.GameRunning;
    public override void _Input(InputEvent @event)
    {
        if (!InputAllowed)
            return;
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Keycode == Key.Escape)
            {
                DisableInteraction();
                GetViewport().SetInputAsHandled();
                return;
            }
        }
    }
}

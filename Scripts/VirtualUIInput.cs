using Godot;
using System;

public partial class VirtualUIInput : Node
{
    [Export]
    Node3D interactable;
    [Export]
    SubViewport reciever;
    [Export]
    Control cursor;
    [Export]
    Camera3D screenCam;
    [Export]
    AudioStreamPlayer3D mouseClick;
    Vector2 vMousePos;

    [Export]
    public bool InputActive { get; set; }

    public override void _Ready() {
        vMousePos = reciever.Size / 2;
        cursor.Position = vMousePos;
    }


    public void EnableInteraction()
    {
        InputActive = true;
        screenCam.MakeCurrent();
        GameplayManager.Player.InputActive = false;
        GameplayManager.HudVisible = false;
        if(interactable is not null)
            interactable.Visible = false;
    }

    public void DisableInteraction()
    {
        InputActive = false;
        GameplayManager.Player.MakeCamCurrent();
        GameplayManager.Player.InputActive = true;
        GameplayManager.HudVisible = true;
        if (interactable is not null)
            interactable.Visible = true;
    }

    bool InputAllowed => InputActive && GameplayManager.GameRunning;
    public override void _Input(InputEvent @event)
    {
        if (!InputAllowed)
            return;
        if (@event is InputEventMouseButton buttonEvent)
        {
            //modify and pass
            buttonEvent.Position = vMousePos;
            reciever.PushInput(buttonEvent);
            GetViewport().SetInputAsHandled();
            if (buttonEvent.ButtonIndex == MouseButton.Left && buttonEvent.ButtonMask.HasFlag(MouseButtonMask.Left))
                mouseClick?.Play();
            return;
        }
        if (@event is InputEventMouseMotion motionEvent)
        {
            //modify and pass
            vMousePos += motionEvent.Relative;
            vMousePos.X = Mathf.Clamp(vMousePos.X, 0, reciever.Size.X-4);
            vMousePos.Y = Mathf.Clamp(vMousePos.Y, 0, reciever.Size.Y-4);
            motionEvent.Position = vMousePos;
            cursor.Position = vMousePos;
            reciever.PushInput(motionEvent);
            GetViewport().SetInputAsHandled();
            return;
        }
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Keycode == Key.Escape)
            {
                DisableInteraction();
                GetViewport().SetInputAsHandled();
                return;
            }
            //pass
            reciever.PushInput(keyEvent);
            GetViewport().SetInputAsHandled();
            return;
        }
    }
}

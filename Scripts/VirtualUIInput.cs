using Godot;
using System;

public partial class VirtualUIInput : Node
{
    [Export]
    PlayerController player;
    [Export]
    SubViewport reciever;
    [Export]
    Control cursor;
    [Export]
    Camera3D screenCam;
    Vector2 vMousePos;

    [Export]
    public bool InputActive { get; set; }

	public override void _Ready() {
        vMousePos = reciever.Size / 2;
        cursor.Position = vMousePos;
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public void EnableInteraction()
    {
        InputActive = true;
        screenCam.MakeCurrent();
        player.InputActive = false;
        player.hud.Visible = false;
    }

    public void DisableInteraction()
    {
        InputActive = false;
        player.MakeCamCurrent();
        player.InputActive = true;
        player.hud.Visible = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (!InputActive)
            return;
        if (@event is InputEventMouseButton buttonEvent)
        {
            //modify and pass
            buttonEvent.Position = vMousePos;
            reciever.PushInput(buttonEvent);
            GetViewport().SetInputAsHandled();
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

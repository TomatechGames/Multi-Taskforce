using Godot;
using System;

public partial class InteractionTarget : Area3D
{
    [Signal]
    public delegate void InteractionTriggeredEventHandler();
    [Signal]
    public delegate void HoverChangedEventHandler(bool hoverVal);

    [Export]
    float directionalTolorence = 90;

    public override void _Ready()
    {
        SetHovered(false);
    }

    public bool CanInteract(Vector3 incomingDirection)
    {
        Vector3 forward = -GlobalBasis.Z;
        float angle = Mathf.RadToDeg(forward.AngleTo(-incomingDirection));
        return angle <= directionalTolorence;
    }

    public void SetHovered(bool val)
    {
        EmitSignal(SignalName.HoverChanged, val);
    }

    public void TryInteract(Vector3 incomingDirection)
    {
        if (!CanInteract(incomingDirection))
            return;
        EmitSignal(SignalName.InteractionTriggered);
    }
}

using Godot;
using System;

public partial class TogglePassthrough : Node
{
    [Signal]
    public delegate void ToggleStateChangedEventHandler(bool toggleVal);
    [Export]
    bool toggleState;
	public bool ToggleState
	{
		get => toggleState;
		set
		{
			toggleState = value;
			EmitSignal(SignalName.ToggleStateChanged, toggleState);
		}
	}
    public override void _Ready()
    {
        EmitSignal(SignalName.ToggleStateChanged, toggleState);
    }
    public void Toggle()
	{
		ToggleState ^= true;
	}
}

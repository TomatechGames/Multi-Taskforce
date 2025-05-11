using Godot;
using System;

[GlobalClass]
public partial class PowerPanelTaskResource : TaskResource
{
    public override PackedScene TaskScene => GetTaskScene("PowerPanel");
}

using Godot;
using System;

[GlobalClass]
public partial class BasicTaskResource : TaskResource
{
    [Export]
    int taskDifficulty;
    public override PackedScene TaskScene => null;
}

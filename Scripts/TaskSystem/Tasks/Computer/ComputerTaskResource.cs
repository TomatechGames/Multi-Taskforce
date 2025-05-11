using Godot;
using System;

[GlobalClass]
public partial class ComputerTaskResource : TaskResource
{
    public override PackedScene TaskScene => GetTaskScene("Computer");
}

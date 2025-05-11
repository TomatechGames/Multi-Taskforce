using Godot;
using System;

[GlobalClass]
public partial class WorkDay : Resource
{
    [Export]
    public string title { get; private set; }
    [Export(PropertyHint.ArrayType)]
    public TaskResource[] tasks { get; private set; }
}

using Godot;
using System;

[GlobalClass]
public partial class WorkDay : Resource
{
    public string id => ResourcePath.Split('/')[^1].Replace(".tres", "");
    [Export]
    public bool unlockByDefault { get; private set; }
    [Export]
    public WorkDay toUnlock { get; private set; }
    [Export]
    public string title { get; private set; }
    [Export]
    public int rngSeed { get; private set; } = -1;
    [Export]
    public int timeScale { get; private set; } = 7;
    [Export]
    public int budget { get; private set; } = 1000;
    [Export(PropertyHint.ArrayType)]
    public TaskResource[] tasks { get; private set; }
}

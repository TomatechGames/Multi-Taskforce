using Godot;
using System;

public abstract partial class TaskResource : Resource
{
    public abstract PackedScene TaskScene { get; }
}

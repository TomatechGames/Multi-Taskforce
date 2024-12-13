using Godot;
using System;

public abstract partial class TaskResource : Resource
{
    public abstract PackedScene TaskScene { get; }
    public abstract void PrepareTask(Node rootNode);
}

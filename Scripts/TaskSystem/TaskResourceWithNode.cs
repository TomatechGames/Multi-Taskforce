using Godot;
using System;

public abstract partial class TaskResourceWithNode<T> : TaskResource where T : Node
{
    PackedScene taskScene;
    public override PackedScene TaskScene => taskScene ??= (PackedScene)ResourceLoader.Load($"res://Scenes/Tasks/{nameof(T)}", "PackedScene");
    public sealed override void PrepareTask(Node rootNode)
    {
        if (rootNode is T taskNode)
            PrepareTask(taskNode);
    }
    public abstract void PrepareTask(T taskNode);
}

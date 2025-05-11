using Godot;

[GlobalClass]
public abstract partial class TaskResource : Resource
{
    protected static PackedScene GetTaskScene(string keyword) => 
        ResourceLoader.Load<PackedScene>($"res://Scenes/Tasks/{keyword}/{keyword}Task.tscn");
    public abstract PackedScene TaskScene { get; }
}

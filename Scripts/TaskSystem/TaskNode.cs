using Godot;
using System.Collections.Generic;

public abstract partial class TaskNode : Node3D
{
    public abstract string TaskId { get; }
    public virtual string[] TaskDependancies => null;
	protected bool taskStarted { get; private set; } = false;
    public sealed override void _Ready() {}

    public abstract void PrepareTask(TaskResource config, Dictionary<string, TaskNode> dependancies);
    public abstract void StartTask();
	public abstract void TickTask();
}

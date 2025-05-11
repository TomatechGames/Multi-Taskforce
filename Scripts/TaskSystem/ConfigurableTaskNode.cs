using Godot;
using System;
using System.Collections.Generic;


public abstract partial class ConfigurableTaskNode<T> : TaskNode where T:TaskResource
{
    public sealed override void PrepareTask(TaskResource config, Dictionary<string, TaskNode> dependancies)
    {
        this.config = config is T typedConfig ? typedConfig : null;
        PrepareTask(dependancies);
    }

    protected T config {  get; private set; }

    protected abstract void PrepareTask(Dictionary<string, TaskNode> dependancies);
}
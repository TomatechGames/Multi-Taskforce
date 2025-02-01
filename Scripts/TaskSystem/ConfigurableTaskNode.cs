using Godot;
using System;
using System.Collections.Generic;


public abstract partial class ConfigurableTaskNode<T> : TaskNode where T:TaskResource
{
    protected sealed override void PrepareTask(TaskResource config, Dictionary<string, TaskNode> dependancies)
    {
		ConfigureTask(config is T typedConfig ? typedConfig : null, dependancies);
    }

    protected abstract void ConfigureTask(T config, Dictionary<string, TaskNode> dependancies);
}
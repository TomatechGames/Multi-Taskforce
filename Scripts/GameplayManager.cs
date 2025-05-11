using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameplayManager : Node
{
    public const int TicksPerSecond = 20;
    public const double TickCooldown = 1.0 / TicksPerSecond;
    static GameplayManager instance;

	static WorkDay currentWorkDay;
	//only editable while gameplay manager is null; ie. when not in a day
	public static WorkDay CurrentWorkDay
	{
		get => currentWorkDay;
		set
		{
			if (instance is null)
				currentWorkDay = value;
		}
	}

	[Export]
	WorkDay debugWorkDay;
	[Export]
	Node3D taskParent;
	[Export]
	PlayerController player;
	[Export]
	Control hud;
	[Export]
	Control taskUIParent;
	public static PlayerController Player => instance?.player;
	List<TaskNode> activeTasks = new();

	int currentBudget = 0;

	public override async void _Ready()
	{
		instance = this;
		currentWorkDay ??= debugWorkDay;
        Input.MouseMode = Input.MouseModeEnum.Captured;

        Dictionary<string, TaskNode> taskDict = new();
        Dictionary<string, TaskResource> taskResourceDict = new();

        foreach (var taskResource in CurrentWorkDay.tasks)
		{
			var node = taskResource.TaskScene.Instantiate<TaskNode>();
			var id = node.TaskId;
			taskDict[id] = node;
			taskResourceDict[id] = taskResource;
			activeTasks.Add(node);
			taskParent.AddChild(node);
			GD.Print("loading "+id);
		}

		foreach (var task in activeTasks)
		{
			TaskResource taskRes = null;
			taskResourceDict.TryGetValue(task.TaskId, out taskRes);
            var taskDeps = task.TaskDependancies?.ToDictionary(tid => tid, tid => taskDict.TryGetValue(tid, out var dep) ? dep : null);
            task.PrepareTask(taskRes, taskDeps);
			GD.Print("preparing "+ task.TaskId);
            await Helpers.WaitForFrame();
        }
        foreach (var task in activeTasks)
        {
            task.StartTask();
            GD.Print("starting " + task.TaskId);
            await Helpers.WaitForFrame();
        }
		gameRunning = true;
    }

	bool gameRunning = false;
	public static bool GameRunning => instance?.gameRunning ?? false;

	public static bool HudVisible
	{
		get => instance?.hud.Visible ?? false;
		set
		{
			if (instance is null)
				return;
			instance.hud.Visible = value;
		}
	}

    double currentTickCooldown = 0;
    public override void _Process(double delta)
    {
		if(!gameRunning)
			return;
		currentTickCooldown += delta;
		while (currentTickCooldown > TickCooldown)
		{
			currentTickCooldown -= TickCooldown;
            foreach (var task in activeTasks)
            {
                task.TickTask();
            }
        }
    }

    public override void _ExitTree()
    {
		instance = null;
    }

	public static void AddTaskOverlay(Control overlay)
    {
		if (instance is null)
			return;
		if (overlay.GetParent() is not null)
			overlay.Reparent(instance.taskUIParent);
		else
			instance.taskUIParent.AddChild(overlay);
    }

	public static void SetMouseVisible(bool visible)
	{
		if (instance is null || !GameRunning)
			return;
        Input.MouseMode = visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }

	public static void ChangeBudget(int offset)
	{
		if (instance is null || !GameRunning)
			return;
		instance.currentBudget += offset;
		if (instance.currentBudget < 0)
		{
			//game over
		}
	}
}

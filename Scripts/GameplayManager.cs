using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.Time;

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
    Control playerHud;
    [Export]
    Label budgetLabel;
    [Export]
    Label timeLabel;
    [Export]
    Control failOverlay;
    [Export]
    Control winOverlay;
    [Export]
    Control nextWorkDayButton;
    [Export]
    Control highscoreMsg;
    [Export]
    Control taskUIParent;
    public static PlayerController Player => instance?.player;

    public static RandomNumberGenerator rng { get; private set; } = new();

    List<TaskNode> activeTasks = [];

    int currentBudget = 0;

    public override async void _Ready()
    {
        instance = this;
        currentWorkDay ??= debugWorkDay;
        currentBudget = currentWorkDay.budget;
        if (currentWorkDay.rngSeed >= 0)
            rng.Seed = (uint)currentWorkDay.rngSeed;
        budgetLabel.Text = $"€{currentBudget}";
        Input.MouseMode = Input.MouseModeEnum.Captured;

        Dictionary<string, TaskNode> taskDict = [];
        Dictionary<string, TaskResource> taskResourceDict = [];

        foreach (var taskResource in CurrentWorkDay.tasks)
        {
            var node = taskResource.TaskScene.Instantiate<TaskNode>();
            var id = node.TaskId;
            taskDict[id] = node;
            taskResourceDict[id] = taskResource;
            activeTasks.Add(node);
            taskParent.AddChild(node);
            GD.Print("loading " + id);
        }

        foreach (var task in activeTasks)
        {
            TaskResource taskRes = null;
            taskResourceDict.TryGetValue(task.TaskId, out taskRes);
            var taskDeps = task.TaskDependancies?.ToDictionary(tid => tid, tid => taskDict.TryGetValue(tid, out var dep) ? dep : null);
            task.PrepareTask(taskRes, taskDeps);
            GD.Print("preparing " + task.TaskId);
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

    public static bool PlayerHudVisible
    {
        get => instance?.playerHud.Visible ?? false;
        set
        {
            if (instance is null)
                return;
            instance.playerHud.Visible = value;
        }
    }

    double currentTickCooldown = 0;
    int currentTick = 0;
    public override void _Process(double delta)
    {
        if (!gameRunning)
            return;
        currentTickCooldown += delta;
        while (currentTickCooldown > TickCooldown)
        {
            currentTickCooldown -= TickCooldown;
            foreach (var task in activeTasks)
            {
                task.TickTask();
            }

            int time = (currentTick / TicksPerSecond) * currentWorkDay.timeScale;
            time += 60 * 9;
            timeLabel.Text = $"Time: {time / 60}:{(time % 60 < 10 ? "0" : "")}{time % 60}";
            if (time >= 60 * 17)
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
                gameRunning = false;
                playerHud.Visible = false;
                taskUIParent.Visible = false;
                winOverlay.Visible = true;
                ConfigFile configFile = new();
                configFile.Load("user://progress.cfg");
                bool wasComplete = configFile.GetValue(currentWorkDay.id, "complete", false).AsBool();
                configFile.SetValue(currentWorkDay.id, "complete", true);
                if (currentWorkDay.toUnlock is WorkDay next)
                    configFile.SetValue(next.id, "unlocked", true);
                var highscore = configFile.GetValue(currentWorkDay.id, "highscore", 0).AsInt32();
                if (currentBudget > highscore)
                {
                    configFile.SetValue(currentWorkDay.id, "highscore", currentBudget);
                    highscoreMsg.Visible = true;
                }
                GD.Print(configFile.Save("user://progress.cfg"));
                foreach (var sec in configFile.GetSections())
                {
                    foreach (var key in configFile.GetSectionKeys(sec))
                    {
                        GD.Print($"{sec}.{key} = {configFile.GetValue(sec, key)}");
                    }
                }
                currentWorkDay = currentWorkDay.toUnlock;
                nextWorkDayButton.Visible = currentWorkDay is not null;
            }

            currentTick++;
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
        instance.budgetLabel.Text = $"€{instance.currentBudget}";

        if (instance.currentBudget < 0)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            instance.gameRunning = false;
            instance.playerHud.Visible = false;
            instance.taskUIParent.Visible = false;
            instance.failOverlay.Visible = true;
        }
    }
}

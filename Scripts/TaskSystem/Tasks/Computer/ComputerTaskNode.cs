using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class ComputerTaskNode : TaskNode
{
    [Export]
    PackedScene window;
    [Export]
    Control computerUI;
    [Export]
    Control blanket;
    Control windowParent;
    Control taskbarButtonParent;
    public override string TaskId => "Computer";
    public override string[] TaskDependancies => new string[] { "PowerPanel" };
    List<ComputerWindow> windows = new();

    PowerPanelTaskNode power;
    public override void PrepareTask(TaskResource config, Dictionary<string, TaskNode> dependancies)
    {
        windowParent = computerUI.GetNode<Control>("%WindowParent");
        taskbarButtonParent = computerUI.GetNode<Control>("%TaskbarButtonParent");
        power = dependancies["PowerPanel"] as PowerPanelTaskNode;
        power.PowerStateChanged += UpdateBlanket;
        UpdateBlanket();
    }

    public override void StartTask()
    {
        CreateWindow("Window One", w => w.CustomMinimumSize = new(235, 179));
    }

    void UpdateBlanket()
    {
        blanket.Visible = !power.HasPower;
    }

    public ComputerWindow CreateWindow(string title, Action<ComputerWindow> configureWindow = null, string taskbarText = null, bool noMinimise = false)
    {
        var newWindow = window.Instantiate<ComputerWindow>();
        windowParent.AddChild(newWindow);
        newWindow.WindowTitle = title;
        newWindow.IsMinimised = true;
        configureWindow?.Invoke(newWindow);
        var limit = windowParent.Size - newWindow.GetCombinedMinimumSize();
        newWindow.Position = new((float)GD.RandRange(0,limit.X),(float)GD.RandRange(0,limit.Y));
        windows.Add(newWindow);
        if (!noMinimise)
        {
            var button = new Button() { Text = taskbarText ?? title };
            button.Pressed += newWindow.ToggleMinimised;
            taskbarButtonParent.AddChild(button);
            newWindow.SetMeta("minButton", button);
        }
        else
        {
            newWindow.MinimiseBtn.Visible = false;
        }
        return newWindow;
    }

    public void RemoveWindow(ComputerWindow window)
    {
        if (!windows.Contains(window))
            return;
        if (window.HasMeta("minButton"))
        {
            var minBtn = window.GetMeta("minButton").As<Button>();
            minBtn.QueueFree();
        }
        window.QueueFree();
        windows.Remove(window);
    }
}

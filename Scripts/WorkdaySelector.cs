using Godot;
using System;

public partial class WorkdaySelector : Control
{
    [Export]
    WorkDay[] workDays;
    [Export]
    Control buttonParent;
    [Export]
    Control playPanel;
    [Export]
    Label dayName;
    [Export]
    Label highscore;
    ConfigFile configFile = new();

    public override void _Ready()
    {
        Visible = false;
        playPanel.Visible = false;
        configFile.Load("user://progress.cfg");
        foreach (var workday in workDays)
        {
            Button btn = new()
            {
                Text = workday.title,
                Disabled = !configFile.GetValue(workday.ResourceName, "unlocked", workday == workDays[0]).AsBool(),
            };
            buttonParent.AddChild(btn);
            var day = workday;
            btn.Pressed += () => SelectDay(day);
        }
    }

    public void SelectDay(WorkDay day)
    {
        GameplayManager.CurrentWorkDay = day;
        bool completed = configFile.GetValue(day.ResourceName, "complete", false).AsBool();
        highscore.Text = completed ? "No Highscore" : configFile.GetValue(day.ResourceName, "highscore", 0).AsInt32().ToString();
        dayName.Text = day.title;
        playPanel.Visible = true;
    }
}

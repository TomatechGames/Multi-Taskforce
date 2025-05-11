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
                Disabled = !(configFile.GetValue(workday.id, "unlocked", false).AsBool() || workday.unlockByDefault),
            };
            buttonParent.AddChild(btn);
            var day = workday;
            btn.Pressed += () => SelectDay(day);
        }

        foreach (var sec in configFile.GetSections())
        {
            foreach (var key in configFile.GetSectionKeys(sec))
            {
                GD.Print($"{sec}.{key} = {configFile.GetValue(sec, key)}");
            }
        }
    }

    public void SelectDay(WorkDay day)
    {
        GD.Print(day.id);
        GameplayManager.CurrentWorkDay = day;
        bool completed = configFile.GetValue(day.id, "complete", false).AsBool();
        highscore.Text = completed ? configFile.GetValue(day.id, "highscore", 0).AsInt32().ToString() : "No Highscore";
        dayName.Text = day.title;
        playPanel.Visible = true;
    }
}

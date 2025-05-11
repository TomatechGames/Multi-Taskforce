using Godot;
using System;

[GlobalClass]
public partial class PowerPanelTaskResource : TaskResource
{
    public override PackedScene TaskScene => GetTaskScene("PowerPanel");

    [Export]
    public int cooldownPenaltyPerMissing { get; private set; } = (GameplayManager.TicksPerSecond * 3) / 2;
    [Export]
    public int fullCooldown { get; private set; } = GameplayManager.TicksPerSecond * 5;
    [Export]
    public int costPenaltyPerMissing { get; private set; } = 20;
    [Export]
    public int drainPerEmptyPulse { get; private set; } = 3;
    [Export]
    public int failWeight { get; private set; } = 10;
    [Export]
    public int initialTargetWeight { get; private set; } = 1;
}

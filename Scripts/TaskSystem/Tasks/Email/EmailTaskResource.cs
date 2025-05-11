using Godot;
using System;

[GlobalClass]
public partial class EmailTaskResource : TaskResource
{
    public override PackedScene TaskScene => GetTaskScene("Email");

    [Export]
    public int minCooldown { get; private set; } = GameplayManager.TicksPerSecond * 10;
    [Export]
    public int maxCooldown { get; private set; } = GameplayManager.TicksPerSecond * 15;
    [Export]
    public int initialCooldown { get; private set; } = GameplayManager.TicksPerSecond * 10;
    [Export]
    public int penaltyThreshold { get; private set; } = GameplayManager.TicksPerSecond * 12;
    [Export]
    public int penaltyFrequency { get; private set; } = GameplayManager.TicksPerSecond / 2;
    [Export]
    public int penaltyAmount { get; private set; } = 1;
    [Export]
    public int completionThreshold { get; private set; } = 50;
}

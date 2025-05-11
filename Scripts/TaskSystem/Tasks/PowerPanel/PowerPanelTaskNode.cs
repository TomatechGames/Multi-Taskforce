using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PowerPanelTaskNode : ConfigurableTaskNode<PowerPanelTaskResource>
{
    public override string TaskId => "PowerPanel";
    public override string[] TaskDependancies => new string[] { "DeliveryBoxes" };

    [Export]
    DeliveryBox deliveryBox;
    [Export]
    BaseItem targetType;
    [ExportGroup("Meshes")]
    [Export]
    Node3D cover;
    [Export]
	Node3D[] fuseModels;
    Node3D[] fuseCores;
    [ExportGroup("Buttons")]
    [Export]
    Button[] fuseButtons;
    [Export]
    Button replenishButton;
    [ExportGroup("Sounds")]
    [Export]
    AudioStreamPlayer3D replenishSound;
    [Export]
    AudioStreamPlayer3D retrieveSound;
    [Export]
    AudioStreamPlayer3D breakSound;
    [Export]
    AudioStreamPlayer3D powerOffSound;
    [Export]
    AudioStreamPlayer3D powerOnSound;

    public event Action PowerStateChanged;
    bool hasPower = true;
    public bool HasPower => hasPower;
    protected override void PrepareTask(Dictionary<string, TaskNode> dependancies)
    {
        var deliveries = dependancies["DeliveryBoxes"] as DeliveryBoxesTaskNode;
        deliveries.RegisterDeliveryBox(deliveryBox);
        fuseCores = new Node3D[fuseModels.Length];
        fuseStates = new int[fuseModels.Length];
        for (int i = 0; i < fuseModels.Length; i++)
        {
            fuseCores[i] = fuseModels[i].GetChild<Node3D>(0);
            fuseStates[i] = 2;
            fuseButtons[i].Visible = false;
            int index = i;
            fuseButtons[i].Pressed += () => CollectFuse(index);
        }
        replenishButton.Pressed += ReplenishFuse;
        UpdateReplenishButton();
        cover.Scale = Vector3.One;
    }

    int[] fuseStates;

    public override void StartTask()
    {
        cooldown = config.fullCooldown/2;
    }

    public void TweenCover(bool open)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(cover, "scale", open ? new Vector3(1, 0.1f, 1) : Vector3.One, 0.1);
        UpdateReplenishButton();
    }

    void ValidateFuses()
    {
        bool newHasPower = fuseStates.Contains(2);
        if (newHasPower != hasPower)
        {
            hasPower = newHasPower;
            PowerStateChanged?.Invoke();
            var sound = hasPower ? powerOnSound : powerOffSound;
            sound?.Play();
        }
    }

    int cooldown = 0;
    int targetWeight = 1;
    public override void TickTask()
    {
        cooldown--;
        if (cooldown < 0)
        {
            var missing = fuseStates.Count(s => s != 2);
            if (missing == 2)
            {

            }
            cooldown = config.fullCooldown - (config.cooldownPenaltyPerMissing * missing);
            float targetPercent = targetWeight / (float)(targetWeight + config.failWeight);
            if (GameplayManager.rng.Randf() > targetPercent)
            {
                targetWeight++;
                return;
            }
            var targetIdx = GameplayManager.rng.RandiRange(0, fuseButtons.Length - 1);
            if (fuseStates[targetIdx] != 2)
                return;
            GameplayManager.ChangeBudget(-config.costPenaltyPerMissing * missing);
            targetWeight = 1;
            fuseStates[targetIdx] = 1;
            fuseCores[targetIdx].Visible = false;
            fuseButtons[targetIdx].Visible = true;
            ValidateFuses();
            breakSound?.Play();
        }
    }

    public void UpdateReplenishButton()
    {
        var amount = GameplayManager.Player.inventory.GetCount(targetType);
        replenishButton.Text = $"Replenish Fuse ({amount})";
        replenishButton.Disabled = amount == 0;
    }

    public void CollectFuse(int index)
    {
        if (fuseStates[index] != 1)
            return;
        fuseStates[index] = 0;
        fuseModels[index].Visible = false;
        fuseButtons[index].Visible = false;
        retrieveSound?.Play();
    }

    public void ReplenishFuse()
    {
        if (!GameplayManager.Player.inventory.HasItem(targetType))
            return;
        int targetIdx = -1;
        for (int i = 0; i < fuseStates.Length; i++)
        {
            if (fuseStates[i] == 0)
            {
                targetIdx = i;
                break;
            }
        }
        if (targetIdx == -1)
            return;
        GameplayManager.Player.inventory.TakeItem(targetType);
        fuseStates[targetIdx] = 2;
        fuseCores[targetIdx].Visible = true;
        fuseModels[targetIdx].Visible = true;
        replenishSound?.Play();
        UpdateReplenishButton();
        ValidateFuses();
        cooldown = config.fullCooldown;
    }
}

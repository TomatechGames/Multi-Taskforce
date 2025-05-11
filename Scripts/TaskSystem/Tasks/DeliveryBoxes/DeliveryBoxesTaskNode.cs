using Godot;
using System;
using System.Collections.Generic;

public partial class DeliveryBoxesTaskNode : TaskNode
{
    public override string TaskId => "DeliveryBoxes";
    public override string[] TaskDependancies => new string[] { "Computer" };

    [Export]
    float boxSpacing;
    [Export]
    PackedScene shopWindowScene;
    [Export]
    PackedScene shopEntryScene;

    ComputerTaskNode computerTask;
    public override void PrepareTask(TaskResource config, Dictionary<string, TaskNode> dependancies)
    {
        computerTask = dependancies["Computer"] as ComputerTaskNode;
    }

    Control shopEntryParent;
    public override void StartTask()
    {
        var shopWindow = shopWindowScene.Instantiate<Control>();
        var window = computerTask.CreateWindow("Scammazone", w => w.AddContent(shopWindow), "Shop");
        shopEntryParent = shopWindow.GetNode<Control>("%ShopEntryParent");
        foreach (var item in deliveryBoxes)
        {
            LinkBoxToShop(item);
        }
    }

    List<DeliveryBox> deliveryBoxes = new();

    public void RegisterDeliveryBox(DeliveryBox deliveryBox)
    {
        if (deliveryBox.GetParent() is not null)
            deliveryBox.Reparent(this);
        else
            AddChild(deliveryBox);
        deliveryBox.Position = new(deliveryBoxes.Count * boxSpacing, 0, 0);
        deliveryBox.Rotation = Vector3.Zero;
        deliveryBoxes.Add(deliveryBox);
        if (shopEntryParent is not null)
            LinkBoxToShop(deliveryBox);
    }

    void LinkBoxToShop(DeliveryBox deliveryBox)
    {
        var shopEntry = shopEntryScene.Instantiate<DeliveryBoxShopEntry>();
        shopEntryParent?.AddChild(shopEntry);
        deliveryBox.LinkShop(shopEntry);
    }
}

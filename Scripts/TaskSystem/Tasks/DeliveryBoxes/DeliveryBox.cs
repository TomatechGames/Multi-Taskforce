using Godot;
using System;

public partial class DeliveryBox : Node3D
{
    [Export]
    int itemCost = 1;

    [Export]
    int currentAmount = 0;

    [Export]
    Node3D[] itemRepresentations;

    [Export]
    BaseItem itemType;
    [Export]
    Node3D interactable;
    [Export]
    AudioStreamPlayer3D purchase;

    public override void _Ready()
    {
        SetAmount(currentAmount);
    }

    public void SetAmount(int amount)
    {
        GD.Print(currentAmount + "->" + amount);
        currentAmount = Mathf.Clamp(amount, 0, itemRepresentations.Length);
        for (int i = 0; i < itemRepresentations.Length; i++)
        {
            itemRepresentations[i].Visible = i < currentAmount;
        }
        if(interactable is not null)
            interactable.Visible = currentAmount > 0;
    }

    public void SetCost(int newCost)
    {
        itemCost = newCost;
        shopEntry?.SetCost(newCost);
    }

    DeliveryBoxShopEntry shopEntry;
    public void LinkShop(DeliveryBoxShopEntry newShopEntry)
    {
        if(shopEntry is not null)
            shopEntry.OnPurchaseRequested -= TryPurchase;
        shopEntry = newShopEntry;
        shopEntry.SetItem(itemType);
        shopEntry.SetCost(itemCost);
        shopEntry.OnPurchaseRequested += TryPurchase;
        shopEntry.SetInteractable(currentAmount < itemRepresentations.Length);
    }

    void TryPurchase()
    {
        if (currentAmount >= itemRepresentations.Length)
            return;
        GameplayManager.ChangeBudget(-itemCost);
        SetAmount(currentAmount+1);
        shopEntry.SetInteractable(currentAmount < itemRepresentations.Length);
        purchase?.Play();
    }

    public void HandleInteraction()
    {
        if (currentAmount > 0)
        {
            GameplayManager.Player.inventory.GiveItem(itemType, 1);
            SetAmount(currentAmount - 1);
        }
    }
}

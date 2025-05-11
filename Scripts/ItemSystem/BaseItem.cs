using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BaseItem : Resource
{
    [Export]
    string uniqueId;
    public string UniqueId => uniqueId;
    [Export]
    Texture2D icon;
    public Texture2D Icon => icon;
    [Export]
    string displayName;
    public string DisplayName => displayName;
    [Export]
    PackedScene heldModel;
    public PackedScene HeldModel => heldModel;
}

public class ItemInstance
{
    public BaseItem itemType { get; private set; }
    public int quantity { get; private set; }
    public bool valid => itemType != null && quantity > 0;

    public ItemInstance(BaseItem itemType, int quantity = 1)
    {
        this.itemType = itemType;
        this.quantity = quantity;
    }

    public ItemInstance TrySplit(int takeQuantity, int limitRemainder = -1) =>
        TryTake(takeQuantity, limitRemainder) ?
        new(itemType, takeQuantity) : null;

    public bool TryTake(int takeQuantity, int limitRemainder = -1)
    {
        if (!valid || (limitRemainder >= 0 && (quantity - takeQuantity) > limitRemainder))
            return false;
        quantity -= takeQuantity;
        return true;
    }

    public bool MergeTo(ItemInstance toItem) => toItem.MergeFrom(this);

    public bool MergeFrom(ItemInstance fromItem)
    {
        if (!valid || !fromItem.valid || fromItem.itemType!=itemType)
            return false;
        quantity += fromItem.quantity;
        fromItem.quantity = 0;
        return true;
    }
}

public class ItemContainer
{
    Dictionary<string, ItemInstance> items = new();

    public int GetCount(BaseItem itemType) =>
        items.TryGetValue(itemType.UniqueId, out var instance) ? instance.quantity : 0;
    public bool HasItem(BaseItem itemType, int minQuantity = 1) => 
        HasItemInternal(itemType, out _, minQuantity);

    bool HasItemInternal(BaseItem itemType, out ItemInstance instance, int minQuantity = 1)=>
        items.TryGetValue(itemType.UniqueId, out instance) &&
        instance.itemType == itemType &&
        instance.quantity >= minQuantity;

    public ItemInstance TakeItem(BaseItem itemType, int quantity = 1)
    {
        if(!HasItemInternal(itemType, out var instance, quantity))
            return null;
        if (quantity == instance.quantity)
        {
            items.Remove(itemType.UniqueId);
            return instance;
        }
        return instance.TrySplit(quantity);
    }

    public bool GiveItem(ItemInstance newInstance)
    {
        if (HasItemInternal(newInstance.itemType, out var existingInstance))
            existingInstance.MergeFrom(newInstance);
        else
            items.Add(newInstance.itemType.UniqueId, newInstance);
        return true;
    }

    public void GiveItem(BaseItem itemType, int quantity = 1)
    {
        if (quantity < 0)
            return;
        if (HasItemInternal(itemType, out var existingInstance))
            existingInstance.TryTake(-quantity);
        else
            items.Add(itemType.UniqueId, new(itemType, quantity));
    }
}

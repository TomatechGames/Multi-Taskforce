using Godot;
using System;

public partial class DeliveryBoxShopEntry : Control
{
    [Export]
    Label nameLabel;
    [Export]
    TextureRect iconTarget;
    [Export]
    Control iconContainer;
    [Export]
    Button buyButton;

    public override void _Ready()
    {
        buyButton.Pressed += Purchase;
    }

    public void SetItem(BaseItem itemType)
    {
        if (itemType is null)
            return;
        nameLabel.Text = itemType.DisplayName;
        bool hasIcon = itemType.Icon is not null;
        iconContainer.Visible = hasIcon;
        if (hasIcon)
            iconTarget.Texture = itemType.Icon;
    }

    public void SetInteractable(bool interactable) => buyButton.Disabled = !interactable;

    public void SetCost(int newCost) => buyButton.Text = "$" + newCost;

    public event Action OnPurchaseRequested;
    public void Purchase() => OnPurchaseRequested?.Invoke();
}

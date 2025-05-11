using Godot;
using System;

[GlobalClass]
public partial class DeliveryBoxesTaskResource : TaskResource
{
    public override PackedScene TaskScene => GetTaskScene("DeliveryBoxes");
}

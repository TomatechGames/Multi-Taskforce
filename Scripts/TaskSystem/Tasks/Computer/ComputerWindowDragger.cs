using Godot;

[GlobalClass]
public partial class ComputerWindowDragger : BaseButton
{
	[Export]
	Control target;
	Vector2 offset;
	bool dragging = false;
	public override void _Ready()
	{
		ButtonDown += () =>
		{
			offset = target.GlobalPosition - GetViewport().GetMousePosition();
			dragging = true;
		};
		ButtonUp += () =>
		{
			dragging = false;
            UpdateDragPos();
        };
	}

    public override void _Process(double delta)
    {
		if (dragging)
        {
			UpdateDragPos();
        }
    }

	void UpdateDragPos()
    {
        target.GlobalPosition = GetViewport().GetMousePosition() + offset;
		var limit = target.GetParent<Control>().Size;
		var pos = target.Position;
		var size = target.Size;
		pos.X = Mathf.Clamp(pos.X, 0, limit.X-size.X);
		pos.Y = Mathf.Clamp(pos.Y, 0, limit.Y - size.Y);
		target.Position = pos;
    }
}

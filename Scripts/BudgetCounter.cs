using Godot;

public partial class BudgetCounter : Label
{
	public static BudgetCounter Instance;
	[Export]
	int currentVal = 500;
    [Export]
    PlayerController playerController;
    [Export]
    Control failOverlay;

    public bool isEmpty = false;

	public override void _Ready()
	{
		Instance = this;
		Text = $"Budget: ${currentVal}";
    }

	public void Drain(int amount)
	{
        if (isEmpty || DemoTimeCounter.Instance.IsComplete)
            return;
		currentVal -= amount;

        Text = $"Budget: ${currentVal}";

		if (currentVal < 0)
		{
			isEmpty = true;
            //game over

            Input.MouseMode = Input.MouseModeEnum.Visible;
            playerController.InputActive = false;
            failOverlay.Visible = true;
        }
    }
}

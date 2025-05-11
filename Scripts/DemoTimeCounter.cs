using Godot;
using System;

public partial class DemoTimeCounter : Label
{
	[Export]
	Control winOverlay;
	[Export]
    double rate = 0.1;
    [Export]
    int currentTime = 9 * 60;
    [Export]
    int targetTime = 17 * 60;

	public bool IsComplete => currentTime > targetTime;

    public override void _Ready()
    {
		deltaSum = rate;
    }

    double deltaSum = 0;
	public override void _Process(double delta)
	{
		deltaSum += delta;
		if (deltaSum >= rate)
		{
			while (deltaSum > rate)
			{
				deltaSum -= rate;
				//currentTime += 1;
            }
            string secs = (currentTime % 60).ToString();
            if (secs.Length == 1)
                secs = "0" + secs;
            Text = $"Current Time: {currentTime / 60}:{secs}";
			if (IsComplete)
				Complete();
        }
	}

	bool done = false;
	void Complete()
	{
		if (done)
			return;
		done = true;

        Input.MouseMode = Input.MouseModeEnum.Visible;
		winOverlay.Visible = true;
    }
}

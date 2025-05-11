using GdUnit4;
using static GdUnit4.Assertions;
using Godot;
using System;
using System.Threading.Tasks;

[TestSuite]
public partial class EmailTests
{
	[TestCase]
	public async Task TestCase()
	{
		GameplayManager.CurrentWorkDay = ResourceLoader.Load<WorkDay>("res://WorkDays/Gameplay/EasyEmails.tres");
		GameplayManager.rng.Seed = 0;
		var runner = ISceneRunner.Load("res://Scenes/Gameplay.tscn");
		runner.SetTimeFactor(10);
		var player = runner.FindChild("Player") as PlayerController;
		runner.SimulateMouseMove(new(15, 0));
		AssertBool(true);
	}
}

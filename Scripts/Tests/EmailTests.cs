using GdUnit4;
using static GdUnit4.Assertions;
using Godot;
using System;

[TestSuite]
public partial class EmailTests
{
	[TestCase]
	public async void TestCase()
	{
		GameplayManager.CurrentWorkDay = ResourceLoader.Load<WorkDay>("res://WorkDays/Gameplay/EasyEmails.tres");
		GameplayManager.rng.Seed = 0;
		var runner = ISceneRunner.Load("res://Scenes/gameplay.tscn");
		runner.SetTimeFactor(10);
		var player = runner.Invoke("find_child", "Player").As<PlayerController>();
		runner.SimulateMouseMove(new(15, 0));
		AssertBool(true);
	}
}

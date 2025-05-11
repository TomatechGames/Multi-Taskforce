using Godot;
using System;
using System.Threading.Tasks;

public static class Helpers
{
    static SceneTree MainLoopSceneTree => (SceneTree)Engine.GetMainLoop();
    public static async Task WaitForFrame() => await MainLoopSceneTree.WaitForFrame();
    static async Task WaitForFrame(this SceneTree sceneTree) =>
        await sceneTree.ToSignal(sceneTree, SceneTree.SignalName.ProcessFrame);

    public static async Task WaitForTimer(double time) => await MainLoopSceneTree.WaitForTimer(time);
    static async Task WaitForTimer(this SceneTree sceneTree, double time) => await sceneTree.CreateTimer(time).WaitForTimer();
    public static async Task WaitForTimer(this SceneTreeTimer timer) => await timer.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
}

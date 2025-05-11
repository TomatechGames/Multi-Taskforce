using Godot;
using System;

public partial class ServerInstance : Node
{
    [Export]
    Node3D goodMesh;
    [Export]
    Node3D badMesh;
    [Export]
    double rollColldown = 5;
    [Export]
    double drainColldown = 1;

    public bool isGood = true;

    double timeSinceMadeGood = 0;
    double timeSinceRolled = 5;
    double timeSinceDrained = 1;

    public override void _Ready()
    {
        goodMesh.Visible = true;
        badMesh.Visible = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        timeSinceMadeGood += delta;
        timeSinceRolled -= delta;
        if (timeSinceRolled < 0)
        {
            timeSinceRolled = rollColldown;
            float value = GD.Randf()*35;
            if (value < timeSinceMadeGood - 5)
            {
                isGood = false;
                goodMesh.Visible = false;
                badMesh.Visible = true;
            }
        }
        if (!isGood)
        {
            timeSinceDrained -= delta;
            if (timeSinceDrained < 0)
            {
                timeSinceDrained = drainColldown;
            }
        }
    }

    public void MakeGood()
    {
        isGood = true;
        goodMesh.Visible = true;
        badMesh.Visible = false;
        timeSinceMadeGood = 0;
    }
}

using Godot;
using System;

public partial class ServerRack : Node
{
	[Export(PropertyHint.ArrayType)]
	ServerInstance[] servers;

	public void FixOne()
	{
        foreach (var server in servers)
        {
            if (!server.isGood)
            {
                server.MakeGood();
                return;
            }
        }
    }
}

using Godot;
using System;

public partial class SceneNav : Node
{
    [Export(PropertyHint.File)]
    public string SceneFile { get; set; }

    public void LoadScene()
    {
        GetTree().ChangeSceneToFile(SceneFile);
    }
}

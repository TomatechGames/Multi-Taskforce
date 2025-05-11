using Godot;
using System;

public partial class EmailTaskNode : TaskNode
{
    public override string TaskId => "Email";

    static string[] fakeWords = ["╎𝙹⚍ ⍊⍑", "リ𝙹ꖎ⍑↸", "↸ᓭ𝙹⋮⎓ ᒲ;", "⍑╎𝙹", "∷⎓リ𝙹ꖎ⍑↸⍊", "⍑ʖ⋮ꖌ∷⚍", "ʖ⍊╎⚍", "ꖎ⍑", "⋮↸ᓭ𝙹⋮"];

    public override void StartTask()
    {
        throw new NotImplementedException();
    }
}

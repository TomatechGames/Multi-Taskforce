using Godot;
using System;

public partial class ComputerWindow : Control
{
    [Export]
    Control contentParent;
    public Control ContentParent => contentParent;
    [Export]
    Label titleLabel;
    [Export]
    Control minimiseBtn;
    public Control MinimiseBtn => minimiseBtn;
    public string WindowTitle
    {
        get => titleLabel.Text;
        set => titleLabel.Text = value;
    }

    bool hovered = false;
    public override void _Ready()
    {
        MouseEntered += () => hovered = true;
        MouseExited += () => hovered = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (hovered && @event is InputEventMouseButton buttonEvent && buttonEvent.ButtonIndex == MouseButton.Left && buttonEvent.ButtonMask.HasFlag(MouseButtonMask.Left))
        {
            MoveToFront();
        }
    }

    public void AddContent(Node content)
    {
        if (content.GetParent() is not null)
            content.Reparent(ContentParent);
        else
            ContentParent.AddChild(content);
    }

    bool minimised = false;
    public bool IsMinimised
    {
        get => minimised;
        set
        {
            if (minimised==value)
                return;
            minimised = value;
            Visible = !value;
            if(!minimised)
                MoveToFront();
            MinimisedChanged?.Invoke(minimised);
        }
    }
    public void ToggleMinimised() => IsMinimised = !IsMinimised;
    public event Action<bool> MinimisedChanged;
}

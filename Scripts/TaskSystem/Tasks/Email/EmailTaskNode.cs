using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EmailTaskNode : ConfigurableTaskNode<EmailTaskResource>
{
    public override string TaskId => "Email";
    public override string[] TaskDependancies => ["Computer"];

    [Export]
    Control emailWindowContent;
    [Export]
    Label fromLabel;
    [Export]
    RichTextLabel contentLabel;
    [Export]
    TextEdit responseBox;
    [Export]
    Control buttonParent;
    [Export]
    ProgressBar progressBar;
    [Export]
    Control responseLayout;
    [Export]
    AudioStreamPlayer3D notif;
    [Export]
    string[] fakeWords;

    ComputerTaskNode computer;
    protected override void PrepareTask(Dictionary<string, TaskNode> dependancies)
    {
        computer = dependancies["Computer"] as ComputerTaskNode;
        responseBox.TextChanged += UpdateProgress;
    }

    List<EmailData> emails = [];
    EmailData currentEmail;

    class EmailData
    {
        public Button listButton;
        public string sender;
        public string content;
        public string response;
        public int lifetime;
    }

    public override void StartTask()
    {
        computer.CreateWindow("Email", w => w.AddContent(emailWindowContent));
        cooldown = config.initialCooldown;
    }

    void SetEmail(EmailData email)
    {
        responseLayout.Visible = true;
        fromLabel.Text = email.sender;
        contentLabel.Text = email.content;
        responseBox.Editable = true;
        responseBox.Text = email.response;
        responseBox.GrabFocus();
        currentEmail = email;
        UpdateProgress();
    }

    void UpdateProgress()
    {
        if (responseBox.Text.Length > 20)
        {
            //prevents spamming the same keys
            var latestChars = responseBox.Text.ToCharArray()[^15..].Distinct();
            if (latestChars.Count() < 3)
            {
                responseBox.Text = responseBox.Text[..^15];
                responseBox.SetCaretColumn(responseBox.Text.Length);
            }
        }
        currentEmail.response = responseBox.Text;
        progressBar.Value = (float)currentEmail.response.Length / config.completionThreshold;
        if (progressBar.Value >= 1)
        {
            responseBox.Editable = false;
            emails.Remove(currentEmail);
            currentEmail.listButton.QueueFree();
            cooldown = Mathf.Max(config.minCooldown, cooldown);
        }
    }

    int cooldown = 0;

    public override void TickTask()
    {
        foreach (var email in emails)
        {
            email.lifetime++;
            if(email.lifetime> config.penaltyThreshold && email.lifetime % config.penaltyFrequency == 0)
            {
                GameplayManager.ChangeBudget(-config.penaltyAmount);
            }
        }
        cooldown--;
        if (cooldown < 0)
        {
            cooldown = GameplayManager.rng.RandiRange(config.minCooldown, config.maxCooldown);
            GenerateEmail();
        }
    }

    void GenerateEmail()
    {
        int senderId = GameplayManager.rng.RandiRange(100, 999); 
        EmailData newEmail = new()
        {
            sender = $"employee_{senderId}@mtfmail.com",
            content = $"{PickWords(2)}\n\n{PickWords(8)}\n\n{PickWords(2)}"
        };
        Button emailBtn = new()
        {
            Text = $"-> employee_{senderId}"
        };
        emailBtn.MouseFilter = Control.MouseFilterEnum.Pass;
        newEmail.listButton = emailBtn;
        emailBtn.AddThemeConstantOverride("font_size", 10);
        emailBtn.Pressed += () => SetEmail(newEmail);
        buttonParent.AddChild(emailBtn);
        emails.Add(newEmail);
        notif?.Play();
    }

    string PickWords(int amount)
    {
        List<string> words = [];
        for (int i = 0; i < amount; i++)
        {
            words.Add(fakeWords[GameplayManager.rng.RandiRange(0, fakeWords.Length - 1)]);
        }
        return string.Join("  ", words);
    }
}

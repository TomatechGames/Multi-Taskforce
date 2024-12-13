using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
	[Export]
	public Control hud;
	[Export]
	RayCast3D testRC;
	[Export]
	Camera3D camera;
	[Export]
	float topSpeed = 5.0f;
    [Export]
    float accelleration = 5.0f;
    [Export]
	float jumpVelocity = 4.5f;
	[Export]
	float lookSensitivity = 90;
	Vector3 currentLookRot;

    public override void _Ready()
    {
		currentLookRot.X = Mathf.RadToDeg(camera.Rotation.X);
        currentLookRot.Y = Mathf.RadToDeg(Rotation.Y);
		camera.MakeCurrent();
    }

	public void MakeCamCurrent() => camera.MakeCurrent();

	[Export]
	public bool InputActive { get; set; } = true;
    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if (!InputActive)
            return;
        if (inputEvent.IsActionPressed("interact"))
        {
            if (testRC.GetCollider() is InteractionTarget interactionTarget)
            {
                interactionTarget.TryInteract(-testRC.GlobalBasis.Z);
            }
			GetViewport().SetInputAsHandled();
			return;
        }
		if(inputEvent is InputEventMouseMotion mouseMovement)
		{
			var screenSize = GetWindow().Size;
            float smallestSize = Mathf.Min(screenSize.X, screenSize.Y);
			Vector2 lookDelta = mouseMovement.Relative * (1 / smallestSize) * lookSensitivity;
            currentLookRot -= new Vector3(lookDelta.Y, lookDelta.X, 0);
			currentLookRot.X = Mathf.Clamp(currentLookRot.X, -89, 89);
			currentLookRot.Y = ((currentLookRot.Y + 540) % 360) - 180;
			Rotation = Rotation with { Y = Mathf.DegToRad(currentLookRot.Y) };
			camera.Rotation = camera.Rotation with { X = Mathf.DegToRad(currentLookRot.X) };
            GetViewport().SetInputAsHandled();
        }
    }

	InteractionTarget currentlyHovered;
    public override void _Process(double delta)
    {
		if(testRC.GetCollider() is InteractionTarget interactionTarget)
		{
			if (interactionTarget != currentlyHovered)
			{
				currentlyHovered?.SetHovered(false);
				currentlyHovered = interactionTarget;
            }
            currentlyHovered?.SetHovered(currentlyHovered.CanInteract(-testRC.GlobalBasis.Z));
        }
		else if(currentlyHovered is not null)
		{
			currentlyHovered.SetHovered(false);
			currentlyHovered = null;
		}
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDir = InputActive ? Input.GetVector("move_left", "move_right", "move_forward", "move_backward") : Vector2.Zero;
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction == Vector3.Zero)
		{
			if (Velocity.Length() > 0.2f)
				direction = -(Velocity with { Y = 0 }).Normalized();
			else
				Velocity = Vector3.Zero;
		}
		Vector3 currentAccel = direction * accelleration;

		bool isOnFloor = IsOnFloor();
		if (!isOnFloor)
			currentAccel += GetGravity();

		Velocity += currentAccel * (float)delta * 0.5f;
        Velocity = Velocity.LimitLength(topSpeed);

        MoveAndSlide();
		//offset camera by -velocity

        Velocity += currentAccel * (float)delta * 0.5f;
        Velocity = Velocity.LimitLength(topSpeed);
    }
}

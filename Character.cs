using Godot;
using System;

public partial class Character : CharacterBody3D
{
	[Export] public const float Speed = 7.0f;
    [Export] public const float JumpVelocity = 10f;

	private AnimationTree animationTree;
    private AnimationNodeStateMachinePlayback stateMachine;
	private bool isDeath = false;
	private float deathTimer = 0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle() * 2;

	private bool isDoubleJumpAvailable = true;
	bool isJumping = false;

    public override void _Ready()
    {
		animationTree = GetNode<AnimationTree>("AnimationTree");
        stateMachine = (AnimationNodeStateMachinePlayback) animationTree.Get("parameters/playback");
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		if (isDeath)
		{
			deathTimer += (float) delta;

			if (deathTimer > 3) {
                GlobalPosition = new Vector3(-2.246f, 0.098f, 8.722f);
				stateMachine.Travel("Idle-Walk-Run");

				deathTimer = 0f;
                isDeath = false;
            } else
			{
                return;
            }
        }

		// Add the gravity
		if (!IsOnFloor())
		{
			velocity.Y -= gravity * (float) delta;
        } else
		{
			if (isJumping)
			{
				isJumping = false;
                stateMachine.Travel("Jump_Land");
            }
			isDoubleJumpAvailable = true;
		}

		// Handle Jump
		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || isDoubleJumpAvailable))
		{
            velocity.Y = JumpVelocity;
			isJumping = true;
            animationTree.Set("parameters/Jump_Idle/playback_speed", 5f);

            if (!IsOnFloor())
			{
                stateMachine.Travel("Jump_Idle");
                isDoubleJumpAvailable = false;
			} else
			{
                stateMachine.Travel("Jump_Start");
            }
        }

		// Get the input direction and handle the movement/deceleration
		// As good practice, you should replace UI actions with custom gameplay actions
		Vector2 inputDir = Input.GetVector("right", "left", "down", "up");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		animationTree.Set("parameters/Idle-Walk-Run/blend_position", new Vector2(-inputDir.X, inputDir.Y));

        if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

    public void _on_Area_body_entered(Node3D body)
    {
        if (body.Name == "Void")
		{
            GlobalPosition = new Vector3(-2.246f, 0.098f, 8.722f);
        }

        if (body.Name == "Pikudos")
        {
            isDeath = true;
            stateMachine.Travel("Death_A");
        }

        if (body.Name == "Garrafa")
		{
			body.QueueFree();
			GD.Print("Coletou uma garrafinha, amém 🙏");
		}
    }
}

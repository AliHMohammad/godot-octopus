using Godot;

public partial class Player : Area2D
{

    [Export]
    public int Speed { get; set; } = 400;

    public AnimatedSprite2D AnimatedSprite2D { get; private set; }

    public Vector2 ScreenSize;

    [Signal]
    public delegate void HitEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        Hide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        Vector2 velocity = Vector2.Zero;

        Move(delta, velocity);
    }

    public void Start(Vector2 position)
    {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    private void OnBodyEntered(Node2D body)
    {
        Hide(); // Player disappears after being hit.
        EmitSignal(SignalName.Hit);
        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }



    private void Move(double delta, Vector2 velocity)
    {


        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 1;
        }

        if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= 1;
        }

        if (Input.IsActionPressed("move_up"))
        {
            velocity.Y -= 1;
        }

        if (Input.IsActionPressed("move_down"))
        {
            velocity.Y += 1;
        }

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            AnimatedSprite2D.Play();
        }
        else
        {
            AnimatedSprite2D.Stop();
        }

        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );

        if (velocity.X != 0)
        {
            AnimatedSprite2D.Animation = "walk";
            AnimatedSprite2D.FlipV = false;
            // See the note below about boolean assignment.
            AnimatedSprite2D.FlipH = velocity.X < 0;
        }

        else if (velocity.Y != 0)
        {
            AnimatedSprite2D.Animation = "up";
            AnimatedSprite2D.FlipV = velocity.Y > 0;
        }
    }

}




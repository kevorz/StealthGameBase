using Godot;
using System;

public class Guard : Enemy
{
    public override void _Ready()
    {
        base._Ready();
        this.EnemyType = eEnemyType.SecurityGuard;
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }

}

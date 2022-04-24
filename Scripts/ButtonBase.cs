using Godot;
using System;

public class ButtonBase : InteractBase
{
    public String ButtonName = "ButtonBase";
    public override void _Ready()
    {
        base._Ready();
        this.PlayerOnly = true;
    }
    
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public override void StuffToDo()
    { 
        base.StuffToDo();
    }

    public override void _on_InteractArea_body_entered(Node body)
    {
        base._on_InteractArea_body_entered(body);
    }

    public override void _on_InteractArea_body_exited(Node body)
    {
        base._on_InteractArea_body_exited(body);
    }
}

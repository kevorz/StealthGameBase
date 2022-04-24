using Godot;
using System;

public class ButtonTest : ButtonBase
{
    [Export] string InteractionName = "Change me!";
    public override void StuffToDo()
    { 
        base.StuffToDo();
        GD.Print($"{ButtonName} StuffToDo() ran succesfully.");
    }
    public override void _Ready()
    {
        base._Ready();
        this.ButtonName = InteractionName;
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

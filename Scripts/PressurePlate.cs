using Godot;
using System;

public class PressurePlate : InteractBase
{
    public override void _Ready()
    {
        base._Ready();

        this.CanUse = true;
        this.IsDisabled = false;
    }

    public override void StuffToDo()
    {
        base.StuffToDo();
        GD.Print($"PressurePlate DoStuff");
        if (this.CanUse)
        {
            if (BeingUsed)
                return;
            BeingUsed = true;
            ShowInteractionWidget("DisableTrap", 2.0f);
            IsDisabled = true;
        }
    }
    public override void _on_TriggerArea_body_entered(Node body)
    {  
        base._on_TriggerArea_body_entered(body);
        if (body.GetType() != typeof(Player))
            return;
        GD.Print($"PressurePlate triggered. State Disabled = {IsDisabled}");
        if (!IsDisabled)
        {
            TriggerAlarm();
        }
    }
    public override void _on_TriggerArea_body_exited(Node body)
    {
        base._on_TriggerArea_body_exited(body);
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

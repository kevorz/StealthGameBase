using Godot;
using System;

public class InteractBase : Spatial
{
    public Area InteractArea;
    public Area TriggerArea;
    public ulong InstanceID;
    public bool BeingUsed = false;
    public bool CanUse = true;
    public bool IsDisabled = false;
    public bool PlayerOnly = false;
    [Export] public float InteractNoise = 5f;
    [Export] public float InteractionTime = 1f;
    public static Player player;
    public override void _Ready()
    {
        base._Ready();
        PlayerOnly = true;
        InteractArea = GetNode<Area>("InteractArea") ?? null;
        TriggerArea = GetNode<Area>("TriggerArea") ?? null;
        InstanceID = this.GetInstanceId();
        player = Owner.Owner.GetNode<Player>("PlayerManager/Player");
        
    }
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public void DoStuff()
    {
        player.IsInteracting = true;
        player.Idle(InteractionTime);
        StuffToDo();
        player.IsInteracting = false;
    }

    public virtual void StuffToDo()
    {
        //override & customize
    }
    public virtual void _on_InteractArea_body_entered(Node body)
    {
        if (body.GetType() == typeof(Player))
            {
                Player player = (Player)body;
                var temp = new Interactable();
                temp.Object = this;
                temp.InstanceID = this.InstanceID;
                temp.MyPosition = this.GlobalTransform.origin;

                player.InteractableListAdd(temp);
            }
            else
                return;
    }
    public virtual void _on_InteractArea_body_exited(Node body)
    {
        if (body.GetType() == typeof(Player))
        {
            Player player = (Player)body;
            player.InteractableListRemove(InstanceID);
        }
        else
            return;
    }

    public virtual void _on_TriggerArea_body_entered(Node body){}
    public virtual void _on_TriggerArea_body_exited(Node body){}

    public void TriggerAlarm()
    {
        GD.Print($"You triggered the alarm dummy!");
    }

    public virtual void ShowInteractionWidget(string widgettype)
    {
        switch (widgettype)
        {
            case "Lockpick":
            break;
        }
    }
    public virtual void ShowInteractionWidget(string widgettype, float waittime)
    {
        switch (widgettype)
        {
            case "DisableTrap":
                DisableTrap(waittime);
            break;
        }
    }

    public async void DisableTrap(float waittime)
    {
        GD.Print($"Disabling a trap (cant move now)");
        player.IsIdling = true;
        await ToSignal(GetTree().CreateTimer(waittime), "timeout");
        GD.Print($"Succesfully disabled trap (can move again!)");
        player.IsIdling = false;
    }
}

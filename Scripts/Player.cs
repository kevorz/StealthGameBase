using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Player : KinematicBody
{
    public List<Interactable> InteractablesInRange = new List<Interactable>();
    public bool PlayerCharacter = true;
    [Export] public float Speed = 10f;
    [Export] public float SneakSpeedFactor = 0.5f;
    [Export] public float Gravity = 70f;
    [Export] public float MovementNoise = 5f;
    [Export] public float SneakNoise = 1f;
    public float InteractNoise = 0f;
    public float PlayerNoise = 0f;
    public float Sneak = 1f;
    public bool IsSneaking = false;
    public bool IsInteracting = false;
    public bool IsIdling = false;
    public Vector3 InputVector;
    public Vector3 Velocity;
    public Spatial Pivot;
    public Spatial tmp;

    public override void _Ready()
    {
        base._Ready();

        Pivot = GetNode<Spatial>("Pivot");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        GetCurrentNoiseLevel();
                
        InputVector = GetInputVector();
        ApplyMovement(InputVector);

        ApplyGravity(delta);
        GetOtherInput(delta);
        Velocity = MoveAndSlideWithSnap(Velocity, Vector3.Down, Vector3.Up, true);

    }

    public void GetCurrentNoiseLevel()
    {
        PlayerNoise = 0f;

        if (!IsInteracting)
            InteractNoise = 0f;
        else
            PlayerNoise += InteractNoise;
    }

    public async void Idle(float time)
    {
        IsIdling = true;
        await ToSignal(GetTree().CreateTimer(time), "timeout");
        IsIdling = false;
    }
    public void ApplyMovement(Vector3 input)
    {
        if (IsIdling)
            return;

        Velocity.x = InputVector.x * (Speed * Sneak);
        Velocity.z = InputVector.z * (Speed * Sneak);

        if (InputVector != Vector3.Zero)
        {
            Pivot.LookAt(Translation + InputVector, Vector3.Up);
            if (IsSneaking)
                PlayerNoise += SneakNoise;
            else
                PlayerNoise += MovementNoise;
        }
            
    }
    public void ApplyGravity(float delta)
    {
        Velocity.y -= Gravity * delta;
    }
    public Vector3 GetInputVector()
    {
        InputVector = Vector3.Zero;
        InputVector.x = Input.GetActionStrength("Right") - Input.GetActionStrength("Left");
        InputVector.z = Input.GetActionStrength("Down") - Input.GetActionStrength("Up");

        return InputVector.Normalized();
    }
    public void GetOtherInput(float delta)
    {
        if (IsIdling)
            return;

        if (Input.IsActionJustPressed("Sneak"))
            ToggleSneak();
        if (Input.IsActionJustPressed("Interact"))
            TryInteract();
    }


    public void ToggleSneak()
    {
        if (!IsSneaking)
        {
            IsSneaking = true;
            Sneak = SneakSpeedFactor;
        }
        else
        {
            IsSneaking = false;
            Sneak = 1f;
        }
    }

    public void TryInteract()
    {
        int i = 0;
        if (InteractablesInRange.Count < 1)
        {
            GD.Print($"No interactables in range.");
            return;
        }
            
        foreach (var interactable in InteractablesInRange)
        {
            i++;
            interactable.MyDistanceToPlayer = this.GlobalTransform.origin.DistanceTo(interactable.MyPosition);
            GD.Print($"Interactable #{i} of type:{interactable.GetType()}, Range: {interactable.MyDistanceToPlayer}");
        }
        InteractablesInRange.OrderByDescending(x => x.MyDistanceToPlayer);

        InteractWith(InteractablesInRange[0]);
    }

    public void InteractWith(Interactable interactable)
    {
        var interractobject = interactable.Object;
        var interacttype = interractobject.GetType();
        
        GD.Print($"Interacting with a {interacttype}");
        InteractBase tmp = (InteractBase)interractobject ?? null;
        if (tmp != null)
        {
            InteractNoise = (float)tmp.Get("InteractNoise");
            tmp.DoStuff();
        }
    }


    public void InteractableListAdd(Interactable interactable)
    {

        try
        {
            InteractablesInRange.Add(interactable);
            GD.Print($"Added interactable with id: {interactable.InstanceID} with type: {interactable.Object.GetType()}");
        }
        catch (Exception ex)
        {
            GD.Print($"Error: {ex.Message}");
        }
    }

    public void InteractableListRemove(ulong uid)
    {
        try
        {
            InteractablesInRange.RemoveAll(x => x.InstanceID == uid);
            GD.Print($"Removed all interactables with Unique id: {uid}");
        }
        catch (Exception ex)
        {
            GD.Print($"Error: {ex.Message}");
        }
    }
}

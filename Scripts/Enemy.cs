using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Enemy : KinematicBody
{
    public int CurrentPatrolPoint;
    public int NextPatrolPoint;
    [Export] public eEnemyType EnemyType = eEnemyType.Undefined;
    [Export] public float Speed = 7f;
    [Export] public float RunSpeed = 10f;
    public float Gravity = 70f;
    [Export] public string PatrolPathName = "";
    public Path MyPath;
    public Curve3D PatrolPath;
    public bool IsIdling = false;
    public bool IsMoving = false;
    public Vector3 PivotBasisZ;
    [Export] public bool IsDetecting = true;
    [Export] public float DetectDistance = 20;
    [Export] public float DetectFOV = 30f;
    [Export] public bool IdleBetweenPoints = true;
    [Export] public float IdleTime = 1f;
    [Export] public float NoiseTreshold = 4f;

    public Vector3 Velocity = Vector3.Zero;
    public Vector3 Target = Vector3.Zero;
    public Spatial Pivot;
    public Player player;
    public Area DetectionArea;
    public CollisionShape DetectionShape;
    public Area DetectionConeArea;
    public CollisionShape DetectionConeShape;
    public bool ChasingPlayer = false;

    public bool IsPlayerInCone = false;
    public PhysicsDirectSpaceState SpaceState;
    
    public override void _Ready()
    {
        base._Ready();
        SpaceState = GetWorld().DirectSpaceState;

        player = Owner.Owner.GetNode<Player>("PlayerManager/Player");
        Pivot = GetNode<Spatial>("Pivot");

        DetectionArea = GetNode<Area>("DetectionArea");
        DetectionShape = DetectionArea.GetNode<CollisionShape>("CollisionShape");

        DetectionShape.Set("radius", DetectDistance);

        DetectionConeArea = GetNode<Area>("DetectionConeArea");
        DetectionConeShape = DetectionConeArea.GetNode<CollisionShape>("CollisionShape");

        DetectionConeArea.Monitoring = false;

        GetPatrolPath();
        SetNextPatrolPoint();
    }

    public void SetNextPatrolPoint()
    {
        NextPatrolPoint++;
        if (PatrolPath.GetPointCount() <= NextPatrolPoint)
            NextPatrolPoint = 0;

        var tmppos = PatrolPath.GetPointPosition(NextPatrolPoint);

        SetTarget(tmppos);
    }

    public Vector3 EnemyToPlayer()
    {
        var etp = Transform.origin - player.GlobalTransform.origin;
        return etp;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        Velocity.y += Gravity * delta; //Apply Gravity
        if (!IsIdling)
            ApplyMovement(Target); //Lets move

        TryToFindPlayer();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public void GetPatrolPath()
    {
        if (!String.IsNullOrEmpty(PatrolPathName))
        {
            MyPath = Owner.GetNode<Path>($"Paths/{PatrolPathName}");
            PatrolPath = MyPath.Curve;
        }
        if (PatrolPath == null)
            return;
        if (PatrolPath.GetPointCount() < 1)
            return;

        for (int i = 0; i < PatrolPath.GetPointCount(); i++)
        {
            var tmppos = PatrolPath.GetPointPosition(i);
        }
    }

    public void ModelLookAt(Vector3 target)
    {
        LookAt(target, Vector3.Up);
        Rotation = new Vector3(0, Rotation.y, Rotation.z);
    }
    public void ApplyMovement(Vector3 tar)
    {
        if (ChasingPlayer)
        {
            GD.Print($"Chasing player!!!");
            var tmptarget = player.GlobalTransform.origin;

            ModelLookAt(tmptarget);
            Velocity = -Transform.basis.z * RunSpeed;

            if (Transform.origin.DistanceTo(tmptarget) < 4f)
                CaughtPlayer();

        }

        else if (tar != Vector3.Zero)
        {
            ModelLookAt(tar);
            Velocity = -Transform.basis.z * Speed;
            if (Transform.origin.DistanceTo(tar) < 4.1f)
            {
                if (IdleBetweenPoints)
                    {
                        Idle(IdleTime);
                    }
                    
                tar = Vector3.Zero;
                Velocity = Vector3.Zero;
                    
                SetNextPatrolPoint();
            }
        }
        if (!IsIdling)
        {
            Velocity = MoveAndSlide(Velocity, Vector3.Up, true);
        }
    }

    public async void Idle(float time)
    {
        IsIdling = true;
        await ToSignal(GetTree().CreateTimer(time), "timeout");
        IsIdling = false;
    }
    public void SetTarget(Vector3 tar)
    {
        this.Target = tar;
    }

    public enum eEnemyType
    {
        Undefined,
        SecurityGuard,
        Dog,
        Police
    }

    public virtual void _on_DetectionArea_body_entered(Node body)
    {
        if (body.GetType() == typeof(Player))
        {
            GD.Print($"{this.GetType()}: Player entered detection radius!");
            if (player.PlayerNoise > NoiseTreshold && player.IsInteracting)
            {
                ModelLookAt(player.Transform.origin);
                GD.Print($"I think i heard something!");
                Idle(5);
            }
            DetectionConeArea.Monitoring = true;
        }
    }
    public virtual void _on_DetectionArea_body_exited(Node body)
    {
        if (body.GetType() == typeof(Player))
        {
            GD.Print($"{this.GetType()}: Player exited detection radius!");
            DetectionConeArea.SetDeferred("monitoring", false);
        }
    }
    public virtual void _on_DetectionConeArea_body_entered(Node body)
    {
        if (body.GetType() == typeof(Player))
        {
            TryToFindPlayer();
            GD.Print($"Player entered cone");
            IsPlayerInCone = true;
        }
    }
    public virtual void _on_DetectionConeArea_body_exited(Node body)
    {
        if (body.GetType() == typeof(Player))
        {
            GD.Print($"Player exited cone");
            IsPlayerInCone = false;
        }
    }

    public virtual void TryToFindPlayer()
    {
            var result = SpaceState.IntersectRay(this.GlobalTransform.origin, player.GlobalTransform.origin, new Godot.Collections.Array { this });
            if (result.Count > 0)
            {
                if (result["collider"].GetType() == typeof(Player) && IsPlayerInCone)
                {
                    if (!ChasingPlayer)
                    {
                        GD.Print($"Chasing player!");
                        ChasingPlayer = true;
                    }
                }
                else
                {
                    if (ChasingPlayer)
                    {
                        ChasingPlayer = false;
                        GD.Print("Lost the player i think.");
                        Idle(0.3f);
                    }
                }
            }
    }
    public virtual void StartChasingPlayer()
    {
        ChasingPlayer = true;
    }

    public virtual void StopChasingPlayer()
    {
        ChasingPlayer = false;

        ResetArea(DetectionArea);
        ResetArea(DetectionConeArea);

        DetectionConeArea.SetDeferred("monitoring", false);

        TryToFindPlayer();
        Idle(2f);
    }

    public void ResetArea(Area area)
    {
        area.Monitoring = false;
        Idle(0.1f);
        area.Monitoring = true;

    }
    public virtual void CaughtPlayer()
    {
        GD.Print($"Player got apprehended?");
    }
}
    

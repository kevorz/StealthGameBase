using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class SecurityMonitor : InteractBase
{
    [Export] public List<String> securityCameraNames;
    public Viewport SecurityCam;
    public ViewportTexture CamTexture;
    public Sprite3D sprite;
    public int CurrentCam;
    public override void _Ready()
    {
        base._Ready();
        this.InteractionTime = 0.1f;
        sprite = GetNode<Sprite3D>("Sprite3D");
        ChangeCam();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public override void StuffToDo()
    {
        base.StuffToDo();

        GD.Print($"SecMonitor dostuff reached.");
        CurrentCam++;
        ChangeCam();
    }

    public void ChangeCam()
    {
        GD.Print($"Currentcam: {CurrentCam}");
        if (CurrentCam > securityCameraNames.Count() - 1)
            CurrentCam = 0;

        SecurityCam = Owner.GetNode<Viewport>($"Agents/{securityCameraNames[CurrentCam]}/Viewport");
        if (SecurityCam != null)
        {
            CamTexture = SecurityCam.GetTexture();
            sprite.Texture = CamTexture;
        }
    }

}

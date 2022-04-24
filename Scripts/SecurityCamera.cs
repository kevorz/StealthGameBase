using Godot;
using System;

public class SecurityCamera : Spatial
{
    public Viewport viewport;
    public ViewportTexture viewportTexture;
    public Camera myCamera;
    public override void _Ready()
    {
        this.viewport = GetNode<Viewport>("Viewport");
        viewportTexture = viewport.GetTexture();
        viewportTexture.GetData().FlipY();
        myCamera = GetNode<Camera>("Viewport/Camera");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        myCamera.GlobalTransform = this.GlobalTransform;
    }

}

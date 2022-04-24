using System;
using Godot;

public class Interactable
{
    public object Object {get;set;}
    public ulong InstanceID {get;set;}
    public Vector3 MyPosition {get;set;}
    public float MyDistanceToPlayer {get;set;}
}
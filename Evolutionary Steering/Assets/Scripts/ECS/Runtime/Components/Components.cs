using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public struct MousePosition : IComponentData
{
    public float2 value;
}

public struct OutOfBoundSteering : IComponentData
{
    public Bounds squareBounds;
    public float steeringForce;
}

public struct Energy : IComponentData
{
    public float max;
    public float current;
    public float decreasePerSeconds;

    public float foodDigestion;

    public Color fullHpColor;
    public bool colorChange;
}

[System.Serializable]
public struct Range
{
    public float min;
    public float max;
}

public struct CameraFollow : IComponentData
{
    public bool enabled;

    public Entity currentTarget;
    public float3 offset;

    public Range fromRange;
    public Range moveSpeedRange;
    public Range dragSpeedRange;
    public Range scrollSpeedRange;

    public float GetMoveSpeed(float x)
    {
        return math.remap(fromRange.min, fromRange.max, moveSpeedRange.min, moveSpeedRange.max, x);
    }

    public float GetDragSpeed(float x)
    {
        return math.remap(fromRange.min, fromRange.max, dragSpeedRange.min, dragSpeedRange.max, x);
    }

    public float GetScrollSpeed(float x)
    {
        return math.remap(fromRange.min, fromRange.max, scrollSpeedRange.min, scrollSpeedRange.max, x);
    }
}

public struct Selector : IComponentData
{
    public CollisionFilter layers;
}

public struct SelectorInit : IComponentData
{
    
}

public struct Selected : IComponentData
{
    
}
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

public struct Health : IComponentData
{
    public float max;
    public float current;
    public float hpPerFood;
    public float hpPerPoison;
    public float decreasePerSeconds;

    public Color fullHpColor;
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
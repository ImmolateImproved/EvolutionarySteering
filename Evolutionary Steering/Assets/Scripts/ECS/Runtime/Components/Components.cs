using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct MousePosition : IComponentData
{
    public float2 value;
}

public struct SquareWorldBounds : IComponentData
{
    public float2 value;
}

public struct OutOfBoundSteering : IComponentData
{
    public float3 center;
    public float radiusSq;

    public float steeringForce;
}

public struct Health : IComponentData
{
    public float max;
    public float current;
    public float hpPerKill;
    public float decreasePerSeconds;

    public Color fullHpColor;
}
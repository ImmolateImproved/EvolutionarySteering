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
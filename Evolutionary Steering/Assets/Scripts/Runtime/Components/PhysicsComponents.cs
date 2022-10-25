using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct ResultantForce : IComponentData
{
    public float3 value;
}

public struct PhysicsData : IComponentData
{
    public float3 velocity;
    public float maxSpeed;
}

public readonly partial struct PhysicsBodyAspect : IAspect
{
    readonly RefRW<ResultantForce> resultantForce;
    readonly RefRW<PhysicsData> physicsData;

    public float3 ResultantForce
    {
        get => resultantForce.ValueRO.value;
        set => resultantForce.ValueRW.value = value;
    }

    public float3 Velocity
    {
        get => physicsData.ValueRO.velocity;
        set => physicsData.ValueRW.velocity = value;
    }

    public float MaxSpeed
    {
        get => physicsData.ValueRO.maxSpeed;
        set => physicsData.ValueRW.maxSpeed = value;
    }

    public void ApplyForce(ref Translation translation, float deltaTime)
    {
        var acceleration = ResultantForce;

        Velocity += acceleration;
        Velocity = MathUtils.ClampMagnitude(Velocity, MaxSpeed);

        translation.Value += Velocity * deltaTime;

        ResultantForce = 0;
    }
}
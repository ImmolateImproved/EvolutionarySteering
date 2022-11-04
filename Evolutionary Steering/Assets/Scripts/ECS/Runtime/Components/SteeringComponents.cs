using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct SteeringAgent : IComponentData
{
    public float maxForce;
}

public readonly partial struct SteeringAgentAspect : IAspect
{
    readonly PhysicsBodyAspect physicsBodyAspect;
    readonly RefRO<Translation> translation;
    readonly RefRO<SteeringAgent> steeringAgent;

    public float MaxForce => steeringAgent.ValueRO.maxForce;
    public float3 Position => translation.ValueRO.Value;

    public void Steer(float attractionForce, float3 targetPosition)
    {
        var force = targetPosition - translation.ValueRO.Value;

        force = MathUtils.SetMagnitude(force, physicsBodyAspect.MaxSpeed);

        force -= physicsBodyAspect.Velocity;

        force = MathUtils.ClampMagnitude(force, MaxForce);

        physicsBodyAspect.ResultantForce += force * attractionForce;
    }
}
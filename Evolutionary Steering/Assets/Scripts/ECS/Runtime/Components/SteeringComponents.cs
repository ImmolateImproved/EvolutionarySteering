using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct SteeringData : IBufferElementData
{
    public float attractionForce;
    public float maxForce;
    public float predictionAmount;
}

public readonly partial struct SteeringAgentAspect : IAspect
{
    readonly PhysicsBodyAspect physicsBodyAspect;
    readonly RefRO<Translation> translation;

    public float3 Position => translation.ValueRO.Value;

    public void Steer(in SteeringData steeringData, float3 targetPosition)
    {
        var force = targetPosition - translation.ValueRO.Value;

        force = MathUtils.SetMagnitude(force, physicsBodyAspect.MaxSpeed);

        force -= physicsBodyAspect.Velocity;

        force = MathUtils.ClampMagnitude(force, steeringData.maxForce);

        physicsBodyAspect.ResultantForce += force * steeringData.attractionForce;
    }

    public void SteerAhead(in SteeringData steeringData, float3 targetPosition, float3 targetDirection)
    {
        targetPosition += targetDirection * steeringData.predictionAmount;
        Steer(steeringData, targetPosition);
    }
}
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct SteeringData : IBufferElementData
{
    public float attractionForce;
    public float maxForce;
    public float predictionAmount;
    public float slowRadiusSq;
}

public readonly partial struct SteeringAgentAspect : IAspect
{
    readonly PhysicsBodyAspect physicsBodyAspect;
    readonly RefRO<Translation> translation;

    public void Steer(in SteeringData steeringData, float3 targetPosition)
    {
        var force = targetPosition - translation.ValueRO.Value;

        var distance = math.lengthsq(force);

        var maxSpeed = physicsBodyAspect.MaxSpeed;

        var desiredSpeed = distance < steeringData.slowRadiusSq
           ? math.remap(steeringData.slowRadiusSq, 0, maxSpeed, 0, distance)
           : maxSpeed;

        force = MathUtils.SetMagnitude(force, desiredSpeed);

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
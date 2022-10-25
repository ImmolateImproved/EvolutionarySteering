using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(ApplyForceSystem))]
public partial struct OutOfBoundsSteeringSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<OutOfBoundSteering>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var outOfBoundSteeringData = SystemAPI.GetSingleton<OutOfBoundSteering>();

        new SquareBoundsJob
        {
            outOfBoundData = outOfBoundSteeringData

        }.ScheduleParallel();

    }

    [BurstCompile]
    partial struct SquareBoundsJob : IJobEntity
    {
        public OutOfBoundSteering outOfBoundData;

        public void Execute(SteeringAgentAspect steeringAgentAspect)
        {
            if (!outOfBoundData.squareBounds.Contains(steeringAgentAspect.Position))
            {
                steeringAgentAspect.Steer(outOfBoundData.steeringForce, outOfBoundData.squareBounds.center);
            }
        }
    }
}
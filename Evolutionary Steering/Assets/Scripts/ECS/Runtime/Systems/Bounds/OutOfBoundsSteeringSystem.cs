using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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

        new OutOfBoundsForceJob
        {
            outOfBoundSteeringData = outOfBoundSteeringData

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct OutOfBoundsForceJob : IJobEntity
    {
        public OutOfBoundSteering outOfBoundSteeringData;

        public void Execute(SteeringAgentAspect steeringAgentAspect, in Translation translation)
        {
            if (math.distancesq(translation.Value, outOfBoundSteeringData.center) > outOfBoundSteeringData.radiusSq)
            {
                var steeringData = new SteeringData
                {
                    attractionForce = 1,
                    maxForce = outOfBoundSteeringData.steeringForce
                };

                steeringAgentAspect.Steer(steeringData, outOfBoundSteeringData.center);
            }

        }
    }
}
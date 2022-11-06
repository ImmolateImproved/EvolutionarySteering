using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateBefore(typeof(ApplyForceSystem))]
[UpdateAfter(typeof(FindTargetSystem))]
public partial struct PursueTargetSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var translationLookup = SystemAPI.GetComponentLookup<Translation>(true);
        var rotationLookup = SystemAPI.GetComponentLookup<Rotation>(true);

        new SteeringJob
        {
            targetTranslations = translationLookup,
            targetRotations = rotationLookup

        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithNone(typeof(InactiveState))]
    partial struct SteeringJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<Translation> targetTranslations;

        [ReadOnly]
        public ComponentLookup<Rotation> targetRotations;

        public void Execute(SteeringAgentAspect steeringAspect, in Energy health, in TargetSeeker seeker)
        {
            if (!targetTranslations.HasComponent(seeker.target))
                return;

            var targetDirection = math.mul(targetRotations[seeker.target].Value, new float3(1, 0, 0));
            var targetPos = targetTranslations[seeker.target].Value;
            targetPos += targetDirection * seeker.predictionAmount;

            var missingHpFraction = 1f - (health.current / health.max);

            var attractionForce = seeker.attractionForce + (missingHpFraction * seeker.hungerAttractionBonus);

            steeringAspect.Steer(attractionForce, targetPos);
        }
    }
}
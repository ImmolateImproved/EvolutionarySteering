using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(RotateTowardsVelocitySystem))]
public partial struct SteeringSystem : ISystem
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
    partial struct SteeringJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<Translation> targetTranslations;

        [ReadOnly]
        public ComponentLookup<Rotation> targetRotations;

        public void Execute(SteeringAgentAspect steeringAgent, in DynamicBuffer<TargetSeeker> seeker, in DynamicBuffer<SteeringData> steeringDatas)
        {
            var seekerArray = seeker.AsNativeArray();

            for (int i = 0; i < seekerArray.Length; i++)
            {
                var seekerData = seekerArray[i];

                if (!targetTranslations.HasComponent(seekerData.target))
                    continue;

                var targetDirection = math.mul(targetRotations[seekerData.target].Value, new float3(1, 0, 0));
                var targetPos = targetTranslations[seekerData.target].Value;

                steeringAgent.SteerAhead(steeringDatas[i], targetPos, targetDirection);
            }
        }
    }
}
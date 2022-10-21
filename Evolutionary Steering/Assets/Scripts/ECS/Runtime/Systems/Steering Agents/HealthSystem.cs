using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
public partial struct HealthSystem : ISystem
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
        var dt = SystemAPI.Time.DeltaTime;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var targetInRangeLookup = SystemAPI.GetComponentLookup<TargetInRangeTag>();

        var colorLookup = SystemAPI.GetComponentLookup<URPMaterialPropertyBaseColor>();

        new TargetInDistanceJob
        {
            targetInRangeTag = targetInRangeLookup

        }.ScheduleParallel();

        new HealthJob
        {
            ecb = ecb.AsParallelWriter(),
            dt = dt

        }.ScheduleParallel();

        new HealthViewJob
        {
            colorLookup = colorLookup

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct HealthJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        public float dt;

        public void Execute(Entity e, [ChunkIndexInQuery] int chunkIndex, ref Health health)
        {
            health.current -= health.decreasePerSeconds * dt;

            if (health.current <= 0)
            {
                ecb.DestroyEntity(chunkIndex, e);
            }
        }
    }

    [BurstCompile]
    partial struct TargetInDistanceJob : IJobEntity
    {
        [NativeDisableContainerSafetyRestriction]
        public ComponentLookup<TargetInRangeTag> targetInRangeTag;

        public void Execute(Entity e, ref Health health, ref TargetInRangeTag targetInRange)
        {
            if (targetInRange.targetType == TargetTypeEnum.Food)
            {
                health.current += health.hpPerKill;
            }
            else if (targetInRange.targetType == TargetTypeEnum.Poison)
            {
                health.current = 0;
            }

            health.current = math.clamp(health.current, 0, health.max);

            targetInRangeTag.SetComponentEnabled(e, false);
        }
    }

    [BurstCompile]
    partial struct HealthViewJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        public ComponentLookup<URPMaterialPropertyBaseColor> colorLookup;

        public void Execute(in DynamicBuffer<Child> children, in Health health)
        {
            for (int i = 0; i < children.Length; i++)
            {
                var color = (Vector4)Color.Lerp(Color.red, health.fullHpColor, health.current / health.max);
                colorLookup[children[i].Value] = new URPMaterialPropertyBaseColor { Value = color };
            }
        }
    }
}
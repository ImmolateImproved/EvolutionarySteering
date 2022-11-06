using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(FindTargetSystem))]
public partial struct DestroyNearestTargetSystem : ISystem
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
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var translationLookup = SystemAPI.GetComponentLookup<Translation>(true);
        var targetInRangeLookup = SystemAPI.GetComponentLookup<TargetInRange>();
        var energyLookup = SystemAPI.GetComponentLookup<Energy>(true);

        state.Dependency = new DestroyNearestTargetJob
        {
            ecb = ecb.AsParallelWriter(),
            translationLookup = translationLookup,
            targetInRangeLookup = targetInRangeLookup,
            energyLookup = energyLookup

        }.ScheduleParallel(state.Dependency);
    }

    [BurstCompile]
    [WithAll(typeof(TargetSeeker))]
    partial struct DestroyNearestTargetJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<Translation> translationLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<TargetInRange> targetInRangeLookup;

        [ReadOnly]
        public ComponentLookup<Energy> energyLookup;

        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute(Entity e, [ChunkIndexInQuery] int chunkIndex, in Translation translation, in TargetSeeker targetSeeker, in Energy energy)
        {
            targetInRangeLookup.SetComponentEnabled(e, false);

            var target = targetSeeker.target;

            if (!translationLookup.HasComponent(target))
                return;

            var dist = math.distancesq(translation.Value, translationLookup[target].Value);

            if (dist < 1)
            {
                targetInRangeLookup.SetComponentEnabled(e, true);

                if (energy.current >= energyLookup[targetSeeker.target].current)
                    ecb.DestroyEntity(chunkIndex, target);
            }

        }
    }
}
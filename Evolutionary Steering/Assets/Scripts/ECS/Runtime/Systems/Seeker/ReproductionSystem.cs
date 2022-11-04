using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(DestroyNearestTargetSystem))]
public partial struct ReproductionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitPrefab>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var unitPrefabs = SystemAPI.GetSingletonBuffer<UnitPrefab>(true);

        new ReproductionJob
        {
            unitPrefabs = unitPrefabs,
            ecb = ecb.AsParallelWriter()

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct ReproductionJob : IJobEntity
    {
        [ReadOnly]
        public DynamicBuffer<UnitPrefab> unitPrefabs;
        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute([ChunkIndexInQuery] int chunkIndex, ReproductionAspect reproductionAspect)
        {
            var unitPrefab = unitPrefabs[reproductionAspect.PrefabIndex].value;

            reproductionAspect.ReproduceAndMutate(ref ecb, chunkIndex, unitPrefab);
        }
    }
}
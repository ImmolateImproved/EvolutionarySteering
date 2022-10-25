using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(DestroyNearestTargetSystem))]
public partial struct ReproductionSystem : ISystem
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

        new ReproductionJob
        {
            ecb = ecb.AsParallelWriter()

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct ReproductionJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute([ChunkIndexInQuery] int chunkIndex, ReproductionAspect reproductionAspect)
        {
            reproductionAspect.ReproduceAndMutate(ref ecb, chunkIndex);
        }
    }
}
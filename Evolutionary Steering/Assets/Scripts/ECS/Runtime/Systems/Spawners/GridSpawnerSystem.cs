using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct GridSpawnerSystem : ISystem
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
        foreach (var (spawner, gridSpawner) in SystemAPI.Query<SpawnerAspect, RefRW<GridSpawner>>())
        {
            spawner.Spawn(ref state, ref gridSpawner.ValueRW);
        }

        foreach (var (spawner, circularSpawner) in SystemAPI.Query<SpawnerAspect, RefRW<CircularSpawner>>())
        {
            spawner.Spawn(ref state, ref circularSpawner.ValueRW);
        }
    }
}
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnerSystem : ISystem
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
        foreach (var (spawner, gridPositionFabric) in SystemAPI.Query<SpawnerAspect, RefRW<GridPositionFactory>>().WithNone<SpawnTimer>())
        {
            spawner.Spawn(ref state, ref gridPositionFabric.ValueRW);
            spawner.Clear();
        }

        foreach (var (spawner, circularPositionFabric) in SystemAPI.Query<SpawnerAspect, RefRW<CircularPositionFactory>>().WithNone<SpawnTimer>())
        {
            spawner.Spawn(ref state, ref circularPositionFabric.ValueRW);
            spawner.Clear();
        }

        foreach (var (spawner, gridPositionFabric, timer) in SystemAPI.Query<SpawnerAspect, RefRW<GridPositionFactory>, RefRW<SpawnTimer>>())
        {
            timer.ValueRW.current += SystemAPI.Time.DeltaTime;

            if (timer.ValueRW.current >= timer.ValueRW.max)
            {
                timer.ValueRW.current = 0;
                spawner.Spawn(ref state, ref gridPositionFabric.ValueRW);
            }
        }
    }
}
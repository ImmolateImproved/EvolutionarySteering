using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;

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
        foreach (var (spawner, gridSpawner) in SystemAPI.Query<SpawnerAspect, RefRW<GridSpawner>>().WithNone<Timer>())
        {
            spawner.Spawn(ref state, ref gridSpawner.ValueRW);
            spawner.Clear();
        }

        foreach (var (spawner, circularSpawner) in SystemAPI.Query<SpawnerAspect, RefRW<CircularSpawner>>().WithNone<Timer>())
        {
            spawner.Spawn(ref state, ref circularSpawner.ValueRW);
            spawner.Clear();
        }

        foreach (var (spawner, gridSpawner, timer) in SystemAPI.Query<SpawnerAspect, RefRW<GridSpawner>, RefRW<Timer>>())
        {
            timer.ValueRW.current += SystemAPI.Time.DeltaTime;

            if (timer.ValueRW.current >= timer.ValueRW.max)
            {
                timer.ValueRW.current = 0;
                spawner.Spawn(ref state, ref gridSpawner.ValueRW);
            }
        }
    }
}
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
        foreach (var (spawner, gridPositionFabric) in SystemAPI.Query<SpawnerAspect, RefRW<GridPositionFabric>>().WithNone<SpawnTimer>())
        {
            spawner.Spawn(ref state, ref gridPositionFabric.ValueRW);
            spawner.Clear();
        }

        foreach (var (spawner, circularPositionFabric) in SystemAPI.Query<SpawnerAspect, RefRW<CircularPositionFabric>>().WithNone<SpawnTimer>())
        {
            spawner.Spawn(ref state, ref circularPositionFabric.ValueRW);
            spawner.Clear();
        }

        foreach (var (spawner, gridPositionFabric, timer) in SystemAPI.Query<SpawnerAspect, RefRW<GridPositionFabric>, RefRW<SpawnTimer>>())
        {
            timer.ValueRW.current += SystemAPI.Time.DeltaTime;

            if (timer.ValueRW.current >= timer.ValueRW.max)
            {
                timer.ValueRW.current = 0;
                spawner.Spawn(ref state, ref gridPositionFabric.ValueRW);
            }
        }

        foreach (var (spawner, gridPositionFabric, mutationData) in SystemAPI.Query<SpawnerAspect, RefRW<GridPositionFabric>, RefRW<MutationData>>().WithNone<SpawnTimer>())
        {
            for (int i = 0; i < spawner.SpawnRequestCount; i++)
            {
                var seekerEntities = spawner.Spawn(ref state, ref gridPositionFabric.ValueRW, i);

                for (int j = 0; j < seekerEntities.Length; j++)
                {
                    var steeringAgent = SystemAPI.GetComponent<SteeringAgent>(seekerEntities[j]);
                    steeringAgent.maxForce += mutationData.ValueRO.maxFroce;
                    SystemAPI.SetComponent(seekerEntities[j], steeringAgent);

                    var physicsData = SystemAPI.GetComponent<PhysicsData>(seekerEntities[j]);
                    physicsData.maxSpeed = mutationData.ValueRO.maxSpeed;
                    SystemAPI.SetComponent(seekerEntities[j], physicsData);

                    var seekerDatas = SystemAPI.GetBuffer<TargetSeeker>(seekerEntities[j]);

                    for (int k = 0; k < seekerDatas.Length; k++)
                    {
                        ref var seekerData = ref seekerDatas.ElementAt(k);

                        seekerData.attractionForce += mutationData.ValueRO.attractionFroce;
                        seekerData.searchRadius += mutationData.ValueRO.targetSearchRadius;
                    }
                }
            }
            spawner.Clear();
        }
    }
}
using Unity.Burst;
using Unity.Entities;
using UnityEditor.PackageManager;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(SpawnerSystem))]
public partial struct SeekerSpawnerSpawnerSystem : ISystem
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
        var targetSeekerLookup = SystemAPI.GetBufferLookup<TargetSeeker>();

        foreach (var (spawner, gridPositionFabric, mutationData) in SystemAPI.Query<SpawnerAspect, RefRW<GridPositionFactory>, RefRW<InitialSeekerStats>>().WithNone<SpawnTimer>())
        {
            for (int i = 0; i < spawner.SpawnRequestCount; i++)
            {
                var seekerEntities = spawner.Spawn(ref state, ref gridPositionFabric.ValueRW, i);

                for (int j = 0; j < seekerEntities.Length; j++)
                {
                    var steeringAgent = SystemAPI.GetComponent<SteeringAgent>(seekerEntities[j]);
                    steeringAgent.maxForce = mutationData.ValueRW.GetMaxForce();
                    SystemAPI.SetComponent(seekerEntities[j], steeringAgent);

                    var physicsData = SystemAPI.GetComponent<PhysicsData>(seekerEntities[j]);
                    physicsData.maxSpeed = mutationData.ValueRW.GetMaxSpeed();
                    SystemAPI.SetComponent(seekerEntities[j], physicsData);

                    var seekerDatas = targetSeekerLookup[seekerEntities[j]];//SystemAPI.GetBuffer<TargetSeeker>(seekerEntities[j]);

                    ref var foodSeekerData = ref seekerDatas.ElementAt(0);
                    foodSeekerData.attractionForce = mutationData.ValueRW.GetAttractionForce();
                    foodSeekerData.searchRadius = mutationData.ValueRW.GetFoodSearchRadius();

                    ref var poisonSeekerData = ref seekerDatas.ElementAt(1);
                    poisonSeekerData.attractionForce = -mutationData.ValueRW.GetRepultionForce();
                    poisonSeekerData.searchRadius = mutationData.ValueRW.GetPoisonSearchRadius();

                }
            }

            spawner.Clear();
        }
    }
}
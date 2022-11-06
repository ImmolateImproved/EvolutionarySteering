using Unity.Entities;
using UnityEngine;

using Random = Unity.Mathematics.Random;

[System.Serializable]
public struct SpawnRequestData
{
    public GameObject prefab;
    public int count;
}

public class SpawnerAuthoring : MonoBehaviour
{
    public PositionFactoryData positionFactory;

    public SpawnRequestData[] spawnRequests;

    public float timeBetweenSpawns;

    class GridSpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            DependsOn(authoring.positionFactory);
            DependsOn(authoring.transform);

            var random = Random.CreateFromIndex((uint)(System.DateTime.Now.Millisecond + authoring.GetInstanceID()));

            var positionFactory = authoring.positionFactory;

            if (authoring.timeBetweenSpawns > 0)
            {
                AddComponent(new SpawnTimer
                {
                    max = authoring.timeBetweenSpawns

                });
            }

            switch (positionFactory.factoryType)
            {
                case PositionFactoryEnum.Square:
                    {
                        AddComponent(new GridPositionFactory
                        {
                            maxPosition = positionFactory.bounds,
                            minPosition = -positionFactory.bounds,
                            offset = authoring.transform.position,
                            random = random
                        });

                        break;
                    }
                case PositionFactoryEnum.Circle:
                    {
                        AddComponent(new CircularPositionFactory
                        {
                            center = authoring.transform.position,
                            maxRadius = positionFactory.maxRadius,
                            random = random
                        });
                        break;
                    }
            }

            var requests = AddBuffer<SpawnRequest>();

            for (int i = 0; i < authoring.spawnRequests.Length; i++)
            {
                requests.Add(new SpawnRequest
                {
                    count = authoring.spawnRequests[i].count,
                    prefab = GetEntity(authoring.spawnRequests[i].prefab),
                });
            }
        }
    }
}
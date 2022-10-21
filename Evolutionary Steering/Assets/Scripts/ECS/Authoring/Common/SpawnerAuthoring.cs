using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct SpawnRequestData
{
    public GameObject prefab;
    public int count;
}

public enum SpawnerTypeEnum
{
    Square, Circle
}

public class SpawnerAuthoring : MonoBehaviour
{
    public SpawnerTypeEnum spawnerType;

    [Header("Square spawner settings")]
    public Vector3 minPosition;
    public Vector3 maxPosition;

    [Header("Circular spawner settings")]
    public float maxRadius;

    [Header("Common spawner settings")]
    public Vector3 offset;

    public SpawnRequestData[] spawnRequests;

    class GridSpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Millisecond);

            switch (authoring.spawnerType)
            {
                case SpawnerTypeEnum.Square:
                    {
                        AddComponent(new GridSpawner
                        {
                            minPosition = authoring.minPosition,
                            maxPosition = authoring.maxPosition,
                            offset = authoring.offset,
                            random = random
                        });

                        break;
                    }
                case SpawnerTypeEnum.Circle:
                    {
                        AddComponent(new CircularSpawner
                        {
                            center = authoring.offset,
                            maxRadius = authoring.maxRadius,
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
                    prefab = GetEntity(authoring.spawnRequests[i].prefab)
                });
            }
        }
    }
}
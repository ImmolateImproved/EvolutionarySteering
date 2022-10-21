using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public interface ISpawner
{
    float3 GetNextSpawnPosition();
}

public struct GridSpawner : IComponentData, ISpawner
{
    public float3 minPosition;
    public float3 maxPosition;
    public float3 offset;

    public Random random;

    public float3 GetNextSpawnPosition()
    {
        return random.NextFloat3(minPosition, maxPosition) + offset;
    }
}

public struct CircularSpawner : IComponentData, ISpawner
{
    public float3 center;
    public float maxRadius;

    public Random random;

    public float3 GetNextSpawnPosition()
    {
        var radius = random.NextFloat(maxRadius);

        var point = random.NextFloat2Direction() * radius;

        return math.float3(point, 0) + center;
    }
}

public struct SpawnRequest : IBufferElementData
{
    public Entity prefab;

    public int count;
}

public readonly partial struct SpawnerAspect : IAspect
{
    readonly DynamicBuffer<SpawnRequest> spawnRequestBuffer;

    public int SpawnRequestCount => spawnRequestBuffer.Length;

    public void Spawn<T>(ref SystemState state, ref T spawner) where T : ISpawner
    {
        for (int i = 0; i < SpawnRequestCount; i++)
        {
            Spawn(ref state, ref spawner, i);
        }

        Clear();
    }

    public NativeArray<Entity> Spawn<T>(ref SystemState state, ref T spawner, int spawnRequestIndex) where T : ISpawner
    {
        var spawnRequest = spawnRequestBuffer[spawnRequestIndex];

        var entities = state.EntityManager.Instantiate(spawnRequest.prefab, spawnRequest.count, Allocator.Temp);

        for (int i = 0; i < entities.Length; i++)
        {
            var randomPosition = spawner.GetNextSpawnPosition();
            state.EntityManager.SetComponentData(entities[i], new Translation { Value = randomPosition });
        }

        return entities;
    }

    public void Clear()
    {
        spawnRequestBuffer.Clear();
    }
}
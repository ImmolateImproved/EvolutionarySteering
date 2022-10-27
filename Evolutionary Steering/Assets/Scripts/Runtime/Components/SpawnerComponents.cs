using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public interface IPositionFactory
{
    float3 GetNextSpawnPosition();
}

public struct GridPositionFactory : IComponentData, IPositionFactory
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

public struct CircularPositionFactory : IComponentData, IPositionFactory
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

public struct SpawnTimer : IComponentData
{
    public float max;
    public float current;
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

    public void Spawn<T>(ref SystemState systemState, ref T spawner) where T : IPositionFactory
    {
        for (int i = 0; i < SpawnRequestCount; i++)
        {
            Spawn(ref systemState, ref spawner, i);
        }
    }

    public NativeArray<Entity> Spawn<T>(ref SystemState systemState, ref T positionFactory, int spawnRequestIndex) where T : IPositionFactory
    {
        var spawnRequest = spawnRequestBuffer[spawnRequestIndex];

        var entities = systemState.EntityManager.Instantiate(spawnRequest.prefab, spawnRequest.count, Allocator.Temp);

        for (int i = 0; i < entities.Length; i++)
        {
            var randomPosition = positionFactory.GetNextSpawnPosition();
            systemState.EntityManager.SetComponentData(entities[i], new Translation { Value = randomPosition });
        }

        return entities;
    }

    public void Clear()
    {
        spawnRequestBuffer.Clear();
    }
}
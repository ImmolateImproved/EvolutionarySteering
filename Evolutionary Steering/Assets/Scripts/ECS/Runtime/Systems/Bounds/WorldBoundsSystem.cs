using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct WorldBoundsSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SquareWorldBounds>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var bounds = SystemAPI.GetSingleton<SquareWorldBounds>();
        
        new BoundariesCollisionJob
        {
            bounds = bounds.value

        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct BoundariesCollisionJob : IJobEntity
    {
        public float2 bounds;

        public void Execute(ref Translation translation, ref PhysicsData physicsData)
        {
            if (translation.Value.x >= bounds.x)
            {
                translation.Value.x = bounds.x;
                physicsData.velocity.x *= -1;
            }
            else if (translation.Value.x <= -bounds.x)
            {
                translation.Value.x = -bounds.x;
                physicsData.velocity.x *= -1;
            }

            if (translation.Value.y >= bounds.y)
            {
                translation.Value.y = bounds.y;
                physicsData.velocity.y *= -1;
            }

            if (translation.Value.y <= -bounds.y)
            {
                translation.Value.y = -bounds.y;
                physicsData.velocity.y *= -1;
            }
        }
    }
}
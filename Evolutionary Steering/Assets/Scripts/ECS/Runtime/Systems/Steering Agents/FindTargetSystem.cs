using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[UpdateBefore(typeof(SteeringSystem))]
public partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var targetsQuery = SystemAPI.QueryBuilder().WithAll<Translation, TargetType>().Build();

        var targetEntities = targetsQuery.ToEntityArray(Allocator.TempJob);
        var targetPositions = targetsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var targetTypes = targetsQuery.ToComponentDataArray<TargetType>(Allocator.TempJob);

        state.Dependency = new FindTargetJob
        {
            targetEntities = targetEntities,
            targetPositions = targetPositions,
            targetTypes = targetTypes

        }.ScheduleParallel(state.Dependency);

        targetEntities.Dispose(state.Dependency);
        targetPositions.Dispose(state.Dependency);
        targetTypes.Dispose(state.Dependency);
    }

    [BurstCompile]
    partial struct FindTargetJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<Entity> targetEntities;

        [ReadOnly]
        public NativeArray<Translation> targetPositions;

        [ReadOnly]
        public NativeArray<TargetType> targetTypes;

        public void Execute(in TransformAspect transform, ref DynamicBuffer<TargetSeeker> seekerBuffer)
        {
            var distances = new NativeArray<float>(seekerBuffer.Length, Allocator.Temp);

            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = math.INFINITY;
            }

            var newTargetEntity = new NativeArray<Entity>(seekerBuffer.Length, Allocator.Temp);

            for (int i = 0; i < targetEntities.Length; i++)
            {
                var targetTypeIndex = (int)targetTypes[i].value;

                var newDistance = math.distance(transform.Position, targetPositions[i].Value);

                if (newDistance < seekerBuffer[targetTypeIndex].searchRadius && newDistance < distances[targetTypeIndex])
                {
                    distances[targetTypeIndex] = newDistance;

                    newTargetEntity[targetTypeIndex] = targetEntities[i];
                }
            }

            for (int i = 0; i < seekerBuffer.Length; i++)
            {
                seekerBuffer.ElementAt(i).target = newTargetEntity[i];
            }
        }
    }

    [BurstCompile]
    partial struct FindTargetWithUnityPhysicsJob : IJobEntity
    {
        [ReadOnly]
        public PhysicsWorldSingleton physicsWorld;

        public void Execute(in Translation translation, in DynamicBuffer<TargetSeeker> seeker)
        {
            for (int i = 0; i < seeker.Length; i++)
            {
                var input = new PointDistanceInput
                {
                    Filter = seeker[i].layers,
                    MaxDistance = seeker[i].searchRadius,
                    Position = translation.Value
                };

                if (physicsWorld.CalculateDistance(input, out var closet))
                {
                    seeker.ElementAt(i).target = closet.Entity;
                }
            }
        }
    }
}
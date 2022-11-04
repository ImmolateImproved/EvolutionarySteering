using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[UpdateBefore(typeof(PursueTargetSystem))]
public partial struct FindTargetSystem : ISystem
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
        var dt = SystemAPI.Time.DeltaTime;

        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        new FindTargetJob
        {
            dt = dt,
            physicsWorld = physicsWorld

        }.ScheduleParallel();

        #region OldFindTarget
        //var targetsQuery = SystemAPI.QueryBuilder().WithAll<Translation, TargetType>().Build();

        //var targetEntities = targetsQuery.ToEntityArray(Allocator.TempJob);
        //var targetPositions = targetsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        //var targetTypes = targetsQuery.ToComponentDataArray<TargetType>(Allocator.TempJob);

        //state.Dependency = new FindTargetByCheckingDistanceJob
        //{
        //    targetEntities = targetEntities,
        //    targetPositions = targetPositions,
        //    targetTypes = targetTypes,
        //    dt = dt

        //}.ScheduleParallel(state.Dependency);
        #endregion
    }

    [BurstCompile]
    partial struct FindTargetJob : IJobEntity
    {
        [ReadOnly]
        public PhysicsWorldSingleton physicsWorld;

        public float dt;

        public void Execute(in Translation translation, ref DynamicBuffer<TargetSeeker> seekerBuffer)
        {
            for (int i = 0; i < seekerBuffer.Length; i++)
            {
                ref var seeker = ref seekerBuffer.ElementAt(i);

                var input = new PointDistanceInput
                {
                    Filter = seeker.layers,
                    MaxDistance = seeker.searchRadius,
                    Position = translation.Value
                };

                seeker.seekTimer += dt;
                if (seeker.seekTimer >= seeker.timeBeforeSeek)
                {
                    physicsWorld.CalculateDistance(input, out var closest);

                    seekerBuffer.ElementAt(i).target = closest.Entity;
                }
            }
        }
    }
    private struct IngnoreSelfCollector : ICollector<DistanceHit>
    {
        public bool EarlyOutOnFirstHit => false;

        public float MaxFraction { get; private set; }

        public int NumHits { get; private set; }

        public DistanceHit ClosestHit;

        private Entity entityToIgnore;

        public IngnoreSelfCollector(Entity entityToIgnore, float maxDistance)
        {
            this.entityToIgnore = entityToIgnore;

            MaxFraction = maxDistance;
            ClosestHit = default;
            NumHits = 0;
        }

        public bool AddHit(DistanceHit hit)
        {
            if (hit.Entity == entityToIgnore)
            {
                return false;
            }

            ClosestHit = hit;
            MaxFraction = hit.Fraction;
            NumHits = 1;

            return true;
        }
    }
    //[BurstCompile]
    //partial struct FindTargetByCheckingDistanceJob : IJobEntity
    //{
    //    [ReadOnly]
    //    [DeallocateOnJobCompletion]
    //    public NativeArray<Entity> targetEntities;

    //    [ReadOnly]
    //    [DeallocateOnJobCompletion]
    //    public NativeArray<Translation> targetPositions;

    //    [ReadOnly]
    //    [DeallocateOnJobCompletion]
    //    public NativeArray<TargetType> targetTypes;

    //    public float dt;

    //    public void Execute(in TransformAspect transform, ref DynamicBuffer<TargetSeeker> seekerBuffer)
    //    {
    //        var distances = new NativeArray<float>(seekerBuffer.Length, Allocator.Temp);

    //        for (int i = 0; i < distances.Length; i++)
    //        {
    //            distances[i] = math.INFINITY;
    //        }

    //        var newTargetEntity = new NativeArray<Entity>(seekerBuffer.Length, Allocator.Temp);

    //        for (int i = 0; i < targetEntities.Length; i++)
    //        {
    //            var targetTypeIndex = (int)targetTypes[i].value;

    //            var newDistance = math.distance(transform.Position, targetPositions[i].Value);

    //            if (newDistance < seekerBuffer[targetTypeIndex].searchRadius && newDistance < distances[targetTypeIndex])
    //            {
    //                distances[targetTypeIndex] = newDistance;

    //                newTargetEntity[targetTypeIndex] = targetEntities[i];
    //            }
    //        }

    //        for (int i = 0; i < seekerBuffer.Length; i++)
    //        {
    //            ref var seeker = ref seekerBuffer.ElementAt(i);

    //            seeker.seekTimer += dt;
    //            if (seeker.seekTimer >= seeker.timeBeforeSeek)
    //            {
    //                seeker.target = newTargetEntity[i];
    //            }
    //        }
    //    }
    //}
}
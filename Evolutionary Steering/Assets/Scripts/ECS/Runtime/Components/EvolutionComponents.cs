using Unity.Entities;
using Unity.Transforms;

public struct ReproductionData : IComponentData
{
    public int foodToReproduce;
    public int currentFood;
    public Entity seekerPrefab;
}

public struct MutationData : IComponentData
{
    public float attractionFroce;
    public float maxSpeed;
    public float maxFroce;
    public float targetSearchRadius;

    public Unity.Mathematics.Random random;

    public float GetMutationAmount(float value)
    {
        return random.NextFloat(-value, value);
    }
}

public readonly partial struct ReproductionAspect : IAspect
{
    readonly RefRW<MutationData> mutationData;
    readonly RefRW<ReproductionData> reproductionData;
    readonly RefRO<TargetInRange> targetInRange;

    readonly RefRO<SteeringAgent> steeringData;
    readonly RefRO<PhysicsData> physicsData;
    readonly RefRO<Translation> translation;

    readonly DynamicBuffer<TargetSeeker> targetSeeker;

    private MutationData MutationData => mutationData.ValueRO;

    public void ReproduceAndMutate(ref EntityCommandBuffer.ParallelWriter ecb, int chunkIndex)
    {
        if (targetInRange.ValueRO.targetType == TargetTypeEnum.Food)
        {
            reproductionData.ValueRW.currentFood++;

            if (reproductionData.ValueRO.currentFood >= reproductionData.ValueRO.foodToReproduce)
            {
                reproductionData.ValueRW.currentFood = 0;

                var seeker = ecb.Instantiate(chunkIndex, reproductionData.ValueRO.seekerPrefab);
                Mutate(ref ecb, chunkIndex, seeker);
            }
        }
    }

    private void Mutate(ref EntityCommandBuffer.ParallelWriter ecb, int chunkIndex, Entity seeker)
    {
        ecb.SetComponent(chunkIndex, seeker, new PhysicsData
        {
            maxSpeed = physicsData.ValueRO.maxSpeed + mutationData.ValueRW.GetMutationAmount(MutationData.maxSpeed),
            velocity = -physicsData.ValueRO.velocity
        });

        ecb.SetComponent(chunkIndex, seeker, translation.ValueRO);
        ecb.SetComponent(chunkIndex, seeker, MutateSteeringAgent());

        var seekerDatas = ecb.SetBuffer<TargetSeeker>(chunkIndex, seeker);
        MutateSeeker(ref seekerDatas);
    }

    private SteeringAgent MutateSteeringAgent()
    {
        var newSteeringData = steeringData.ValueRO;

        newSteeringData.maxForce += mutationData.ValueRW.GetMutationAmount(MutationData.maxFroce);

        return newSteeringData;
    }

    private void MutateSeeker(ref DynamicBuffer<TargetSeeker> seekerDatas)
    {
        seekerDatas.CopyFrom(targetSeeker);

        ref var seeker1 = ref seekerDatas.ElementAt(0);

        seeker1.seekTimer = 0;
        seeker1.timeBeforeSeek = 2;

        for (int i = 0; i < seekerDatas.Length; i++)
        {
            ref var seeker = ref seekerDatas.ElementAt(i);

            seeker.searchRadius += mutationData.ValueRW.GetMutationAmount(MutationData.targetSearchRadius);
            seeker.attractionForce += mutationData.ValueRW.GetMutationAmount(MutationData.attractionFroce);
        }
    }
}
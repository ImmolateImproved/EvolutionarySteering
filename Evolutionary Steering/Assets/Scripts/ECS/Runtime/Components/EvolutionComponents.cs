using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct ReproductionData : IComponentData
{
    public int prefabIndex;
    public int foodToReproduce;
    public int currentFood;
}

public struct UnitPrefab : IBufferElementData
{
    public Entity value;
}

public struct MutationData : IComponentData
{
    public float mutationChance;

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

public readonly partial struct MutationAspect : IAspect
{
    readonly RefRW<MutationData> mutationData;
    readonly RefRW<ReproductionData> reproductionData;
    readonly RefRO<TargetInRange> targetInRange;

    readonly RefRO<SteeringAgent> steeringData;
    readonly RefRO<PhysicsData> physicsData;
    readonly RefRO<Translation> translation;

    readonly DynamicBuffer<TargetSeeker> targetSeeker;

    private MutationData MutationData => mutationData.ValueRO;

    public int PrefabIndex => reproductionData.ValueRO.prefabIndex;

    public void ReproduceAndMutate(ref EntityCommandBuffer.ParallelWriter ecb, int chunkIndex, Entity childPrefab)
    {
        if (targetInRange.ValueRO.targetType == TargetTypeEnum.Food)
        {
            reproductionData.ValueRW.currentFood++;

            if (reproductionData.ValueRO.currentFood >= reproductionData.ValueRO.foodToReproduce)
            {
                reproductionData.ValueRW.currentFood = 0;

                var seeker = ecb.Instantiate(chunkIndex, childPrefab);

                InheritGenes(ref ecb, chunkIndex, seeker);
            }
        }
    }

    private void InheritGenes(ref EntityCommandBuffer.ParallelWriter ecb, int chunkIndex, Entity seeker)
    {
        ecb.SetComponent(chunkIndex, seeker, translation.ValueRO);

        var newPhysicsData = physicsData.ValueRO;
        InheritPhysicsData(ref newPhysicsData);
        ecb.SetComponent(chunkIndex, seeker, newPhysicsData);

        var newSteeringAgent = steeringData.ValueRO;
        InheritSteeringAgent(ref newSteeringAgent);
        ecb.SetComponent(chunkIndex, seeker, newSteeringAgent);

        var seekerDatas = ecb.SetBuffer<TargetSeeker>(chunkIndex, seeker);
        InheritSeeker(ref seekerDatas);
    }

    private void InheritPhysicsData(ref PhysicsData physicsData)
    {
        physicsData.velocity *= -1;
        if (MutationNeeded())
        {
            physicsData.maxSpeed += mutationData.ValueRW.GetMutationAmount(MutationData.maxSpeed);
        }
    }

    private void InheritSteeringAgent(ref SteeringAgent steeringAgent)
    {
        if (MutationNeeded())
        {
            steeringAgent.maxForce += mutationData.ValueRW.GetMutationAmount(MutationData.maxFroce);
        }
    }

    private void InheritSeeker(ref DynamicBuffer<TargetSeeker> seekerDatas)
    {
        seekerDatas.CopyFrom(targetSeeker);
        ref var seeker1 = ref seekerDatas.ElementAt(0);

        seeker1.seekTimer = 0;
        seeker1.timeBeforeSeek = 2;

        if (!MutationNeeded())
            return;

        for (int i = 0; i < seekerDatas.Length; i++)
        {
            ref var seeker = ref seekerDatas.ElementAt(i);

            seeker.searchRadius += mutationData.ValueRW.GetMutationAmount(MutationData.targetSearchRadius);
            seeker.attractionForce += mutationData.ValueRW.GetMutationAmount(MutationData.attractionFroce);
        }
    }

    private bool MutationNeeded()
    {
        return mutationData.ValueRW.random.NextFloat(1) < mutationData.ValueRO.mutationChance;
    }
}
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct ReproductionData : IComponentData
{
    public int foodToReproduce;
    public int currentFood;
    public Entity seekerPrefab;
}

public struct MutationData : IComponentData
{
    public float attractionFroce;
    public float repultionFroce;
    public float maxFroce;
    public float foodSearchRadius;
    public float poisonSearchRadius;

    public Unity.Mathematics.Random random;

    public float GetMutationAmount(float value)
    {
        return random.NextFloat(-value, value);
    }
}

public readonly partial struct ReproductionAspect : IAspect
{
    readonly RefRW<MutationData> mutationData;

    readonly RefRO<PhysicsData> physicsData;
    readonly RefRO<Translation> translation;

    readonly DynamicBuffer<SteeringData> steeringData;
    readonly DynamicBuffer<TargetSeeker> targetSeeker;

    private MutationData MutationData => mutationData.ValueRO;

    public void Reproduce(ref EntityCommandBuffer.ParallelWriter ecb, int chunkIndex, Entity seekerPrefab)
    {
        var newUnit = ecb.Instantiate(chunkIndex, seekerPrefab);

        ecb.SetComponent(chunkIndex, newUnit, new PhysicsData
        {
            maxSpeed = physicsData.ValueRO.maxSpeed,
            velocity = -physicsData.ValueRO.velocity
        });
        ecb.SetComponent(chunkIndex, newUnit, translation.ValueRO);

        var newSteeringData = ecb.SetBuffer<SteeringData>(chunkIndex, newUnit);
        newSteeringData.CopyFrom(steeringData);

        newSteeringData.ElementAt(0).attractionForce = steeringData[0].attractionForce + mutationData.ValueRW.GetMutationAmount(MutationData.attractionFroce);
        newSteeringData.ElementAt(0).maxForce = steeringData[0].maxForce + mutationData.ValueRW.GetMutationAmount(MutationData.maxFroce);

        newSteeringData.ElementAt(1).attractionForce = steeringData[1].attractionForce + mutationData.ValueRW.GetMutationAmount(MutationData.repultionFroce);
        newSteeringData.ElementAt(1).maxForce = steeringData[1].maxForce + mutationData.ValueRW.GetMutationAmount(MutationData.maxFroce);

        var newSeeker = ecb.SetBuffer<TargetSeeker>(chunkIndex, newUnit);
        newSeeker.CopyFrom(targetSeeker);

        newSeeker.ElementAt(0).searchRadius = targetSeeker[0].searchRadius + mutationData.ValueRW.GetMutationAmount(MutationData.foodSearchRadius);
        newSeeker.ElementAt(1).searchRadius = targetSeeker[1].searchRadius + mutationData.ValueRW.GetMutationAmount(MutationData.foodSearchRadius);
    }
}
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public struct InactiveState : IComponentData
{
    public float duration;
    public float timer;
}

public struct ReproductionData : IComponentData
{
    public int prefabIndex;
    public int energyToReproduce;
    public float inactiveStateDuration;
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
    readonly RefRO<ReproductionData> reproductionData;
    readonly RefRO<TargetSeeker> targetSeeker;

    readonly RefRW<Energy> energy;
    readonly RefRO<SteeringAgent> steeringData;
    readonly RefRO<PhysicsData> physicsData;
    readonly RefRO<Translation> translation;

    private MutationData MutationData => mutationData.ValueRO;

    public int PrefabIndex => reproductionData.ValueRO.prefabIndex;

    public void ReproduceAndMutate(ref EntityCommandBuffer.ParallelWriter ecb, int chunkIndex, Entity childPrefab)
    {
        if (energy.ValueRO.current >= reproductionData.ValueRO.energyToReproduce)
        {
            energy.ValueRW.current -= reproductionData.ValueRO.energyToReproduce;

            var seeker = ecb.Instantiate(chunkIndex, childPrefab);

            Init(ref ecb, chunkIndex, seeker);

            InheritGenes(ref ecb, chunkIndex, seeker);
        }
    }

    private void Init(ref EntityCommandBuffer.ParallelWriter ecb, int chunkIndex, Entity seeker)
    {
        ecb.SetComponent(chunkIndex, seeker, translation.ValueRO);

        ecb.SetComponent(chunkIndex, seeker, new InactiveState
        {
            duration = reproductionData.ValueRO.inactiveStateDuration
        });
    }

    private void InheritGenes(ref EntityCommandBuffer.ParallelWriter ecb, int chunkIndex, Entity seeker)
    {
        var newPhysicsData = physicsData.ValueRO;
        InheritPhysicsData(ref newPhysicsData);
        ecb.SetComponent(chunkIndex, seeker, newPhysicsData);

        var newSteeringAgent = steeringData.ValueRO;
        InheritSteeringAgent(ref newSteeringAgent);
        ecb.SetComponent(chunkIndex, seeker, newSteeringAgent);

        var newSeeker = targetSeeker.ValueRO;
        InheritSeeker(ref newSeeker);
        ecb.SetComponent(chunkIndex, seeker, newSeeker);
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

    private void InheritSeeker(ref TargetSeeker seeker)
    {
        if (!MutationNeeded())
            return;

        seeker.searchRadius += mutationData.ValueRW.GetMutationAmount(MutationData.targetSearchRadius);
        seeker.attractionForce += mutationData.ValueRW.GetMutationAmount(MutationData.attractionFroce);
    }

    private bool MutationNeeded()
    {
        return mutationData.ValueRW.random.NextFloat(1) < mutationData.ValueRO.mutationChance;
    }
}
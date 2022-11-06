using Unity.Entities;
using UnityEngine;

public class ReproductionAuthoring : MonoBehaviour
{
    public int prefabIndex;

    public int foodToReproduce;

    public float mutationChance;
    public float attractionFroce;
    public float maxSpeed;
    public float maxFroce;
    public float targetSearchRadius;

    public float inactiveStateDuration;

    class ReproductionBaker : Baker<ReproductionAuthoring>
    {
        public override void Bake(ReproductionAuthoring authoring)
        {
            AddComponent(new ReproductionData
            {
                prefabIndex = authoring.prefabIndex,
                energyToReproduce = authoring.foodToReproduce,
                inactiveStateDuration = authoring.inactiveStateDuration
            });

            AddComponent(new MutationData
            {
                mutationChance = authoring.mutationChance,
                attractionFroce = authoring.attractionFroce,
                targetSearchRadius = authoring.targetSearchRadius,
                maxFroce = authoring.maxFroce,
                maxSpeed = authoring.maxSpeed,
                random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Millisecond)
            });
        }
    }
}
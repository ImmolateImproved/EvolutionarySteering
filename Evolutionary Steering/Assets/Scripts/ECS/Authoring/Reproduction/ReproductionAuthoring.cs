using Unity.Entities;
using UnityEngine;

public class ReproductionAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public int foodToReproduce;

    public float attractionFroce;
    public float maxSpeed;
    public float maxFroce;
    public float targetSearchRadius;

    class ReproductionBaker : Baker<ReproductionAuthoring>
    {
        public override void Bake(ReproductionAuthoring authoring)
        {
            AddComponent(new ReproductionData
            {
                seekerPrefab = GetEntity(authoring.prefab),
                foodToReproduce = authoring.foodToReproduce
            });

            AddComponent(new MutationData
            {
                attractionFroce = authoring.attractionFroce,
                targetSearchRadius = authoring.targetSearchRadius,
                maxFroce = authoring.maxFroce,
                maxSpeed = authoring.maxSpeed,
                random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Millisecond)
            });
        }
    }
}
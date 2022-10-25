using Unity.Entities;
using UnityEngine;

public class ReproductionAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public int foodToReproduce;

    public float attractionFroce;
    public float repultionFroce;
    public float maxFroce;
    public float foodSearchRadius;
    public float poisonSearchRadius;

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
                repultionFroce = authoring.repultionFroce,
                foodSearchRadius = authoring.foodSearchRadius,
                poisonSearchRadius = authoring.poisonSearchRadius,
                maxFroce = authoring.maxFroce,
                random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Millisecond)
            });
        }
    }
}
using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

[System.Serializable]
public struct SteeringAgentData
{
    public GameObject target;

    public float attractionForce;
    public float hungerAttractionBonus;
    public float searchRadius;

    public PhysicsCategoryTags targetLayer;
}

public class SteeringAuthoring : MonoBehaviour
{
    public float maxForce;

    public float predictionAmount;

    public PhysicsCategoryTags belongsTo;

    public SteeringAgentData[] seekerDatas;

    class SteeringBaker : Baker<SteeringAuthoring>
    {
        public override void Bake(SteeringAuthoring authoring)
        {
            AddComponent(new TargetInRange());

            AddComponent(new SteeringAgent
            {
                maxForce = authoring.maxForce,
                predictionAmount = authoring.predictionAmount

            });

            var seekerDatas = AddBuffer<TargetSeeker>();

            for (int i = 0; i < authoring.seekerDatas.Length; i++)
            {
                var steeringData = authoring.seekerDatas[i];

                seekerDatas.Add(new TargetSeeker
                {
                    target = GetEntity(steeringData.target),
                    attractionForce = steeringData.attractionForce,
                    hungerAttractionBonus = steeringData.hungerAttractionBonus,
                    searchRadius = steeringData.searchRadius,

                    layers = new Unity.Physics.CollisionFilter
                    {
                        BelongsTo = authoring.belongsTo.Value,
                        CollidesWith = steeringData.targetLayer.Value
                    }
                });
            }
        }
    }
}
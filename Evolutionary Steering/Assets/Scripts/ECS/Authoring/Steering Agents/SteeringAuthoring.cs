using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

[System.Serializable]
public struct SteeringAgentData
{
    public GameObject target;
    public float attractionForce;
    public float maxForce;

    public float predictionAmount;

    public float searchRadius;
    public PhysicsCategoryTags targetLayer;
}

public class SteeringAuthoring : MonoBehaviour
{
    public SteeringAgentData[] seekerDatas;

    public PhysicsCategoryTags belongsTo;

    class SteeringBaker : Baker<SteeringAuthoring>
    {
        public override void Bake(SteeringAuthoring authoring)
        {
            AddComponent(new TargetInRange());
            var seekerDatas = AddBuffer<TargetSeeker>();

            var steeringDatas = AddBuffer<SteeringData>();

            for (int i = 0; i < authoring.seekerDatas.Length; i++)
            {
                var steeringData = authoring.seekerDatas[i];

                steeringDatas.Add(new SteeringData
                {
                    maxForce = steeringData.maxForce,
                    predictionAmount = steeringData.predictionAmount,
                    attractionForce = steeringData.attractionForce
                });

                seekerDatas.Add(new TargetSeeker
                {
                    target = GetEntity(steeringData.target),
                    searchRadius = authoring.seekerDatas[i].searchRadius,
                    layers = new Unity.Physics.CollisionFilter
                    {
                        BelongsTo = authoring.belongsTo.Value,
                        CollidesWith = authoring.seekerDatas[i].targetLayer.Value
                    }
                });
            }
        }
    }
}
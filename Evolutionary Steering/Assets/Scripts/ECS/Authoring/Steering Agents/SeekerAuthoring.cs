using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

[System.Serializable]
public struct SeekerData
{
    public float attractionForce;
    public float hungerAttractionBonus;
    public float predictionAmount;
    public float searchRadius;

    public TargetTypeEnum targetType;
    public PhysicsCategoryTags targetLayer;
}

public class SeekerAuthoring : MonoBehaviour
{
    public float maxSpeed;

    public float maxForce;

    public Vector2 initialVelocity;

    public PhysicsCategoryTags belongsTo;

    public SeekerData[] seekerDatas;

    class SeekerBaker : Baker<SeekerAuthoring>
    {
        public override void Bake(SeekerAuthoring authoring)
        {
            AddComponent(new PhysicsData
            {
                velocity = new Vector3(authoring.initialVelocity.x, authoring.initialVelocity.y, 0),
                maxSpeed = authoring.maxSpeed
            });

            AddComponent(new ResultantForce { });

            AddComponent(new TargetInRange());

            AddComponent(new SteeringAgent
            {
                maxForce = authoring.maxForce

            });

            var seekerDatas = AddBuffer<TargetSeeker>();

            for (int i = 0; i < authoring.seekerDatas.Length; i++)
            {
                var steeringData = authoring.seekerDatas[i];

                seekerDatas.Add(new TargetSeeker
                {
                    attractionForce = steeringData.attractionForce,
                    hungerAttractionBonus = steeringData.hungerAttractionBonus,
                    searchRadius = steeringData.searchRadius,
                    predictionAmount = steeringData.predictionAmount,
                    targetType = steeringData.targetType,

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
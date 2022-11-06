using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

[System.Serializable]
public struct SeekerData
{
    public float unitType;
    public float attractionForce;
    public float hungerAttractionBonus;
    public float predictionAmount;
    public float searchRadius;
    public float foodPreference;
}

public class SeekerAuthoring : MonoBehaviour
{
    public float maxSpeed;

    public float maxForce;

    public Vector2 initialVelocity;

    public SeekerData seekerData;

    class SeekerBaker : Baker<SeekerAuthoring>
    {
        public override void Bake(SeekerAuthoring authoring)
        {
            AddComponent(new InactiveState());

            AddBuffer<AllTargets>();

            AddComponent(new TargetInRange());

            AddComponent(new SteeringAgent
            {
                maxForce = authoring.maxForce
            });

            AddComponent(new PhysicsData
            {
                velocity = new Vector3(authoring.initialVelocity.x, authoring.initialVelocity.y, 0),
                maxSpeed = authoring.maxSpeed
            });

            AddComponent(new ResultantForce { });

            var seekerData = authoring.seekerData;

            AddComponent(new UnitType { value = seekerData.unitType });

            AddComponent(new TargetSeeker
            {
                attractionForce = seekerData.attractionForce,
                hungerAttractionBonus = seekerData.hungerAttractionBonus,
                searchRadius = seekerData.searchRadius,
                predictionAmount = seekerData.predictionAmount,
                foodPreference = seekerData.foodPreference
            });

        }
    }
}
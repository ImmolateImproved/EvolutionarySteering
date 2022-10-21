using Unity.Entities;
using UnityEngine;

public class PhysicsDataAuthoring : MonoBehaviour
{
    public float maxSpeed;

    public Vector2 initialVelocity;

    class PhysicsDataBaker : Baker<PhysicsDataAuthoring>
    {
        public override void Bake(PhysicsDataAuthoring authoring)
        {
            AddComponent(new PhysicsData
            {
                velocity = new Vector3(authoring.initialVelocity.x, authoring.initialVelocity.y, 0),
                maxSpeed = authoring.maxSpeed
            });

            AddComponent(new ResultantForce { });
        }
    }
}
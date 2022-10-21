using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class OutOfBoundSteeringAuthoring : MonoBehaviour
{
    public Vector3 center;
    public float radius;

    public float steeringForce;

    private void OnDrawGizmosSelected()
    {
        Handles.DrawWireDisc(transform.position, Vector3.back, radius, 1);
    }

    class OutOfBoundSteerinBaker : Baker<OutOfBoundSteeringAuthoring>
    {
        public override void Bake(OutOfBoundSteeringAuthoring authoring)
        {
            AddComponent(new OutOfBoundSteering
            {
                center = authoring.center,
                radiusSq = Mathf.Pow(authoring.radius, 2),
                steeringForce = authoring.steeringForce

            });
        }
    }
}
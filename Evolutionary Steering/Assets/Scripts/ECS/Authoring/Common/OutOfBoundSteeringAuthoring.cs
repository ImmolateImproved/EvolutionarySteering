using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class OutOfBoundSteeringAuthoring : MonoBehaviour
{
    public Bounds bounds;
    public float steeringForce;

    class OutOfBoundSteerinBaker : Baker<OutOfBoundSteeringAuthoring>
    {
        public override void Bake(OutOfBoundSteeringAuthoring authoring)
        {
            AddComponent(new OutOfBoundSteering
            {
                squareBounds = authoring.bounds,
                steeringForce = authoring.steeringForce

            });
        }
    }
}
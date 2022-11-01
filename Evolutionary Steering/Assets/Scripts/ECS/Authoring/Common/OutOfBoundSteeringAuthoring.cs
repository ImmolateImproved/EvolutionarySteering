using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class OutOfBoundSteeringAuthoring : MonoBehaviour
{
    public PositionFactoryData positionFactory;

    public Bounds bounds;
    public float steeringForce;

    class OutOfBoundSteerinBaker : Baker<OutOfBoundSteeringAuthoring>
    {
        public override void Bake(OutOfBoundSteeringAuthoring authoring)
        {
            var posFactory = authoring.positionFactory;

            var bounds = authoring.bounds;

            if (posFactory)
            {
                DependsOn(posFactory.transform);

                bounds.center = posFactory.transform.position;
                bounds.extents = posFactory.bounds;
            }

            AddComponent(new OutOfBoundSteering
            {
                squareBounds = bounds,
                steeringForce = authoring.steeringForce

            });
        }
    }
}
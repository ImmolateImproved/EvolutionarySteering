using Unity.Entities;
using UnityEngine;

public class CameraFollowAuthoring : MonoBehaviour
{
    public Vector3 offset;
    public float rotationSpeed;

    class CameraFollowBaker : Baker<CameraFollowAuthoring>
    {
        public override void Bake(CameraFollowAuthoring authoring)
        {
            AddComponent(new CameraFollow
            {
                offset = authoring.offset,
                rotationSpeed = authoring.rotationSpeed
            });
        }
    }
}
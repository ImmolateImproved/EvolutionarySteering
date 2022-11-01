using Unity.Entities;
using UnityEngine;

public class CameraFollowAuthoring : MonoBehaviour
{
    public Vector3 defaultPosition;
    public Vector3 offset;
    public float moveSpeed;
    public float scrollSpeed;
    public bool initEnableState;

    class CameraFollowBaker : Baker<CameraFollowAuthoring>
    {
        public override void Bake(CameraFollowAuthoring authoring)
        {
            AddComponent(new CameraFollow
            {
                offset = authoring.offset,
                enabled = authoring.initEnableState,
                defaultPosition = authoring.defaultPosition,
                moveSpeed = authoring.moveSpeed,
                scrollSpeed = authoring.scrollSpeed
            });
        }
    }
}
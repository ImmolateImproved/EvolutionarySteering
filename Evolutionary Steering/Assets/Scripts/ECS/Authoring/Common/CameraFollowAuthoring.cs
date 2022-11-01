using Unity.Entities;
using UnityEngine;

public class CameraFollowAuthoring : MonoBehaviour
{
    public bool initEnableState;
    public Vector3 offset;

    public Range fromRange;
    public Range moveSpeedRange;
    public Range dragSpeedRange;
    public Range scrollSpeedRange;

    class CameraFollowBaker : Baker<CameraFollowAuthoring>
    {
        public override void Bake(CameraFollowAuthoring authoring)
        {
            AddComponent(new CameraFollow
            {
                enabled = authoring.initEnableState,
                offset = authoring.offset,

                fromRange = authoring.fromRange,
                moveSpeedRange = authoring.moveSpeedRange,
                dragSpeedRange = authoring.dragSpeedRange,
                scrollSpeedRange = authoring.scrollSpeedRange
            });
        }
    }
}
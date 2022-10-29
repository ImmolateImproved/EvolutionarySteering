using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class CameraFollowSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var camera = Camera.main;

        var cameraFollow = GetSingleton<CameraFollow>();

        Entities.WithAll<SteeringAgent>()
            .ForEach((TransformAspect transform) =>
            {
                var offset = transform.TransformDirectionLocalToWorld(cameraFollow.offset);

                var rotation = Quaternion.Slerp(camera.transform.rotation, transform.Rotation, cameraFollow.rotationSpeed * SystemAPI.Time.DeltaTime);

                camera.transform.SetPositionAndRotation(transform.Position + offset, rotation);

            }).WithoutBurst().Run();
    }
}
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial class CameraFollowSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<CameraFollow>();
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (var cameraFollow in SystemAPI.Query<RefRW<CameraFollow>>())
            {
                cameraFollow.ValueRW.enabled = false;
            }
        }

        foreach (var cameraFollow in SystemAPI.Query<RefRW<CameraFollow>>())
        {
            ref var cameraFollowRW = ref cameraFollow.ValueRW;

            var currentTargetExists = HasComponent<Translation>(cameraFollowRW.currentTarget);

            var spacePressed = Input.GetKeyDown(KeyCode.Space);

            if (spacePressed)
            {
                if (!cameraFollowRW.enabled)
                {
                    cameraFollowRW.enabled = true;
                    return;
                }
            }

            if (spacePressed || (!currentTargetExists && cameraFollowRW.enabled))
            {
                var steeringAgentQuery = SystemAPI.QueryBuilder().WithAll<SteeringAgent>().Build().ToEntityArray(Unity.Collections.Allocator.Temp);

                if (steeringAgentQuery.Length <= 0)
                    return;

                var i = 0;
                do
                {
                    i = UnityEngine.Random.Range(0, steeringAgentQuery.Length);

                } while (steeringAgentQuery.Length > 1 && steeringAgentQuery[i] == cameraFollowRW.currentTarget);

                cameraFollowRW.currentTarget = steeringAgentQuery[i];
            }
        }

        var camera = Camera.main.transform;

        Entities.ForEach((ref CameraFollow cameraFollow) =>
        {
            var cameraPos = camera.position;
            var cameraPosY = camera.position.y;

            if (cameraFollow.enabled && HasComponent<Translation>(cameraFollow.currentTarget))
            {
                cameraPos = GetComponent<Translation>(cameraFollow.currentTarget).Value + cameraFollow.offset;
            }

            var dt = SystemAPI.Time.DeltaTime;

            var dy = cameraFollow.GetScrollSpeed(cameraPosY) * dt * Input.mouseScrollDelta;

            if (!cameraFollow.enabled)
            {
                var movement = Vector3.zero;

                movement.x = Input.GetAxisRaw("Horizontal");
                movement.z = Input.GetAxisRaw("Vertical");
                movement.Normalize();
                movement *= cameraFollow.GetMoveSpeed(cameraPosY);

                if (Input.GetMouseButton(0))
                {
                    movement.x -= Input.GetAxisRaw("Mouse X");
                    movement.z -= Input.GetAxisRaw("Mouse Y");
                    movement *= cameraFollow.GetDragSpeed(cameraPosY);
                }

                cameraPos += dt * movement;

                cameraPos.y -= dy.y;
            }
            else
            {
                cameraFollow.offset.y -= dy.y;
            }

            camera.position = cameraPos;

        }).WithoutBurst().Run();
    }
}
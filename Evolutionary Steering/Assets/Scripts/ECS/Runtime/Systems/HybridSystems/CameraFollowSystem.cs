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
        var camera = Camera.main;

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

        Entities.ForEach((ref CameraFollow cameraFollow) =>
        {
            var cameraPos = camera.transform.position;

            if (Input.GetKeyDown(KeyCode.D))
            {
                cameraFollow.enabled = false;
                cameraPos = cameraFollow.defaultPosition;
            }

            if (cameraFollow.enabled && HasComponent<Translation>(cameraFollow.currentTarget))
            {
                cameraPos = GetComponent<Translation>(cameraFollow.currentTarget).Value + cameraFollow.offset;
            }

            camera.transform.position = cameraPos;

        }).WithoutBurst().Run();
    }
}
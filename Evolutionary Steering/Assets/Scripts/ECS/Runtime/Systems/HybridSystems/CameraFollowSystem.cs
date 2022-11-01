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

                var i = UnityEngine.Random.Range(0, steeringAgentQuery.Length);

                cameraFollowRW.currentTarget = steeringAgentQuery[i];
            }
        }

        Entities.ForEach((ref CameraFollow cameraFollow) =>
        {
            if (!cameraFollow.enabled || !HasComponent<Translation>(cameraFollow.currentTarget))
                return;

            camera.transform.position = GetComponent<Translation>(cameraFollow.currentTarget).Value + cameraFollow.offset;

        }).WithoutBurst().Run();
    }
}
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
            var targetExists = HasComponent<Translation>(cameraFollow.ValueRO.target);

            var followEnabled = cameraFollow.ValueRW.enabled;

            if (Input.GetKeyDown(KeyCode.Space) || !targetExists)
            {
                cameraFollow.ValueRW.enabled = true;
                if (targetExists && !followEnabled)
                    return;

                var steeringAgentQuery = SystemAPI.QueryBuilder().WithAll<SteeringAgent>().Build().ToEntityArray(Unity.Collections.Allocator.Temp);

                if (steeringAgentQuery.Length <= 0)
                    return;

                var i = UnityEngine.Random.Range(0, steeringAgentQuery.Length);


                cameraFollow.ValueRW.target = steeringAgentQuery[i];
            }
        }

        Entities.ForEach((ref CameraFollow cameraFollow) =>
        {
            if (!cameraFollow.enabled)
                return;

            if (!HasComponent<Translation>(cameraFollow.target))
                return;

            camera.transform.position = GetComponent<Translation>(cameraFollow.target).Value + cameraFollow.offset;

        }).WithoutBurst().Run();
    }
}
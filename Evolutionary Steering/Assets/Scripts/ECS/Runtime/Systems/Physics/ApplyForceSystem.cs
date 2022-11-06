using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct ApplyForceSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;

        new ApplyForceJob
        {
            dt = dt

        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithNone(typeof(InactiveState))]
    partial struct ApplyForceJob : IJobEntity
    {
        public float dt;

        public void Execute(ref Translation translation, PhysicsBodyAspect physicsBody)
        {
            physicsBody.ApplyForce(ref translation, dt);
        }
    }
}
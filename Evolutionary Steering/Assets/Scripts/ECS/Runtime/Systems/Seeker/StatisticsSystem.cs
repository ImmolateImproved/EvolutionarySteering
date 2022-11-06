using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public struct AverageDNAStats
{
    public float attractionFroce;
    public float repultionForce;
    public float foodSearchRadius;
    public float poisonSearchRadius;
    public float maxSpeed;
    public float maxFroce;
}

public struct UnitInfo
{
    public string target;
    public float energy;
    public float foodPref;
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class StatisticsSystem : SystemBase
{
    private EntityQuery selectedQuery;

    private EntityQuery seekerQuery;

    protected override void OnCreate()
    {
        seekerQuery = GetEntityQuery(typeof(TargetSeeker));
    }

    protected override void OnUpdate()
    {
        var unitInfo = new UnitInfo();
        unitInfo.target = "";

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        Entities.WithNone<SelectorInit>()
            .ForEach((Entity e, ref Selector selector) =>
            {
                if (seekerQuery.CalculateEntityCount() == 0)
                    return;

                var seeker = seekerQuery.ToEntityArray(Unity.Collections.Allocator.Temp)[0];

                EntityManager.AddComponent<Selected>(seeker);

                EntityManager.AddComponent<SelectorInit>(e);

            }).WithStructuralChanges().Run();

        Entities.ForEach((Entity e, ref Selector selector) =>
        {
            if (Input.GetMouseButtonDown(0))
            {
                var input = new RaycastInput
                {
                    Start = ray.origin,
                    End = ray.origin + ray.direction * 3000,
                    Filter = selector.layers
                };

                if (physicsWorld.CastRay(input, out var closets))
                {
                    EntityManager.RemoveComponent<Selected>(selectedQuery);
                    EntityManager.AddComponent<Selected>(closets.Entity);
                }
            }

        }).WithStructuralChanges().Run();

        Entities.WithAll<Selected>()
            .ForEach((in TargetSeeker seeker, in Energy energy) =>
            {
                unitInfo.energy = energy.current;
                unitInfo.foodPref = seeker.foodPreference;
                unitInfo.target = EntityManager.GetName(seeker.target);

            }).WithoutBurst().WithStoreEntityQueryInField(ref selectedQuery).Run();

        var ui = UISingleton.singleton;
        if (ui)
        {
            ui.SetUnitDebugInfo(ref unitInfo);
        }
        //var averageDNAStats = new AverageDNAStats();

        //var seekersCount = entityQuery.CalculateEntityCount();

        //Entities.ForEach((in TargetSeeker seeker, in SteeringAgent steeringAgent, in PhysicsData physicsData) =>
        //{
        //    //averageDNAStats.attractionFroce += seeker[0].attractionForce;
        //    //averageDNAStats.foodSearchRadius += seeker[0].searchRadius;
        //    //averageDNAStats.repultionForce += -seeker[1].attractionForce;
        //    //averageDNAStats.poisonSearchRadius += seeker[1].searchRadius;

        //    averageDNAStats.maxFroce += steeringAgent.maxForce;
        //    averageDNAStats.maxSpeed += physicsData.maxSpeed;

        //}).WithStoreEntityQueryInField(ref entityQuery).Run();

        //averageDNAStats.attractionFroce /= seekersCount;
        //averageDNAStats.repultionForce /= seekersCount;
        //averageDNAStats.foodSearchRadius /= seekersCount;
        //averageDNAStats.poisonSearchRadius /= seekersCount;
        //averageDNAStats.maxFroce /= seekersCount;
        //averageDNAStats.maxSpeed /= seekersCount;

        //UISingleton.singleton.SetText(ref averageDNAStats, seekersCount);
    }
}
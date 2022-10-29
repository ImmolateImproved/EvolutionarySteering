using Unity.Entities;

public struct AverageDNAStats
{
    public float attractionFroce;
    public float repultionForce;
    public float foodSearchRadius;
    public float poisonSearchRadius;
    public float maxSpeed;
    public float maxFroce;
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class StatisticsSystem : SystemBase
{
    private EntityQuery entityQuery;

    protected override void OnUpdate()
    {
        var averageDNAStats = new AverageDNAStats();

        var seekersCount = entityQuery.CalculateEntityCount();

        Entities.ForEach((in DynamicBuffer<TargetSeeker> seeker, in SteeringAgent steeringAgent, in PhysicsData physicsData) =>
        {
            averageDNAStats.attractionFroce += seeker[0].attractionForce;
            averageDNAStats.foodSearchRadius += seeker[0].searchRadius;
            averageDNAStats.repultionForce += -seeker[1].attractionForce;
            averageDNAStats.poisonSearchRadius += seeker[1].searchRadius;

            averageDNAStats.maxFroce += steeringAgent.maxForce;
            averageDNAStats.maxSpeed += physicsData.maxSpeed;

        }).WithStoreEntityQueryInField(ref entityQuery).Run();

        averageDNAStats.attractionFroce /= seekersCount;
        averageDNAStats.repultionForce /= seekersCount;
        averageDNAStats.foodSearchRadius /= seekersCount;
        averageDNAStats.poisonSearchRadius /= seekersCount;
        averageDNAStats.maxFroce /= seekersCount;
        averageDNAStats.maxSpeed /= seekersCount;

        UISingleton.singleton.SetText(ref averageDNAStats, seekersCount);
    }
}
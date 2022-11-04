using Unity.Entities;
using Unity.Physics;

[System.Serializable]
public struct InitialSeekerStats : IComponentData
{
    public Range attarctionFroce;
    public Range repultionForce;
    public Range maxSpeed;
    public Range maxForce;
    public Range foodSearchRadius;
    public Range poisonSearchRadius;

    public Unity.Mathematics.Random random;

    public float GetAttractionForce()
    {
        return random.NextFloat(attarctionFroce.min, attarctionFroce.max);
    }

    public float GetRepultionForce()
    {
        return random.NextFloat(repultionForce.min, repultionForce.max);
    }

    public float GetMaxSpeed()
    {
        return random.NextFloat(maxSpeed.min, maxSpeed.max);
    }

    public float GetMaxForce()
    {
        return random.NextFloat(maxForce.min, maxForce.max);
    }

    public float GetFoodSearchRadius()
    {
        return random.NextFloat(foodSearchRadius.min, foodSearchRadius.max);
    }

    public float GetPoisonSearchRadius()
    {
        return random.NextFloat(poisonSearchRadius.min, poisonSearchRadius.max);
    }
}

[InternalBufferCapacity(0)]
public struct TargetSeeker : IBufferElementData
{
    public Entity target;

    public float attractionForce;
    public float hungerAttractionBonus;

    public float predictionAmount;

    public float searchRadius;

    public float seekTimer;
    public float timeBeforeSeek;

    public TargetTypeEnum targetType;
    public CollisionFilter layers;
}

public enum TargetTypeEnum
{
    Food, Poison
}

public struct UnitType : IComponentData
{
    public TargetTypeEnum value;
}

public struct TargetInRange : IComponentData, IEnableableComponent
{
    public TargetTypeEnum targetType;
}
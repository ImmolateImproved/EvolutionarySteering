using Unity.Entities;
using Unity.Physics;

[System.Serializable]
public struct InitialMutation : IComponentData
{
    public float minAttractionFroce;
    public float maxAttractionFroce;

    public float minRepultionFroce;
    public float maxRepultionFroce;

    public float minSpeed;
    public float maxSpeed;

    public float minForce;
    public float maxForce;

    public float minFoodSearchRadius;
    public float maxFoodSearchRadius;

    public float minPoisonSearchRadius;
    public float maxPoisonSearchRadius;

    public Unity.Mathematics.Random random;

    public float GetAttractionForceMutation()
    {
        return random.NextFloat(minAttractionFroce, maxAttractionFroce);
    }

    public float GetRepultionForceMutation()
    {
        return random.NextFloat(minRepultionFroce, maxRepultionFroce);
    }

    public float GetSpeedMutation()
    {
        return random.NextFloat(minSpeed, maxSpeed);
    }

    public float GetForceMutation()
    {
        return random.NextFloat(minForce, maxForce);
    }

    public float GetFoodSearchRadiusMutation()
    {
        return random.NextFloat(minFoodSearchRadius, maxFoodSearchRadius);
    }

    public float GetPoisonSearchRadiusMutation()
    {
        return random.NextFloat(minPoisonSearchRadius, maxPoisonSearchRadius);
    }
}

public struct TargetSeeker : IBufferElementData
{
    public Entity target;

    public float attractionForce;
    public float hungerAttractionBonus;

    public float searchRadius;

    public float seekTimer;
    public float timeBeforeSeek;

    public CollisionFilter layers;
}

public enum TargetTypeEnum
{
    Food, Poison
}

public struct TargetType : IComponentData
{
    public TargetTypeEnum value;
}

public struct TargetInRange : IComponentData, IEnableableComponent
{
    public TargetTypeEnum targetType;   
}
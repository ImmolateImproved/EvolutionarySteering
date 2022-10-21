using Unity.Entities;
using Unity.Physics;

public struct TargetSeeker : IBufferElementData
{
    public Entity target;

    public float searchRadius;

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
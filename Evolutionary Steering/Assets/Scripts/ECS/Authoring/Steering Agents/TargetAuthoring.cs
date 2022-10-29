using Unity.Entities;
using UnityEngine;

public class TargetAuthoring : MonoBehaviour
{
    public TargetTypeEnum targetType;

    class TargetBaker : Baker<TargetAuthoring>
    {
        public override void Bake(TargetAuthoring authoring)
        {
            AddComponent(new TargetType
            {
                value = authoring.targetType
            });
        }
    }
}
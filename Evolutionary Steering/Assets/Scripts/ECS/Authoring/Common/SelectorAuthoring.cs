using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class SelectorAuthoring : MonoBehaviour
{
    public PhysicsCategoryTags belongsTo;
    public PhysicsCategoryTags collidesWith;

    class SelectorBaker : Baker<SelectorAuthoring>
    {
        public override void Bake(SelectorAuthoring authoring)
        {
            AddComponent(new Selector
            {
                layers = new CollisionFilter
                {
                    BelongsTo = authoring.belongsTo.Value,
                    CollidesWith = authoring.collidesWith.Value
                }
            });
        }
    }
}
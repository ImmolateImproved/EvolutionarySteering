using Unity.Entities;
using UnityEngine;

public class WorldBoundariesAuthoring : MonoBehaviour
{
    public Vector2 bounds;

    class WorldBoundariesBaker : Baker<WorldBoundariesAuthoring>
    {
        public override void Bake(WorldBoundariesAuthoring authoring)
        {
            AddComponent(new SquareWorldBounds
            {
                value = authoring.bounds

            });
        }
    }
}
using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    public float health;
    public float hpPerFood;
    public float hpPerPoison;
    public float hpDecreasePerSeconds;
    public Color fullHpColor;

    class HealthBaker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            AddComponent(new Health
            {
                max = authoring.health,
                current = authoring.health,
                hpPerFood = authoring.hpPerFood,
                hpPerPoison = authoring.hpPerPoison,
                decreasePerSeconds = authoring.hpDecreasePerSeconds,
                fullHpColor = authoring.fullHpColor
            });
        }
    }
}
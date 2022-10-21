using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    public float health;
    public float hpPerKill;
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
                hpPerKill = authoring.hpPerKill,
                decreasePerSeconds = authoring.hpDecreasePerSeconds,
                fullHpColor = authoring.fullHpColor
            });
        }
    }
}
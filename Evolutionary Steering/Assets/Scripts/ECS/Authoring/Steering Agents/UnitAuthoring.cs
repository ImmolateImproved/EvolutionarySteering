using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public float unitType;

    class TargetBaker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            AddComponent(new UnitType
            {
                value = authoring.unitType
            });
        }
    }
}
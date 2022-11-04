using Unity.Entities;
using UnityEngine;

public class UnitPrefabsAuthoring : MonoBehaviour
{
    public GameObject[] unitPrefabs;

    class ChilPrefabBaker : Baker<UnitPrefabsAuthoring>
    {
        public override void Bake(UnitPrefabsAuthoring authoring)
        {
            var unitPrefabs = AddBuffer<UnitPrefab>().Reinterpret<Entity>();

            for (int i = 0; i < authoring.unitPrefabs.Length; i++)
            {
                unitPrefabs.Add(GetEntity(authoring.unitPrefabs[i]));
            }
        }
    }
}
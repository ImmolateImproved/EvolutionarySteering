using Unity.Entities;
using UnityEngine;

public class ChildPrefabAuthoring : MonoBehaviour
{
    public GameObject prefab;

    class ChilPrefabBaker : Baker<ChildPrefabAuthoring>
    {
        public override void Bake(ChildPrefabAuthoring authoring)
        {
            AddComponent(new ChildPrefab
            {
                value = GetEntity(authoring.prefab)
            });
        }
    }
}
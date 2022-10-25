using Unity.Entities;
using UnityEngine;

public class MutationDataAuthoring : MonoBehaviour
{
    public MutationData mutationData;

    public class MutationDataBaker : Baker<MutationDataAuthoring>
    {
        public override void Bake(MutationDataAuthoring authoring)
        {
            var random = new Unity.Mathematics.Random((uint)(System.DateTime.Now.Millisecond + authoring.GetInstanceID()));

            authoring.mutationData.random = random;

            AddComponent(authoring.mutationData);
        }
    }
}
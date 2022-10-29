using Unity.Entities;
using UnityEngine;

public class InitialMutationDataAuthoring : MonoBehaviour
{
    public InitialMutation mutationData;

    public class MutationDataBaker : Baker<InitialMutationDataAuthoring>
    {
        public override void Bake(InitialMutationDataAuthoring authoring)
        {
            var random = new Unity.Mathematics.Random((uint)(System.DateTime.Now.Millisecond + authoring.GetInstanceID()));

            authoring.mutationData.random = random;

            AddComponent(authoring.mutationData);
        }
    }
}
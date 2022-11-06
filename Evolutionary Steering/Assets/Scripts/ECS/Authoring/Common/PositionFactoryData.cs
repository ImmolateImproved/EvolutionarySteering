using UnityEngine;

public enum PositionFactoryEnum
{
    Square, Circle
}

public class PositionFactoryData : MonoBehaviour
{
    public PositionFactoryEnum factoryType;

    public Vector3 bounds;

    public float maxRadius;
}
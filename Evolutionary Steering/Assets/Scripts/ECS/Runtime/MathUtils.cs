using Unity.Mathematics;

public static class MathUtils
{
    public static float3 SetMagnitude(float3 vector, float length)
    {
        return math.normalizesafe(vector) * length;
    }

    public static float3 ClampMagnitude(float3 vector, float maxLength)
    {
        if (math.lengthsq(vector) > maxLength * maxLength)
            return SetMagnitude(vector, maxLength);

        return vector;
    }
}
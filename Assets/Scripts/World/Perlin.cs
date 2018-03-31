using UnityEngine;

public class Perlin
{
    private static float smoothness = 0.01f;

    public static float fractalBrownianMotion(float x, float y, int octaves, float persistance)
    {
        float total = 0.0f;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency * smoothness, y * frequency * smoothness) * amplitude;
            maxValue += amplitude;
            amplitude += persistance;
            frequency *= 2;
        }

        return total / maxValue;
    }
}

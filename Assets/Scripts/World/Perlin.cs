using UnityEngine;

public class Perlin
{
    private static float smoothness = 0.01f;

    public static float fractalBrownianMotion(float x, float y, int octaves, float persistance)
    {
        return fractalBrownianMotion(x, y, octaves, persistance, smoothness);
    }

    public static float fractalBrownianMotion(float x, float y, int octaves, float persistance, float smooth)
    {
        float total = 0.0f;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency * smooth, y * frequency * smooth) * amplitude;
            maxValue += amplitude;
            amplitude += persistance;
            frequency *= 2;
        }

        return total / maxValue;
    }

    public static float cavePerlin(float x, float y, float z)
    {
        float smoothness = 0.01f;
        float xy = fractalBrownianMotion(x, y, 3, 0.5f, smoothness);
        float yx = fractalBrownianMotion(y, x, 3, 0.5f, smoothness);
        float xz = fractalBrownianMotion(x, z, 3, 0.5f, smoothness);

        float zx = fractalBrownianMotion(z, x, 3, 0.5f, smoothness);
        float zy = fractalBrownianMotion(z, y, 3, 0.5f, smoothness);
        float yz = fractalBrownianMotion(y, z, 3, 0.5f, smoothness);

        return (xy + yx + xz + zx + zy + yz) / 6.0f;
    }
}

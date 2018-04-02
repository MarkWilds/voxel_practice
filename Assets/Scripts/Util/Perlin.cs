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

    public static float cavePerlin(float x, float y, float z, int octave, float smooth)
    {
        float xy = fractalBrownianMotion(x, y, octave, 0.5f, smooth);
        float yx = fractalBrownianMotion(y, x, octave, 0.5f, smooth);
        float xz = fractalBrownianMotion(x, z, octave, 0.5f, smooth);

        float zx = fractalBrownianMotion(z, x, octave, 0.5f, smooth);
        float zy = fractalBrownianMotion(z, y, octave, 0.5f, smooth);
        float yz = fractalBrownianMotion(y, z, octave, 0.5f, smooth);

        return (xy + yx + xz + zx + zy + yz) / 6.0f;
    }
}

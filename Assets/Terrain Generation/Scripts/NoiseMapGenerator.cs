using UnityEngine;

public static class NoiseMapGenerator
{
    public static float[,] NoiseGeneration(int width, int height, int seed, float noiseScale, int octaves, float amplitude, float frequency, float lacunarity, float persistence, Vector2 offset, float minTerrainLevel, float maxTerrainLevel)
    {
        Random.InitState(seed);

        Vector2[] offsets = new Vector2[octaves];

        for (int i = 0; i < offsets.Length; i++)
        {
            offsets[i] = new Vector2(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f)) + offset;
        }

        if (noiseScale == 0)
        {
            noiseScale = 0.001f;
        }

        float[,] noisemap = new float[width + 1, height + 1];

        float minNoiseValue = float.MaxValue;
        float maxNoiseValue = float.MinValue;

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                float tempNoiseValue = 0;
                float internalAmplitude = amplitude;
                float internalFrequency = frequency;
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / noiseScale * internalFrequency + offsets[i].x;
                    float sampleY = y / noiseScale * internalFrequency + offsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    tempNoiseValue += perlinValue * internalAmplitude;

                    internalAmplitude *= persistence;
                    internalFrequency *= lacunarity;
                }

                if (tempNoiseValue > maxNoiseValue)
                {
                    maxNoiseValue = tempNoiseValue;
                }
                if (tempNoiseValue < minNoiseValue)
                {
                    minNoiseValue = tempNoiseValue;
                }

                noisemap[x, y] = tempNoiseValue;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noisemap[x, y] = Mathf.InverseLerp(minNoiseValue, maxNoiseValue, noisemap[x, y]);

                float tempNoiseValue = noisemap[x, y];

                if (tempNoiseValue < minTerrainLevel)
                {
                    tempNoiseValue = minTerrainLevel;
                }
                else if (tempNoiseValue > maxTerrainLevel)
                {
                    tempNoiseValue = maxTerrainLevel;
                }

                noisemap[x, y] = tempNoiseValue;
            }
        }

        return noisemap;
    }

    public static float[,] NoiseGeneration(int width, int height, float scale)
    {
        float[,] noisemap = new float[width + 1, height + 1];

        if (scale == 0)
        {
            scale = 0.001f;
        }

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);

                noisemap[x, y] = perlinValue;
            }
        }

        return noisemap;
    }

    public static float[,] SetBorderHeight(float[,] heightmap, float borderHeight = 0)
    {
        for (int y = 0; y < heightmap.GetLength(1); y++)
        {
            for (int x = 0; x < heightmap.GetLength(0); x++)
            {
                if (x == 0 || y == 0 || x == heightmap.GetLength(0) - 1 || y == heightmap.GetLength(1) - 1)
                {
                    heightmap[x, y] = 0;
                }
            }
        }

        return heightmap;
    }

    public static float[,] GetNoiseFromTexture(Texture2D heightmap)
    {
        float[,] noiseValues = new float[heightmap.width, heightmap.height];
        for (int y = 0; y < heightmap.height; y++)
        {
            for (int x = 0; x < heightmap.width; x++)
            {
                noiseValues[x,y] = heightmap.GetPixel(x, y).grayscale;
            }
        }

        return noiseValues;
    }
}
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
    #region TerrainSettings

    [Header("Generation Settings")]
    [SerializeField] [Tooltip("Stops `OnValidate` generation to allow for value tweaking without performance drops.")] private bool editMode = false;

    [SerializeField] private int width;

    [SerializeField] private int height;
    [SerializeField] private bool useHeightScale;
    [SerializeField] private int heightScale;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private bool nullBorders;
    [SerializeField] private HeightmapMode heightmapMode;
    [SerializeField] private Texture2D preDefHeightmap;

    [Space]
    [Header("Noise Settings")]
    [Space]
    [Header("Base Noise")]
    [SerializeField] private float noiseScale;

    [Space]
    [Header("Detailed Noise")]
    [SerializeField] private int seed;

    [SerializeField] [Range(1, 8)] private int octaves;
    [SerializeField] [Range(1f, 3f)] private float amplitude;
    [SerializeField] [Range(1f, 3f)] private float frequency;
    [SerializeField] [Range(1f, 8f)] private float lacunarity;
    [SerializeField] [Range(0f, 1f)] private float persistence;
    [SerializeField] private Vector2 offset;
    [SerializeField] [Range(0f, 1f)] private float minLevel;
    [SerializeField] [Range(0f, 1f)] private float maxLevel;

    [SerializeField] private FilterMode filterMode;

    [Space]

    #endregion TerrainSettings

    #region TerrainComponents

    [Header("Materials")]
    [SerializeField] private Material texturedMaterial;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    #endregion TerrainComponents

    #region Enums

    private enum HeightmapMode
    { Premade, BasePerlin, DetailedPerlin }

    #endregion Enums

    #region Internal Values

    private int oldHeight = -1;
    private int oldWidth = -1;

    #endregion Internal Values

    private void Awake()
    {
        if (!Application.isPlaying)
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();

            meshFilter.sharedMesh = new Mesh();
        }
    }

    //private void Start()
    //{
    //    GenerateTerrain();
    //}

    public void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (!editMode)
            {
                meshFilter = GetComponent<MeshFilter>();
                meshRenderer = GetComponent<MeshRenderer>();
                meshCollider = GetComponent<MeshCollider>();

                if (meshFilter.sharedMesh == null)
                {
                    meshFilter.sharedMesh = new Mesh();
                }
                GenerateTerrain();
            }
        }
    }

    [ContextMenu("Generate new Terrain")]
    public void GenerateTerrain()
    {

        float[,] heightMap = new float[0, 0];

        switch (heightmapMode)
        {
            case HeightmapMode.Premade:
                if (preDefHeightmap == null)
                {
                    Debug.LogWarning("No Heightmap Set!");
                    break;
                }
                if (oldHeight == -1)
                {
                    oldHeight = height;
                }
                if (oldWidth == -1)
                {
                    oldWidth = width;
                }
                height = preDefHeightmap.height - 1;
                width = preDefHeightmap.width - 1;
                heightMap = NoiseMapGenerator.GetNoiseFromTexture(preDefHeightmap);
                break;

            case HeightmapMode.BasePerlin:
                if (oldHeight != -1)
                {
                    height = oldHeight;
                    oldHeight = -1;
                }
                if (oldWidth != -1)
                {
                    width = oldWidth;
                    oldWidth = -1;
                }
                heightMap = NoiseMapGenerator.NoiseGeneration(width, height, noiseScale);
                break;

            case HeightmapMode.DetailedPerlin:
                if (oldHeight != -1)
                {
                    height = oldHeight;
                    oldHeight = -1;
                }
                if (oldWidth != -1)
                {
                    width = oldWidth;
                    oldWidth = -1;
                }
                heightMap = NoiseMapGenerator.NoiseGeneration(width, height, seed, noiseScale, octaves, amplitude, frequency, lacunarity, persistence, offset, minLevel, maxLevel);
                break;

            default:
                Debug.LogWarning("NoiseGen mode selection did not work!");
                return;
        }

        if (heightMap.Length == 0)
        {
            Debug.LogWarning("NoiseGen did not work!");
            return;
        }

        if (nullBorders)
        {
            heightMap = NoiseMapGenerator.SetBorderHeight(heightMap);
        }

        meshFilter.sharedMesh.Clear();

        if (width * height > 65535)
        {
            meshFilter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        else
        {
            meshFilter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
        }

        GenerateMesh(heightMap);

        meshRenderer.sharedMaterial = texturedMaterial;
    }

    private void GenerateMesh(float[,] heightMap)
    {
        GenerateVertices(heightMap);
        GenerateTriangles();
        GenerateUVs();

        meshFilter.sharedMesh.RecalculateNormals();

        meshFilter.sharedMesh.RecalculateBounds();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
        meshFilter.sharedMesh.RecalculateTangents();
    }

    private void GenerateVertices(float[,] heightMap)
    {
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];

        for (int y = 0, i = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                float tempHeight = useHeightScale ? heightCurve.Evaluate(heightMap[x, y]) * heightScale : 0;
                vertices[i] = new Vector3(x, tempHeight, y);
            }
        }

        meshFilter.sharedMesh.vertices = vertices;
    }

    private void GenerateTriangles()
    {
        int[] triangles = new int[width * height * 6];

        for (int y = 0, triCounter = 0, vertCounter = 0; y < height; y++, vertCounter++)
        {
            for (int x = 0; x < width; x++, triCounter += 6, vertCounter++)
            {
                triangles[triCounter] = vertCounter;
                triangles[triCounter + 1] = vertCounter + width + 1;
                triangles[triCounter + 2] = vertCounter + 1;

                triangles[triCounter + 3] = vertCounter + 1;
                triangles[triCounter + 4] = vertCounter + width + 1;
                triangles[triCounter + 5] = vertCounter + width + 2;
            }
        }

        meshFilter.sharedMesh.triangles = triangles;
    }

    private void GenerateUVs()
    {
        Vector2[] uvs = new Vector2[(width + 1) * (height + 1)];

        for (int y = 0, i = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++, i++)
            {
                uvs[i] = new Vector2(x / (float)width, y / (float)height);
            }
        }

        meshFilter.sharedMesh.uv = uvs;
    }
}
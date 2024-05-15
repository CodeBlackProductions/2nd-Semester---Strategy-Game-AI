using System.Collections.Generic;
using UnityEngine;

namespace TileMap
{
    public class TileMapController : MonoBehaviour
    {
        #region Fields

        [Header("Base Grid Settings")]
        [SerializeField] private int gridWidth;

        [SerializeField] private int gridHeight;

        [SerializeField] private int seed;

        [Header("Base Tile Settings")]
        [SerializeField] private float tileSize;

        [SerializeField] private float tileOffset;

        [Space(10)]
        [SerializeField] private GameObject baseTileVisuals;

        [Space(10)]
        [Header("Terrain Settings")]
        [SerializeField] private GameObject plainsVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Every Tile that is `none` will be set to `Plains`. All Base-Chances added shouldn`t be more than 100%.")] private int baseHillChance = 0;

        [SerializeField] [Tooltip("Chance-bonus for neighbouring Tiles to become the same terrain type")] private int growthHillChance = 12;

        [SerializeField] private GameObject hillVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Every Tile that is `none` will be set to `Plains`. All Base-Chances added shouldn`t be more than 100%.")] private int baseMountainChance = 6;

        [SerializeField] [Tooltip("Chance-bonus for neighbouring Tiles to become the same terrain type")] private int growthMountainChance = 25;

        [SerializeField] private GameObject mountainVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Every Tile that is `none` will be set to `Plains`. All Base-Chances added shouldn`t be more than 100%.")] private int baseWaterChance = 3;

        [SerializeField] [Tooltip("Chance-bonus for neighbouring Tiles to become the same terrain type")] private int growthWaterChance = 35;

        [SerializeField] private GameObject waterVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("How often should the generation `grow` each terrain and biome before going on?")] private int numberOfTerrainGrowthRuns = 3;

        [Space(10)]
        [Header("Biome Settings")]
        [SerializeField] [Tooltip("Every Tile that is `none` will be set to `Grassland`. All Base-Chances added shouldn`t be more than 100%.")] private int baseForestChance = 6;

        [SerializeField] [Tooltip("Chance-bonus for neighbouring Tiles to become the same biome type")] private int growthForestChance = 12;

        [SerializeField] private GameObject forestVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Every Tile that is `none` will be set to `Grassland`. All Base-Chances added shouldn`t be more than 100%.")] private int baseSwampChance = 1;

        [SerializeField] [Tooltip("Chance-bonus for neighbouring Tiles to become the same biome type")] private int growthSwampChance = 10;

        [SerializeField] private GameObject swampVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Every Tile that is `none` will be set to `Grassland`. All Base-Chances added shouldn`t be more than 100%.")] private int baseDesertChance = 3;

        [SerializeField] [Tooltip("Chance-bonus for neighbouring Tiles to become the same biome type")] private int growthDesertChance = 25;

        [SerializeField] private GameObject desertVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("How often should the generation `grow` each terrain and biome before going on?")] private int numberOfBiomeGrowthRuns = 6;

        [Space(10)]
        [Header("Map Population Settings")]
        [SerializeField] [Tooltip("Chance for a Node to spawn. Will be checked before Node-Chances to determine if any Node spawns at all.")] private int baseNodeChance = 40;

        [Space(10)]
        [SerializeField] [Tooltip("Spawns in Plains + Grassland/ Desert.")] private int baseVillageChance = 40;

        [SerializeField] [Tooltip("The range of neighbours, not allowed to have the same node.")] private int spreadFactorVillage = 2;

        [SerializeField] private GameObject villageVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Spawns in Plains + Grassland/ Desert.")] private int baseCityChance = 30;

        [SerializeField] [Tooltip("The range of neighbours, not allowed to have the same node.")] private int spreadFactorCity = 4;

        [SerializeField] private GameObject cityVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Spawns in Plains/ Hills + Grassland/ Desert.")] private int baseOutpostChance = 20;

        [SerializeField] [Tooltip("The range of neighbours, not allowed to have the same node.")] private int spreadFactorOutpost = 2;

        [SerializeField] private GameObject outpostVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Spawns in Plains/ Hills + Grassland/ Desert.")] private int baseCastleChance = 100;

        [SerializeField] [Tooltip("The range of neighbours, not allowed to have the same node.")] private int spreadFactorCastle = 7;

        [SerializeField] private GameObject castleVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Spawns in Plains + Grassland/ Desert.")] private int baseTraderChance = 10;

        [SerializeField] [Tooltip("The range of neighbours, not allowed to have the same node.")] private int spreadFactorTrader = 8;

        [SerializeField] private GameObject traderVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Spawns in Plains + Swamp.")] private int baseCovenChance = 75;

        [SerializeField] [Tooltip("The range of neighbours, not allowed to have the same node.")] private int spreadFactorCoven = 8;

        [SerializeField] private GameObject covenVisuals;

        [Space(10)]
        [SerializeField] [Tooltip("Spawns in Plains + Forest or Hills + Grassland/Forest.")] private int baseBanditCampChance = 50;

        [SerializeField] [Tooltip("The range of neighbours, not allowed to have the same node.")] private int spreadFactorBanditCamp = 2;

        [SerializeField] private GameObject banditCampVisuals;

        public float TileSize { get => tileSize;}
        public float TileOffset { get => tileOffset;}

        #endregion Fields

        #region Structs

        public Tile[,] tileGrid;

        public struct Tile
        {
            public enum TerrainType
            { None, Plains, Hills, Mountains, Water }

            public enum BiomeType
            { None, Grassland, Forest, Swamp, Desert, Mountains, Water }

            public enum NodeType
            {
                None, Village, City, Outpost, Castle, Trader, Coven, BanditCamp
            }

            public Vector2 gridCoordinates;
            public Dictionary<TerrainType, int> terrainChances;
            public TerrainType terrainType;
            public GameObject terrainVisuals;

            public Dictionary<BiomeType, int> biomeChances;
            public BiomeType biomeType;
            public GameObject biomeVisuals;

            public Dictionary<NodeType, int> nodeChances;
            public NodeType node;
            public GameObject NodeVisuals;

            public bool playerOwned;
            public GameObject owningCastle;
            public GameObject visualRootObject;
            public float tileStrength;
        }

        public enum TypeSelection
        { Terrain, Biome, Node }

        #endregion Structs

        public void Start()
        {
            if (seed == 0)
            {
                seed = Random.Range(-10000, 10000);
            }

            Random.InitState(seed);

            GenerateGrid();
            GenerateMap();
            PopulateMap();

            for (int row = 0; row < gridHeight; row++)
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    SpawnVisuals(tileGrid[column, row]);
                }
            }
        }

        #region BaseSetup

        /// <summary>
        /// Generates the base grid.
        /// </summary>
        private void GenerateGrid()
        {
            tileGrid = new Tile[gridHeight, gridWidth];
            for (int row = 0; row < gridHeight; row++)
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    tileGrid[column, row] = new Tile();
                    SetupBaseStats(tileGrid[column, row], new Vector2(column, row));
                }
            }
        }

        /// <summary>
        /// Pushes all user setting onto the new tiles.
        /// </summary>
        /// <param name="tile">Tile to setup.</param>
        /// <param name="gridPosition">Grid position of the tile thats being edited.</param>
        private void SetupBaseStats(Tile tile, Vector2 gridPosition)
        {
            Tile tileToSetup = tile;

            tileToSetup.gridCoordinates = gridPosition;

            tileToSetup.terrainChances = new Dictionary<Tile.TerrainType, int>();
            tileToSetup.terrainChances.Add(Tile.TerrainType.Hills, baseHillChance);
            tileToSetup.terrainChances.Add(Tile.TerrainType.Mountains, baseMountainChance);
            tileToSetup.terrainChances.Add(Tile.TerrainType.Water, baseWaterChance);

            tileToSetup.terrainType = Tile.TerrainType.None;
            tileToSetup.terrainVisuals = null;

            tileToSetup.biomeChances = new Dictionary<Tile.BiomeType, int>();
            tileToSetup.biomeChances.Add(Tile.BiomeType.Forest, baseForestChance);
            tileToSetup.biomeChances.Add(Tile.BiomeType.Swamp, baseSwampChance);
            tileToSetup.biomeChances.Add(Tile.BiomeType.Desert, baseDesertChance);

            tileToSetup.biomeType = Tile.BiomeType.None;
            tileToSetup.biomeVisuals = null;

            tileToSetup.nodeChances = new Dictionary<Tile.NodeType, int>();
            tileToSetup.nodeChances.Add(Tile.NodeType.Village, baseVillageChance);
            tileToSetup.nodeChances.Add(Tile.NodeType.City, baseCityChance);
            tileToSetup.nodeChances.Add(Tile.NodeType.Outpost, baseOutpostChance);
            tileToSetup.nodeChances.Add(Tile.NodeType.Castle, baseCastleChance);
            tileToSetup.nodeChances.Add(Tile.NodeType.Trader, baseTraderChance);
            tileToSetup.nodeChances.Add(Tile.NodeType.Coven, baseCovenChance);
            tileToSetup.nodeChances.Add(Tile.NodeType.BanditCamp, baseBanditCampChance);

            tileToSetup.node = Tile.NodeType.None;
            tileToSetup.NodeVisuals = null;

            tileToSetup.playerOwned = false;
            tileToSetup.owningCastle = null;
            tileToSetup.tileStrength = 0;

            tileGrid[(int)gridPosition.x, (int)gridPosition.y] = tileToSetup;
        }

        #endregion BaseSetup

        #region Map Generation

        private void GenerateMap()
        {
            for (int row = 0; row < gridHeight; row++)
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    GenerateMapTerrain(tileGrid[column, row]);
                }
            }

            for (int i = 0; i < numberOfTerrainGrowthRuns; i++)
            {
                GrowMapTerrain();
            }

            for (int row = 0; row < gridHeight; row++)
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    GenerateMapBiome(tileGrid[column, row]);
                }
            }

            for (int i = 0; i < numberOfBiomeGrowthRuns; i++)
            {
                GrowMapBiome();
            }
        }

        /// <summary>
        /// Generates the "Cores" of biomes.
        /// </summary>
        /// <param name="tile"></param>
        private void GenerateMapTerrain(Tile tile)
        {
            Tile tileToSetup = tile;

            int randNr = Random.Range(0, 100);
            int waterRange = 100 - tileToSetup.terrainChances[Tile.TerrainType.Water];
            int mountainsRange = waterRange - tileToSetup.terrainChances[Tile.TerrainType.Mountains];
            int hillsRange = mountainsRange - tileToSetup.terrainChances[Tile.TerrainType.Hills];

            if (randNr > waterRange && !CheckType(tileToSetup, (int)Tile.TerrainType.Mountains, false))
            {
                tileToSetup.terrainType = Tile.TerrainType.Water;
                tileToSetup.biomeType = Tile.BiomeType.Water;
                ChangeChances(tileToSetup, (int)Tile.TerrainType.Water, growthWaterChance, TypeSelection.Terrain, false, 1);
                ChangeChances(tileToSetup, (int)Tile.TerrainType.Mountains, 0, TypeSelection.Terrain, true, 1);
            }
            else if (randNr > mountainsRange && !CheckType(tileToSetup, (int)Tile.TerrainType.Water, false))
            {
                tileToSetup.terrainType = Tile.TerrainType.Mountains;
                tileToSetup.biomeType = Tile.BiomeType.Mountains;
                ChangeChances(tileToSetup, (int)Tile.TerrainType.Mountains, growthMountainChance, TypeSelection.Terrain, false, 1);
                ChangeChances(tileToSetup, (int)Tile.TerrainType.Hills, growthHillChance, TypeSelection.Terrain, false, 1);
                ChangeChances(tileToSetup, (int)Tile.TerrainType.Water, 0, TypeSelection.Terrain, true, 1);
            }
            else if (randNr > hillsRange)
            {
                tileToSetup.terrainType = Tile.TerrainType.Hills;
                ChangeChances(tileToSetup, (int)Tile.TerrainType.Hills, growthHillChance, TypeSelection.Terrain, false, 1);
            }
            else
            {
                tileToSetup.terrainType = Tile.TerrainType.Plains;
            }

            tileGrid[(int)tileToSetup.gridCoordinates.x, (int)tileToSetup.gridCoordinates.y] = tileToSetup;
        }

        private void GrowMapTerrain()
        {
            for (int row = 0; row < gridHeight; row++)
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    Tile tileToGrow = tileGrid[column, row];

                    if (tileToGrow.terrainType == Tile.TerrainType.None)
                    {
                        Debug.LogWarning("All tiles should have a terrain by now! Plains Added.");
                        tileToGrow.terrainType = Tile.TerrainType.Plains;
                        tileGrid[(int)tileToGrow.gridCoordinates.x, (int)tileToGrow.gridCoordinates.y] = tileToGrow;
                    }
                    else if (tileToGrow.terrainType != Tile.TerrainType.Plains)
                    {
                        GrowMapTileTerrain(tileToGrow);
                    }
                }
            }
        }

        private void GrowMapTileTerrain(Tile tile)
        {
            Tile tileToGrow = tile;
            Tile[] neighbours;

            neighbours = GetNeighbours(tile, 1);

            for (int i = 0; i < neighbours.Length; i++)
            {
                TerrainGrowth(tileToGrow, neighbours[i]);
            }
        }

        private void TerrainGrowth(Tile tileToGrow, Tile neighbour)
        {
            if (neighbour.terrainType == Tile.TerrainType.Plains)
            {
                int randNr = Random.Range(0, 100);
                int waterRange = 100 - neighbour.terrainChances[Tile.TerrainType.Water];
                int mountainsRange = 100 - neighbour.terrainChances[Tile.TerrainType.Mountains];
                int hillsRange = 100 - neighbour.terrainChances[Tile.TerrainType.Hills];

                switch (tileToGrow.terrainType)
                {
                    case Tile.TerrainType.None:
                        Debug.LogError("This Tile should not be Growing!");
                        break;

                    case Tile.TerrainType.Plains:
                        Debug.LogError("This Tile should not be Growing!");
                        break;

                    case Tile.TerrainType.Hills:
                        if (randNr > hillsRange)
                        {
                            neighbour.terrainType = Tile.TerrainType.Hills;
                            ChangeChances(neighbour, (int)Tile.TerrainType.Hills, growthHillChance, TypeSelection.Terrain, false, 1);
                        }
                        break;

                    case Tile.TerrainType.Mountains:
                        if (randNr > mountainsRange && CheckType(neighbour, (int)Tile.TerrainType.Water, false))
                        {
                            neighbour.terrainType = Tile.TerrainType.Mountains;
                            neighbour.biomeType = Tile.BiomeType.Mountains;
                            ChangeChances(neighbour, (int)Tile.TerrainType.Mountains, growthMountainChance, TypeSelection.Terrain, false, 1);
                            ChangeChances(neighbour, (int)Tile.TerrainType.Hills, growthHillChance, TypeSelection.Terrain, false, 1);
                            ChangeChances(neighbour, (int)Tile.TerrainType.Water, 0, TypeSelection.Terrain, true, 1);
                        }
                        if (randNr > hillsRange)
                        {
                            neighbour.terrainType = Tile.TerrainType.Hills;
                            ChangeChances(neighbour, (int)Tile.TerrainType.Hills, growthHillChance, TypeSelection.Terrain, false, 1);
                        }
                        break;

                    case Tile.TerrainType.Water:
                        if (randNr > waterRange && CheckType(neighbour, (int)Tile.TerrainType.Mountains, false))
                        {
                            neighbour.terrainType = Tile.TerrainType.Water;
                            neighbour.biomeType = Tile.BiomeType.Water;
                            ChangeChances(neighbour, (int)Tile.TerrainType.Water, growthWaterChance, TypeSelection.Terrain, false, 1);
                            ChangeChances(neighbour, (int)Tile.TerrainType.Mountains, 0, TypeSelection.Terrain, true, 1);
                        }
                        break;
                }

                tileGrid[(int)neighbour.gridCoordinates.x, (int)neighbour.gridCoordinates.y] = neighbour;
            }
        }

        private void GenerateMapBiome(Tile tile)
        {
            Tile tileToSetup = tile;

            if (tile.biomeType != Tile.BiomeType.Mountains && tile.biomeType != Tile.BiomeType.Water)
            {
                int randNr = Random.Range(0, 100);
                int forestRange = 100 - tileToSetup.biomeChances[Tile.BiomeType.Forest];
                int swampRange = forestRange - tileToSetup.biomeChances[Tile.BiomeType.Swamp];
                int desertRange = swampRange - tileToSetup.biomeChances[Tile.BiomeType.Desert];

                if (randNr > forestRange && !CheckType(tileToSetup, (int)Tile.BiomeType.Desert, true))
                {
                    tileToSetup.biomeType = Tile.BiomeType.Forest;
                    ChangeChances(tileToSetup, (int)Tile.BiomeType.Forest, growthForestChance, TypeSelection.Biome, false, 1);
                    ChangeChances(tileToSetup, (int)Tile.BiomeType.Desert, 0, TypeSelection.Biome, true, 1);
                }
                else if (randNr > swampRange && tile.terrainType != Tile.TerrainType.Hills && !CheckType(tileToSetup, (int)Tile.BiomeType.Desert, true))
                {
                    tileToSetup.biomeType = Tile.BiomeType.Swamp;
                    ChangeChances(tileToSetup, (int)Tile.BiomeType.Swamp, growthSwampChance, TypeSelection.Biome, false, 1);
                    ChangeChances(tileToSetup, (int)Tile.BiomeType.Desert, 0, TypeSelection.Biome, true, 1);
                }
                else if (randNr > desertRange && !CheckType(tileToSetup, (int)Tile.BiomeType.Swamp, true) && !CheckType(tileToSetup, (int)Tile.BiomeType.Forest, true))
                {
                    tileToSetup.biomeType = Tile.BiomeType.Desert;
                    ChangeChances(tileToSetup, (int)Tile.BiomeType.Desert, growthDesertChance, TypeSelection.Biome, false, 1);
                    ChangeChances(tileToSetup, (int)Tile.BiomeType.Forest, 0, TypeSelection.Biome, true, 1);
                    ChangeChances(tileToSetup, (int)Tile.BiomeType.Swamp, 0, TypeSelection.Biome, true, 1);
                }
                else
                {
                    tileToSetup.biomeType = Tile.BiomeType.Grassland;
                }

                tileGrid[(int)tileToSetup.gridCoordinates.x, (int)tileToSetup.gridCoordinates.y] = tileToSetup;
            }
        }

        private void GrowMapBiome()
        {
            for (int row = 0; row < gridHeight; row++)
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    Tile tileToGrow = tileGrid[column, row];

                    if (tileToGrow.biomeType == Tile.BiomeType.None)
                    {
                        Debug.LogWarning("All tiles should have a biome by now! Grassland Added.");
                        tileToGrow.biomeType = Tile.BiomeType.Grassland;
                        tileGrid[(int)tileToGrow.gridCoordinates.x, (int)tileToGrow.gridCoordinates.y] = tileToGrow;
                    }
                    else if (tileToGrow.biomeType != Tile.BiomeType.Water && tileToGrow.biomeType != Tile.BiomeType.Mountains && tileToGrow.biomeType != Tile.BiomeType.Grassland)
                    {
                        GrowMapTileBiome(tileToGrow);
                    }
                }
            }
        }

        private void GrowMapTileBiome(Tile tile)
        {
            Tile tileToGrow = tile;
            Tile[] neighbours;

            neighbours = GetNeighbours(tile, 1);

            for (int i = 0; i < neighbours.Length; i++)
            {
                BiomeGrowth(tileToGrow, neighbours[i]);
            }
        }

        private void BiomeGrowth(Tile tileToGrow, Tile neighbour)
        {
            if (neighbour.biomeType == Tile.BiomeType.Grassland)
            {
                int randNr = Random.Range(0, 100);
                int forestRange = 100 - tileToGrow.biomeChances[Tile.BiomeType.Forest];
                int swampRange = 100 - tileToGrow.biomeChances[Tile.BiomeType.Swamp];
                int desertRange = 100 - tileToGrow.biomeChances[Tile.BiomeType.Desert];

                switch (tileToGrow.biomeType)
                {
                    case Tile.BiomeType.None:
                        Debug.LogError("This Tile should not be Growing!");
                        break;

                    case Tile.BiomeType.Grassland:
                        Debug.LogError("This Tile should not be Growing!");
                        break;

                    case Tile.BiomeType.Forest:
                        if (randNr > forestRange && !CheckType(neighbour, (int)Tile.BiomeType.Desert, true))
                        {
                            neighbour.biomeType = Tile.BiomeType.Forest;
                            ChangeChances(neighbour, (int)Tile.BiomeType.Forest, growthForestChance, TypeSelection.Biome, false, 1);
                            ChangeChances(neighbour, (int)Tile.BiomeType.Desert, 0, TypeSelection.Biome, true, 1);
                        }
                        break;

                    case Tile.BiomeType.Swamp:
                        if (randNr > swampRange && neighbour.terrainType != Tile.TerrainType.Hills && !CheckType(neighbour, (int)Tile.BiomeType.Desert, true))
                        {
                            neighbour.biomeType = Tile.BiomeType.Swamp;
                            ChangeChances(neighbour, (int)Tile.BiomeType.Swamp, growthSwampChance, TypeSelection.Biome, false, 1);
                            ChangeChances(neighbour, (int)Tile.BiomeType.Desert, 0, TypeSelection.Biome, true, 1);
                        }
                        break;

                    case Tile.BiomeType.Desert:
                        if (randNr > desertRange && !CheckType(neighbour, (int)Tile.BiomeType.Forest, true) && !CheckType(neighbour, (int)Tile.BiomeType.Swamp, true))
                        {
                            neighbour.biomeType = Tile.BiomeType.Desert;
                            ChangeChances(neighbour, (int)Tile.BiomeType.Desert, growthDesertChance, TypeSelection.Biome, false, 1);
                            ChangeChances(neighbour, (int)Tile.BiomeType.Forest, 0, TypeSelection.Biome, true, 1);
                            ChangeChances(neighbour, (int)Tile.BiomeType.Swamp, 0, TypeSelection.Biome, true, 1);
                        }
                        break;

                    case Tile.BiomeType.Mountains:
                        Debug.LogError("This Tile should not be Growing!");
                        break;

                    case Tile.BiomeType.Water:
                        Debug.LogError("This Tile should not be Growing!");
                        break;
                }

                tileGrid[(int)neighbour.gridCoordinates.x, (int)neighbour.gridCoordinates.y] = neighbour;
            }
        }

        private void ChangeChances(Tile targetTile, int targetIndex, int value, TypeSelection type, bool setZero, int spreadRange)
        {
            Tile[] neighbours = GetNeighbours(targetTile, spreadRange);

            for (int i = 0; i < neighbours.Length; i++)
            {
                if (type == TypeSelection.Terrain)
                {
                    if (setZero)
                    {
                        neighbours[i].terrainChances[(Tile.TerrainType)targetIndex] = 0;
                    }
                    else
                    {
                        neighbours[i].terrainChances[(Tile.TerrainType)targetIndex] += value;
                    }
                }
                else if (type == TypeSelection.Biome)
                {
                    if (setZero)
                    {
                        neighbours[i].biomeChances[(Tile.BiomeType)targetIndex] = 0;
                    }
                    else
                    {
                        neighbours[i].biomeChances[(Tile.BiomeType)targetIndex] += value;
                    }
                }
                else
                {
                    if (setZero)
                    {
                        neighbours[i].nodeChances[(Tile.NodeType)targetIndex] = 0;
                    }
                    else
                    {
                        neighbours[i].nodeChances[(Tile.NodeType)targetIndex] += value;
                    }
                }

                tileGrid[(int)neighbours[i].gridCoordinates.x, (int)neighbours[i].gridCoordinates.y] = neighbours[i];
            }
        }

        private bool CheckType(Tile targetTile, int toCheckIndex, bool biome)
        {
            Tile[] neighbours = GetNeighbours(targetTile, 1);
            bool result = false;

            for (int i = 0; i < neighbours.Length; i++)
            {
                if (biome)
                {
                    if (neighbours[i].biomeType == (Tile.BiomeType)toCheckIndex)
                    {
                        result = true;
                    }
                }
                else
                {
                    if (neighbours[i].terrainType == (Tile.TerrainType)toCheckIndex)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        private Tile[] GetNeighbours(Tile tile, int range)
        {
            List<Tile> neighbours = new List<Tile>();
            List<Tile> newNeighbours = new List<Tile>();
            int cycles = range;

            if (tileGrid.GetLength(0) > (int)tile.gridCoordinates.x + 1)
            {
                neighbours.Add(tileGrid[(int)tile.gridCoordinates.x + 1, (int)tile.gridCoordinates.y]);
            }

            if ((int)tile.gridCoordinates.x - 1 >= 0)
            {
                neighbours.Add(tileGrid[(int)tile.gridCoordinates.x - 1, (int)tile.gridCoordinates.y]);
            }

            if (tileGrid.GetLength(1) > (int)tile.gridCoordinates.y + 1)
            {
                neighbours.Add(tileGrid[(int)tile.gridCoordinates.x, (int)tile.gridCoordinates.y + 1]);
            }

            if ((int)tile.gridCoordinates.y - 1 >= 0)
            {
                neighbours.Add(tileGrid[(int)tile.gridCoordinates.x, (int)tile.gridCoordinates.y - 1]);
            }

            cycles--;

            if (cycles <= 0)
            {
                return neighbours.ToArray();
            }
            else
            {
                for (int i = 0; i < neighbours.Count; i++)
                {
                    newNeighbours.AddRange(GetNeighbours(neighbours[i], cycles));
                }
                neighbours.AddRange(newNeighbours);
                return neighbours.ToArray();
            }
        }

        #endregion Map Generation

        #region Map Population

        private void PopulateMap()
        {
            for (int row = 0; row < gridHeight; row++)
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    int randNr = Random.Range(0, 100);
                    if (randNr > 100 - baseNodeChance)
                    {
                        PopulateTile(tileGrid[column, row]);
                    }
                }
            }
        }

        private void PopulateTile(Tile tile)
        {
            int randNr = Random.Range(0, 100);

            if (randNr > 100 - baseNodeChance)
            {
                if (tile.terrainType == Tile.TerrainType.Plains)
                {
                    switch (tile.biomeType)
                    {
                        case Tile.BiomeType.Grassland:

                            tile.node = CheckNodeChances(tile, Tile.NodeType.Castle, Tile.NodeType.Village, Tile.NodeType.City, Tile.NodeType.Outpost, Tile.NodeType.Trader);

                            break;

                        case Tile.BiomeType.Forest:

                            tile.node = CheckNodeChances(tile, Tile.NodeType.BanditCamp);

                            break;

                        case Tile.BiomeType.Swamp:

                            tile.node = CheckNodeChances(tile, Tile.NodeType.Coven);

                            break;

                        case Tile.BiomeType.Desert:

                            tile.node = CheckNodeChances(tile, Tile.NodeType.Castle, Tile.NodeType.Village, Tile.NodeType.City, Tile.NodeType.Outpost, Tile.NodeType.Trader);

                            break;

                        default:
                            break;
                    }
                }
                else if (tile.terrainType == Tile.TerrainType.Hills)
                {
                    switch (tile.biomeType)
                    {
                        case Tile.BiomeType.Grassland:

                            tile.node = CheckNodeChances(tile, Tile.NodeType.Castle, Tile.NodeType.Outpost, Tile.NodeType.BanditCamp);

                            break;

                        case Tile.BiomeType.Forest:

                            tile.node = CheckNodeChances(tile, Tile.NodeType.BanditCamp);

                            break;

                        case Tile.BiomeType.Desert:

                            tile.node = CheckNodeChances(tile, Tile.NodeType.Castle, Tile.NodeType.Outpost);

                            break;

                        default:
                            break;
                    }
                }

                SpreadOutNodes(tile);

                tileGrid[(int)tile.gridCoordinates.x, (int)tile.gridCoordinates.y] = tile;
            }
        }

        private void SpreadOutNodes(Tile tile)
        {
            switch (tile.node)
            {
                case Tile.NodeType.None:
                    return;

                case Tile.NodeType.Village:
                    ChangeChances(tile, (int)tile.node, 0, TypeSelection.Node, true, spreadFactorVillage);
                    break;

                case Tile.NodeType.City:
                    ChangeChances(tile, (int)tile.node, 0, TypeSelection.Node, true, spreadFactorCity);

                    break;

                case Tile.NodeType.Outpost:
                    ChangeChances(tile, (int)tile.node, 0, TypeSelection.Node, true, spreadFactorOutpost);

                    break;

                case Tile.NodeType.Castle:
                    ChangeChances(tile, (int)tile.node, 0, TypeSelection.Node, true, spreadFactorCastle);

                    break;

                case Tile.NodeType.Trader:
                    ChangeChances(tile, (int)tile.node, 0, TypeSelection.Node, true, spreadFactorTrader);

                    break;

                case Tile.NodeType.Coven:
                    ChangeChances(tile, (int)tile.node, 0, TypeSelection.Node, true, spreadFactorCoven);

                    break;

                case Tile.NodeType.BanditCamp:
                    ChangeChances(tile, (int)tile.node, 0, TypeSelection.Node, true, spreadFactorBanditCamp);

                    break;

                default:
                    return;
            }
        }

        private Tile.NodeType CheckNodeChances(Tile tile, params Tile.NodeType[] nodesToCheck)
        {
            Tile.NodeType result = Tile.NodeType.None;
            int randNr = Random.Range(0, 100);
            int[] nodeChances = new int[nodesToCheck.Length];

            for (int i = 0; i < nodesToCheck.Length; i++)
            {
                switch (nodesToCheck[i])
                {
                    case Tile.NodeType.Village:
                        nodeChances[i] = tile.nodeChances[Tile.NodeType.Village];
                        break;

                    case Tile.NodeType.City:
                        nodeChances[i] = tile.nodeChances[Tile.NodeType.City];
                        break;

                    case Tile.NodeType.Outpost:
                        nodeChances[i] = tile.nodeChances[Tile.NodeType.Outpost];
                        break;

                    case Tile.NodeType.Castle:
                        nodeChances[i] = tile.nodeChances[Tile.NodeType.Castle]; ;
                        break;

                    case Tile.NodeType.Trader:
                        nodeChances[i] = tile.nodeChances[Tile.NodeType.Trader]; ;
                        break;

                    case Tile.NodeType.Coven:
                        nodeChances[i] = tile.nodeChances[Tile.NodeType.Coven]; ;
                        break;

                    case Tile.NodeType.BanditCamp:
                        nodeChances[i] = tile.nodeChances[Tile.NodeType.BanditCamp]; ;
                        break;
                }
            }

            for (int i = 0; i < nodeChances.Length; i++)
            {
                if (i == 0)
                {
                    nodeChances[i] = 100 - nodeChances[i];
                }
                else
                {
                    nodeChances[i] = nodeChances[i - 1] - nodeChances[i];
                }
            }

            for (int i = 0; i < nodesToCheck.Length; i++)
            {
                if (randNr > nodeChances[i])
                {
                    result = nodesToCheck[i];
                    return result;
                }
            }

            return Tile.NodeType.None;
        }

        #endregion Map Population

        /// <summary>
        /// Instantiates tile visuals.
        /// </summary>
        /// <param name="tile">Tile to spawn visuals for.</param>
        private void SpawnVisuals(Tile tile)
        {
            Tile tileToSetup = tile;
            Vector3 tilePos = new Vector3(tile.gridCoordinates.x * tileSize + tile.gridCoordinates.x * tileOffset, 0, tile.gridCoordinates.y * tileSize + tile.gridCoordinates.y * tileOffset);

            tileToSetup.visualRootObject = new GameObject($"Tile {tile.gridCoordinates}");
            tileToSetup.visualRootObject.transform.position = tilePos;
            tileToSetup.visualRootObject.transform.SetParent(transform);

            if (tileToSetup.terrainType == Tile.TerrainType.None)
            {
                Debug.LogWarning("Some Tiles did not generate properly! Terrain Missing!");
                tileToSetup.terrainVisuals = GameObject.Instantiate<GameObject>(baseTileVisuals);
                tileToSetup.terrainVisuals.transform.position = tilePos;
                tileToSetup.terrainVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
            }
            else
            {
                switch (tileToSetup.terrainType)
                {
                    case Tile.TerrainType.None:
                        Debug.LogError("This Tile should have a Terrain by now!");
                        break;

                    case Tile.TerrainType.Plains:
                        tileToSetup.terrainVisuals = GameObject.Instantiate<GameObject>(plainsVisuals);
                        tileToSetup.terrainVisuals.transform.position = tilePos;
                        tileToSetup.terrainVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                        break;

                    case Tile.TerrainType.Hills:
                        tileToSetup.terrainVisuals = GameObject.Instantiate<GameObject>(hillVisuals);
                        tileToSetup.terrainVisuals.transform.position = tilePos;
                        tileToSetup.terrainVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                        break;

                    case Tile.TerrainType.Mountains:
                        tileToSetup.terrainVisuals = GameObject.Instantiate<GameObject>(mountainVisuals);
                        tileToSetup.terrainVisuals.transform.position = tilePos;
                        tileToSetup.terrainVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                        break;

                    case Tile.TerrainType.Water:
                        tileToSetup.terrainVisuals = GameObject.Instantiate<GameObject>(waterVisuals);
                        tileToSetup.terrainVisuals.transform.position = tilePos;
                        tileToSetup.terrainVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                        break;
                }
            }

            if (tileToSetup.biomeType == Tile.BiomeType.None)
            {
                Debug.LogWarning("Some Tiles did not generate properly! Biome Missing!");
            }
            else
            {
                switch (tileToSetup.biomeType)
                {
                    case Tile.BiomeType.None:
                        Debug.LogError("This Tile should have a Terrain by now!");
                        break;

                    case Tile.BiomeType.Forest:
                        tileToSetup.biomeVisuals = GameObject.Instantiate<GameObject>(forestVisuals);
                        tileToSetup.biomeVisuals.transform.position = new Vector3(tilePos.x, tilePos.y + (int)tileToSetup.terrainType + 1, tilePos.z);
                        tileToSetup.biomeVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                        break;

                    case Tile.BiomeType.Swamp:
                        tileToSetup.biomeVisuals = tileToSetup.terrainVisuals;
                        tileToSetup.biomeVisuals.GetComponent<MeshRenderer>().material.color = Color.black;
                        break;

                    case Tile.BiomeType.Desert:
                        tileToSetup.biomeVisuals = tileToSetup.terrainVisuals;
                        tileToSetup.biomeVisuals.GetComponent<MeshRenderer>().material.color = Color.yellow;
                        break;
                }
            }

            switch (tileToSetup.node)
            {
                case Tile.NodeType.None:
                    break;

                case Tile.NodeType.Village:
                    tileToSetup.NodeVisuals = GameObject.Instantiate<GameObject>(villageVisuals);
                    tileToSetup.NodeVisuals.transform.position = new Vector3(tilePos.x, tilePos.y + (int)tileToSetup.terrainType + 1, tilePos.z);
                    tileToSetup.NodeVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                    break;

                case Tile.NodeType.City:
                    tileToSetup.NodeVisuals = GameObject.Instantiate<GameObject>(cityVisuals);
                    tileToSetup.NodeVisuals.transform.position = new Vector3(tilePos.x, tilePos.y + (int)tileToSetup.terrainType + 1, tilePos.z);
                    tileToSetup.NodeVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                    break;

                case Tile.NodeType.Outpost:
                    tileToSetup.NodeVisuals = GameObject.Instantiate<GameObject>(outpostVisuals);
                    tileToSetup.NodeVisuals.transform.position = new Vector3(tilePos.x, tilePos.y + (int)tileToSetup.terrainType + 1, tilePos.z);
                    tileToSetup.NodeVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                    break;

                case Tile.NodeType.Castle:
                    tileToSetup.NodeVisuals = GameObject.Instantiate<GameObject>(castleVisuals);
                    tileToSetup.NodeVisuals.transform.position = new Vector3(tilePos.x, tilePos.y + (int)tileToSetup.terrainType + 1, tilePos.z);
                    tileToSetup.NodeVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                    break;

                case Tile.NodeType.Trader:
                    tileToSetup.NodeVisuals = GameObject.Instantiate<GameObject>(traderVisuals);
                    tileToSetup.NodeVisuals.transform.position = new Vector3(tilePos.x, tilePos.y + (int)tileToSetup.terrainType + 1, tilePos.z);
                    tileToSetup.NodeVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                    break;

                case Tile.NodeType.Coven:
                    tileToSetup.NodeVisuals = GameObject.Instantiate<GameObject>(covenVisuals);
                    tileToSetup.NodeVisuals.transform.position = new Vector3(tilePos.x, tilePos.y + (int)tileToSetup.terrainType + 1, tilePos.z);
                    tileToSetup.NodeVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                    break;

                case Tile.NodeType.BanditCamp:
                    tileToSetup.NodeVisuals = GameObject.Instantiate<GameObject>(banditCampVisuals);
                    tileToSetup.NodeVisuals.transform.position = new Vector3(tilePos.x, tilePos.y + (int)tileToSetup.terrainType + 1, tilePos.z);
                    tileToSetup.NodeVisuals.transform.SetParent(tileToSetup.visualRootObject.transform);
                    break;

                default:
                    break;
            }

            tileGrid[(int)tileToSetup.gridCoordinates.x, (int)tileToSetup.gridCoordinates.y] = tileToSetup;
        }

        /// <summary>
        /// Returns all tiles that can be traversed by an army.
        /// </summary>
        /// <returns>List of vector2 positions related to tile positions in the grid.</returns>
        public List<Vector2> GetWalkableTiles()
        {
            List<Vector2> possiblePathTiles = new List<Vector2>();

            for (int row = 0; row < gridHeight; row++)
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    if (tileGrid[column, row].terrainType != Tile.TerrainType.Mountains && tileGrid[column, row].terrainType != Tile.TerrainType.Water)
                    {
                        possiblePathTiles.Add(tileGrid[column, row].gridCoordinates);
                    }
                }
            }

            return possiblePathTiles;
        }
    }
}
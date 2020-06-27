using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.AI;

public class TilePlacerWFC : MonoBehaviour
{
    [Header("Набор тайлов для генерации")]
    [SerializeField] private List<Tile> TilePrefabs;
    [Header ("Настройки карты")]
    [SerializeField] public Vector2Int MapSize = new Vector2Int(20, 20);
    [SerializeField] public float TileSize = 10f;

    [Header("NavMeshSurface")]
    [SerializeField] public NavMeshSurface NavigationMesh = null;
    private Tile[,] spawnedTiles;
    [Header("Дополнительно")]
    [SerializeField] private List<Tile> TilesPreset;
    [SerializeField] private Transform TilePresetContainer;
    [SerializeField] private Transform TileContainer;

    private Queue<Vector2Int> recalcPossibleTilesQueue = new Queue<Vector2Int>();
    private List<Tile>[,] possibleTiles;

    private void Start()
    {
        //spawnedTiles = new Tile[MapSize.x, MapSize.y];

        //Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            //StopAllCoroutines();
            foreach (Tile spawnedTile in spawnedTiles)
            {
                if (spawnedTile != null) Destroy(spawnedTile.gameObject);
            }

            //StartCoroutine(Generate());
            NavigationMesh.RemoveData();
            Generate();
            
        }
    }

    private void BuildNavMesh()
    {
        if (NavigationMesh != null) NavigationMesh.RemoveData();
        NavigationMesh.BuildNavMesh();
    }

    public void Generate()
    {
        spawnedTiles = new Tile[MapSize.x, MapSize.y];

        possibleTiles = new List<Tile>[MapSize.x, MapSize.y];

        int maxAttempts = 10;
        int attempts = 0;

        while(attempts++ < maxAttempts)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    possibleTiles[x, y] = new List<Tile>(TilePrefabs);
                }
            }

            Tile tileInCenter = GetRandomTile(TilePrefabs);
            possibleTiles[MapSize.x / 2, MapSize.y / 2] = new List<Tile> { tileInCenter };

            EnqueueNeighboursRecalc(new Vector2Int(MapSize.x / 2, MapSize.y / 2));

            AddPresetTile();

            bool success = GenerateAllPossibleTiles();

            Debug.Log($"Количество попыток: {attempts}");
            if (success) break;
        }

        

        PlaceAllTiles();
        BuildNavMesh();
    }

    private void AddPresetTile()
    {
        TilePresetContainer.gameObject.SetActive(true);
        if (TilesPreset.Count > 0) TilesPreset.Clear();
 

        TilesPreset.AddRange(TilePresetContainer.GetComponentsInChildren<Tile>());

        ImplementPresetTile();

        TilePresetContainer.gameObject.SetActive(false);
        recalcPossibleTilesQueue.Clear();
    }

    private void ImplementPresetTile()
    {
        foreach (var item in TilesPreset)
        {
            Vector2Int tilePresetCoord = new Vector2Int((int)item.transform.position.x / (int)TileSize, (int)item.transform.position.z / (int)TileSize);
            possibleTiles[tilePresetCoord.x, tilePresetCoord.y] = new List<Tile> { item };
            EnqueueNeighboursRecalc(tilePresetCoord);
        }
    }

    private bool GenerateAllPossibleTiles()
    {
        int maxIterations = MapSize.x * MapSize.y;
        int iterations = 0;
        int backtracks = 0;
        while (iterations++ < maxIterations)
        {
            int maxInnterIterations = 500;
            int innetIterations = 0;
            while (recalcPossibleTilesQueue.Count > 0 && innetIterations++ < maxInnterIterations)
            {
                Vector2Int position = recalcPossibleTilesQueue.Dequeue();
                if (position.x == 0 || position.y == 0 || position.x == MapSize.x - 1 || position.y == MapSize.y - 1) continue;


                List<Tile> possibleTilesHere = possibleTiles[position.x, position.y];

                int countRemoved = possibleTilesHere.RemoveAll(t => !IsTilePossible(t, position));


                if (countRemoved > 0)
                {
                    EnqueueNeighboursRecalc(position);

                }

                if (possibleTilesHere.Count == 0)
                {
                    possibleTilesHere.AddRange(TilePrefabs);
                    possibleTiles[position.x + 1, position.y] = new List<Tile>(TilePrefabs);
                    possibleTiles[position.x - 1, position.y] = new List<Tile>(TilePrefabs);
                    possibleTiles[position.x, position.y + 1] = new List<Tile>(TilePrefabs);
                    possibleTiles[position.x, position.y - 1] = new List<Tile>(TilePrefabs);

                    EnqueueNeighboursRecalc(position);
                    ImplementPresetTile();
                    backtracks++;
                }
            }
            if (innetIterations == maxIterations) break;


            List<Tile> maxCountTile = possibleTiles[1, 1];
            Vector2Int maxCountTilePosition = new Vector2Int(1, 1);

            for (int x = 1; x < MapSize.x - 1; x++)
            {
                for (int y = 1; y < MapSize.y - 1; y++)
                {
                    if (possibleTiles[x, y].Count > maxCountTile.Count)
                    {
                        maxCountTile = possibleTiles[x, y];
                        maxCountTilePosition = new Vector2Int(x, y);
                    }
                }
            }

            if (maxCountTile.Count == 1)
            {
                Debug.Log($"Generated for {iterations} iterations, with {backtracks} backtracks");
                return true;
            }

            Tile tileToCollapse = GetRandomTile(maxCountTile);
            possibleTiles[maxCountTilePosition.x, maxCountTilePosition.y] = new List<Tile> { tileToCollapse };
            EnqueueNeighboursRecalc(maxCountTilePosition);
        }

        Debug.Log("Оборвали цикл");
        return false;
    }

    private bool IsTilePossible(Tile tile, Vector2Int position)
    {
        bool isAllForwardImpossible = possibleTiles[position.x, position.y + 1]
            .All(fwdTile => !CanAppendTile(tile, fwdTile, Vector3.forward));
        if (isAllForwardImpossible) return false;

        bool isAllBackImpossible = possibleTiles[position.x, position.y - 1]
            .All(backTile => !CanAppendTile(tile, backTile, Vector3.back));
        if (isAllBackImpossible) return false;

        bool isAllRightImpossible = possibleTiles[position.x - 1, position.y]
            .All(rightTile => !CanAppendTile(tile, rightTile, Vector3.right));
        if (isAllRightImpossible) return false;

        bool isAllLeftImpossible = possibleTiles[position.x + 1, position.y]
            .All(leftTile => !CanAppendTile(tile, leftTile, Vector3.left));
        if (isAllLeftImpossible) return false;

        return true;
    }

    private void PlaceAllTiles()
    {
        for (int x = 1; x < MapSize.x - 1; x++)
        {
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                //yield return new WaitForSeconds(0.2f);
                PlaceTile(x, y);
            }
        }
    }

    private void EnqueueNeighboursRecalc(Vector2Int position)
    {
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x+1, position.y));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x-1, position.y));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x, position.y+1));
        recalcPossibleTilesQueue.Enqueue(new Vector2Int(position.x, position.y-1));
    }

    private void PlaceTile(int x, int y)
    {
        List<Tile> availableTiles = possibleTiles[x, y];
        if (availableTiles.Count == 0) return;

        Tile selectedTile = GetRandomTile(availableTiles);
        spawnedTiles[x, y] = Instantiate(selectedTile, new Vector3(x, 0, y) * TileSize, selectedTile.transform.rotation, TileContainer);
    }

    private Tile GetRandomTile(List<Tile> availableTiles)
    {
        List<float> chances = new List<float>();
        for (int i = 0; i < availableTiles.Count; i++)
        {
            chances.Add(availableTiles[i].Weight);
        }

        float value = UnityEngine.Random.Range(0, chances.Sum());
        float sum = 0;

        for (int i = 0; i < chances.Count; i++)
        {
            sum += chances[i];
            if (value < sum)
            {
                return availableTiles[i];
            }
        }

        return availableTiles[availableTiles.Count - 1];
    }



    private bool CanAppendTile(Tile exsistingTile, Tile tileToAppend, Vector3 direction)
    {
        //Debug.Log("Зашли в CanAppendTile");
        if (exsistingTile == null)
        {
            return true;
        }

        if (direction == Vector3.left)
        {
            bool isAppend = exsistingTile.Rigth.tilestate == tileToAppend.Left.tilestate;
            return isAppend;
        }
        else if (direction == Vector3.right)
        {
            bool isAppend = exsistingTile.Left.tilestate == tileToAppend.Rigth.tilestate;
            return isAppend;
        }
        else if (direction == Vector3.forward)
        {
            bool isAppend = exsistingTile.Forward.tilestate == tileToAppend.Back.tilestate;
            return isAppend;
        }
        else if (direction == Vector3.back)
        {
            bool isAppend = exsistingTile.Back.tilestate == tileToAppend.Forward.tilestate;
            return isAppend;
        }
        else
        {
            throw new ArgumentException("Wrong direction value", nameof(direction));
        }


    }

}

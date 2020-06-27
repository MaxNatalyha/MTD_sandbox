using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TilePlacer : MonoBehaviour
{

    public List<Tile> TilePrefabs;
    public Vector2Int MapSize = new Vector2Int(20, 20);
    public Tile[,] spawnedTiles;
    public float TileSize = 10f;
    public List<Tile> availableTiles = new List<Tile>();


    void Start()
    {
        spawnedTiles = new Tile[MapSize.x, MapSize.y];

        //StartCoroutine (Generate());
        Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            StopAllCoroutines();

            foreach (Tile spawnedTile in spawnedTiles)
            {
                if (spawnedTile != null) Destroy(spawnedTile.gameObject);
            }

            //StartCoroutine(Generate());
            Generate();
        }
    }

    public void Generate()
    {
        for (int x = 1; x < MapSize.x -1; x++)
        {
            for (int y = 1; y < MapSize.y - 1; y++)
            {
                //yield return new WaitForSeconds(0.1f);

                //Debug.Log("Вызываем Плейсер");
                PlaceTile(x, y);
            }
        }
    }

    private void PlaceTile(int x, int y)
    {
        //Debug.Log("Зашли в PlaceTile");
        availableTiles.Clear();
        foreach (Tile tilePrefab in TilePrefabs)
        {
            if (CanAppendTile(spawnedTiles[x - 1, y], tilePrefab, Vector3.left) &&
                CanAppendTile(spawnedTiles[x + 1, y], tilePrefab, Vector3.right) &&
                CanAppendTile(spawnedTiles[x, y + 1], tilePrefab, Vector3.back) &&
                CanAppendTile(spawnedTiles[x, y - 1], tilePrefab, Vector3.forward))
            {
                //Debug.Log("Добавили в варианты тайла");
                availableTiles.Add(tilePrefab);
            }

        }

        if (availableTiles.Count == 0)
        {
            //Debug.Log("Список вероятных тайлов = 0");
            return;
        }

        //Tile selectedTile = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
        Tile selectedTile = GetRandomTile(availableTiles);
        spawnedTiles[x, y] = Instantiate(selectedTile, new Vector3(x, 0, y) * TileSize, Quaternion.identity);
        
        //Debug.Log("ЗАСПАВНИЛИ");
                   
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
            //Debug.Log("Существующий тайл: " + exsistingTile.name);
            //Debug.Log("Подбираемый тайл: " + tileToAppend.name);
            //Debug.Log("Существующий тайл спереди привязка: " + exsistingTile.Forward.tilestate);
            //Debug.Log("Подбираемый тайл сзади привязка: " + tileToAppend.Back.tilestate);
            //Debug.Log(isAppend);
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

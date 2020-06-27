using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacerFirstIteration : MonoBehaviour
{
    //[Header("Набор всех тайлов")]
    //[SerializeField] private List<Tile> TilesPrefab;


    private Vector2Int MapSize;

    private Tile[,] MapGrid;

    [Header("Ссылка на WFC")]
    [SerializeField] private TilePlacerWFC WFC = null;

    private float TileSize;

    [Header("Префабы тайлов")]
    [SerializeField] private GameObject Spawner = null;
    [SerializeField] private GameObject CastleRoadTile = null;
    [SerializeField] private GameObject Castle = null;
    [SerializeField] private GameObject Bridge = null;
    [SerializeField] private GameObject RoadT = null;
    [SerializeField] private GameObject BorderTile = null;
    [SerializeField] private GameObject RoadCornerLeft = null;
    [SerializeField] private GameObject RoadCornerRight = null;
    [SerializeField] private GameObject Road = null;
    [SerializeField] private GameObject RoadRotated = null;
    [SerializeField] private GameObject CastleBase = null;
    [SerializeField] private GameObject RoadQuad = null;

    [Header("Контейнеры")]
    [SerializeField] private Transform BasicTileContainer = null;
    [SerializeField] private Transform TileContainer = null;

    [Header("Дополнительо")]
    [SerializeField] private GameObject VisualtionGrid = null;
    [SerializeField] private List<GameObject> BasicGO = new List<GameObject>();
    [SerializeField] private bool IsWFCEnabled = false;
    [SerializeField] private bool IsSimpleVisualizationEnabled = false;
    [SerializeField] private bool IsSpawnAdditionalRoad = false;


    public void Start()
    {
        MapSize = WFC.MapSize;
        TileSize = WFC.TileSize;

        MapGrid = new Tile[MapSize.x, MapSize.y];

        GenerateBasis();
    }

    private void GenerateBasis()
    {

        // Простая визуализация карты

        if (IsSimpleVisualizationEnabled)
        {
            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    Instantiate(VisualtionGrid, new Vector3(x, 0, y) * TileSize, Quaternion.identity, TileContainer);
                }
            }
        }

        SpawnBasicTile();

        SpawnBorderTile(BorderTile);

        if(IsWFCEnabled)
            WFC.Generate();
    }

    private void SpawnBorderTile(GameObject borderTile)
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            Instantiate(borderTile, new Vector3(x, 0, MapSize.y - 1) * TileSize, Quaternion.identity);
            Instantiate(borderTile, new Vector3(x, 0,  0) * TileSize, Quaternion.identity);          
        }
        for (int y = 0; y < MapSize.y; y++)
        {
            Instantiate(borderTile, new Vector3(MapSize.x - 1, 0, y) * TileSize, Quaternion.identity);
            Instantiate(borderTile, new Vector3(0, 0, y) * TileSize, Quaternion.identity);
        }
    }

    private void SpawnBasicTile()
    {
        //Убрать дублирование кода, вынести в отдельные методы, координаты считать в отдельных переменных
        //Менять замку флаг на крыше

        float xPosFirstPlayer = (MapSize.x / 4);
        float xPosSecondPlayer = ((MapSize.x - (MapSize.x / 4)) - 1);

        BasicGO.Add(Instantiate(Spawner, new Vector3(MapSize.x / 2, 0, MapSize.y - 3) * TileSize, Spawner.transform.localRotation, BasicTileContainer));
        BasicGO.Add(Instantiate(RoadT, new Vector3(MapSize.x / 2, 0, MapSize.y / 2) * TileSize, RoadT.transform.localRotation, BasicTileContainer));
        //MakeRoadYpos(BasicGO[0], BasicGO[1]);

        BasicGO.Add(Instantiate(RoadCornerLeft, new Vector3(xPosFirstPlayer, 0, (MapSize.y / 2)) * TileSize, RoadCornerLeft.transform.localRotation, BasicTileContainer));
        BasicGO.Add(Instantiate(RoadCornerRight, new Vector3(xPosSecondPlayer, 0, (MapSize.y / 2)) * TileSize, RoadCornerRight.transform.localRotation, BasicTileContainer));

        BasicGO.Add(Instantiate(Bridge, new Vector3(xPosSecondPlayer, 0, (MapSize.y / 2) - 1) * TileSize, Bridge.transform.localRotation, BasicTileContainer));
        BasicGO.Add(Instantiate(Bridge, new Vector3(xPosFirstPlayer, 0, (MapSize.y / 2) - 1) * TileSize, Bridge.transform.localRotation, BasicTileContainer));
        //MakeRoadXpos(BasicGO[2], BasicGO[3]);

        SpawnCastle(xPosFirstPlayer, 1);
        SpawnCastle(xPosSecondPlayer, 2);

        //MakeRoadYpos(BasicGO[5], BasicGO[6]);
        //MakeRoadYpos(BasicGO[4], BasicGO[8]);
    }

    private void SpawnCastle(float CastleXpos, int playerNumer)
    {

        GameObject CastleBaseCont = Instantiate(CastleBase, new Vector3(CastleXpos, 0, 3) * TileSize, Quaternion.identity);
        List<GameObject> CastleBaseTiles = new List<GameObject>();

        for (int x = 0; x < 9; x++)
        {
            CastleBaseTiles.Add(CastleBaseCont.transform.GetChild(x).gameObject);
        }

        BasicGO.Add(CastleBaseTiles[0]);
        foreach (var item in CastleBaseTiles)
        {
            item.transform.SetParent(BasicTileContainer);
        }
        Destroy(CastleBaseCont);

        //Спавн префаба замка
        BasicGO.Add(Instantiate(Castle, new Vector3(CastleXpos, 0.2f, 3) * TileSize, Quaternion.identity, TileContainer));
    }

    private void MakeRoadXpos(GameObject Point1, GameObject Point2)
    {
        for (int x = 1; x < (Point2.transform.position.x - Point1.transform.position.x) / TileSize; x++)
        {

            float xPos = Point1.transform.position.x + (x * TileSize);
            float loopX = ((Point2.transform.position.x - Point1.transform.position.x) / TileSize) / 2;

            if (x == loopX)
                continue;

            Instantiate(RoadRotated, new Vector3(xPos, 0, Point1.transform.position.z), Quaternion.identity, BasicTileContainer);
        }
    }

    private void MakeRoadYpos(GameObject Point1, GameObject Point2)
    {
        for (int y = 1; y < (Point1.transform.position.z - Point2.transform.position.z) / TileSize; y++)
        {
            float yPos = Point2.transform.position.z + (y * TileSize);

            if (IsSpawnAdditionalRoad == true && y == UnityEngine.Random.RandomRange(2, (int)((Point1.transform.position.z - Point2.transform.position.z) / TileSize) - 2))
            {
                Instantiate(RoadQuad, new Vector3(Point1.transform.position.x, 0, yPos), RoadQuad.transform.localRotation, BasicTileContainer);
                continue;
            }


            Instantiate(Road, new Vector3(Point1.transform.position.x, 0, yPos), Road.transform.localRotation, BasicTileContainer);
        }
    }
}

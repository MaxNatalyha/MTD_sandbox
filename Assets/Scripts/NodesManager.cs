using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NodesManager : MonoBehaviour
{

    [SerializeField] public GameObject[,] Nodes;
    [SerializeField] public GameObject Node = null;
    // [SerializeField] GameObject HiNode;
    // [SerializeField] GameObject MidNode;
    [SerializeField] GameObject LowNode = null;
    // bool flag = false;
    // int checker = 0;
    [SerializeField] int middleNodeIndex;
    public int heigth = 10;
    public int length = 10;
    public NavMeshSurface surface;
    [SerializeField] GameObject CapsuleGO = null;
    NavMeshAgent CapsuleAgent; 
    

    [SerializeField] Vector2Int[,] Points;

    [SerializeField] List<GameObject> RiverNodeList = null;


    // Start is called before the first frame update
    void Start()
    {   
        //Вычисляю значение сдвига по двум осям для выравнивания массива по центру координат
        int offsetx = ((length*2) / 2) - 1;
        int offsety = ((heigth*2) / 2) - 1;
        
        // Инициализирую двумерные массивы
        Points = new Vector2Int[length, heigth];
        Nodes = new GameObject[length, heigth];

        

        // Заполняю двумерный массив точками
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < heigth; y++)
            {
                Points[x, y] = new Vector2Int (x*2 - offsetx, y*2 - offsety);
            }
        }

        //Создаю префабы по массиву точек
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < heigth; y++)
            {
                Nodes[x, y] = Instantiate(Node, new Vector3(Points[x, y].x, 0, Points[x, y].y), Node.transform.rotation);
            }
        }


        //Нахождение начальной точки реки в заданном диапазоне
        middleNodeIndex = ((heigth / 2) - 3) + Random.Range(0, 5);
        
        //Вычесление 3 отрезков реки в заисимости от ширины карты
        int [] ArrayOtrezok = new int[2];
        ArrayOtrezok[0] = length / 3;
        ArrayOtrezok[1] = (length / 3) + (length % 3);

        print("Слева и справа по " + ArrayOtrezok[0]);
        print("По центру " + ArrayOtrezok[1]);


        //Левая часть
        for (int x = 0; x < ArrayOtrezok[0]; x++)
        {
            RiverNodeList.Add(Nodes[x, middleNodeIndex]);
        }

        //Центральная часть
        for (int x = 0; x < ArrayOtrezok[1] + 2; x++)
        {
            RiverNodeList.Add(Nodes[(ArrayOtrezok[0] + x - 1), middleNodeIndex - 1]);
        }

        // //Правая часть
        for (int x = 0; x < ArrayOtrezok[0]; x++)
        {
            RiverNodeList.Add(Nodes[(ArrayOtrezok[0] + ArrayOtrezok[1] + x), middleNodeIndex - 2]);
        }



        // Визуализация реки
        for (int x = 0; x < RiverNodeList.Count; x++)
        {
            Renderer RiverRend = RiverNodeList[x].GetComponent<Renderer>();
            RiverRend.material.color = Color.blue;          
        }

        surface.BuildNavMesh();
        
        for (int x = 0; x < length/2; x++)
        {
            for (int y = 0; y < heigth/2; y++)
            {
                Nodes[x*2, y*2] = Instantiate(LowNode, new Vector3(Points[x*2, y*2].x, 1, Points[x*2, y*2].y), Node.transform.rotation);
            }
        }



        GameObject Capsule2 = Instantiate(CapsuleGO, Nodes[length-4, 1].transform.position + new Vector3(0, 5, 0), Nodes[length-4, 1].transform.rotation);
        CapsuleAgent = Capsule2.GetComponent<NavMeshAgent>();
        CapsuleAgent.SetDestination(Nodes[0, heigth-1].transform.position);




        // nodes = GameObject.FindGameObjectsWithTag("node");

        // foreach (GameObject node in nodes)
        // {
        //     if (checker == 0)
        //         {    
        //             Instantiate(HiNode, node.transform.position + new Vector3(0, 3, 0), node.transform.rotation);
        //         }
        //     if (checker == 1)
        //         {
        //             Instantiate(MidNode, node.transform.position + new Vector3(0, 2, 0), node.transform.rotation);
        //         }
        //     if (checker == 2)
        //         {
        //             Instantiate(LowNode, node.transform.position + new Vector3(0, 1, 0), node.transform.rotation);
        //         }

        //     checker = Random.Range(0, 3);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

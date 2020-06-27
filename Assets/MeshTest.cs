using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshTest : MonoBehaviour
{
    //public MeshFilter meshF;
    //public Mesh mesh;

    public Gradient gradientForward = new Gradient();
    public Gradient gradientBack = new Gradient();
    public Gradient gradientLeft = new Gradient();
    public Gradient gradientRight = new Gradient();

    public GradientColorKey[] colorKey;
    public GradientAlphaKey[] alphaKey;

    public Color[] ColorForward = new Color[3];
    public Color[] ColorBack = new Color[3];
    public Color[] ColorLeft = new Color[3];
    public Color[] ColorRight = new Color[3];




    //public Vector3[] vertices;
    //public List<Vector3Int> vertSort;
    //public List<Vector3> vertSort2;

    public GameObject Point;


    // Start is called before the first frame update
    void Start()
    {



        colorKey = new GradientColorKey[3];
        colorKey[0].color = ColorLeft[0];
        colorKey[0].time = 0.33f;

        colorKey[1].color = ColorLeft[1];
        colorKey[1].time = 0.67f;

        colorKey[2].color = ColorLeft[2];
        colorKey[2].time = 1f;

        alphaKey = new GradientAlphaKey[3];
        alphaKey[0].alpha = 1f;
        alphaKey[0].time = 0.33f;

        alphaKey[1].alpha = 1f;
        alphaKey[1].time = 0.67f;

        alphaKey[2].alpha = 1f;
        alphaKey[2].time = 1f;

        gradientLeft.mode = GradientMode.Fixed;
        gradientLeft.SetKeys(colorKey, alphaKey);


        //meshF = GetComponent<MeshFilter>();
        //mesh = meshF.mesh;
        //vertices = mesh.vertices;

        Debug.Log(gradientBack.Equals(gradientForward));


        //foreach (var item in vertSort)
        //{
        //    Instantiate(Point, new Vector3(item.x, item.y, item.z), Quaternion.identity);
        //}

    }

}

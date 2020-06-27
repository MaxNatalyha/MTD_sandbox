using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour
{
    private Snap[] snaps;
    public Snap Left;
    public Snap Rigth;
    public Snap Back;
    public Snap Forward;

    [Range(1, 100)] public int Weight = 50;

    void Awake()
    {
        snaps = GetComponentsInChildren<Snap>();

        for (int i = 0; i < 4; i++)
        {
            if (snaps[i].transform.position.x > transform.position.x)
            {
                Rigth = snaps[i];
                Rigth.name = "Rigth Snap";
            }

            if (snaps[i].transform.position.x < transform.position.x)
            {
                Left = snaps[i];
                Left.name = "Left Snap";
            }

            if (snaps[i].transform.position.z < transform.position.z)
            {
                Back = snaps[i];
                Back.name = "Back Snap";
            }

            if (snaps[i].transform.position.z > transform.position.z)
            {
                Forward = snaps[i];
                Forward.name = "Forward Snap";
            }
        }
    }

}

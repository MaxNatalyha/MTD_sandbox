using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState { river,
                        land,
                        hill,
                        land2hillForward,
                        land2hillBack,
                        road,
                        castle,
                        pond }

public class Snap : MonoBehaviour
{
    
    public TileState tilestate;

    void Start()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1.25f);
    }
}

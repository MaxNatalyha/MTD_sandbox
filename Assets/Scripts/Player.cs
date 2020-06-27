using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour
{
    public Sphere sphere;
    public float turnSpeed = 10f;
    public GameObject AnotherPlayer;
    public GameObject WalkSphere;
    public GameObject FlySphere;
    public GameObject[] Enemys;
    public string PlayerTag;
    public float moveProgress;
    Vector3 startPosition;
    public float moveSpeed = 2f;
    public bool flag = true;



    void Start()
    {
        //AnotherPlayer = GameObject.FindGameObjectWithTag(PlayerTag);
        Enemys = GameObject.FindGameObjectsWithTag("Player");
        DetectedTarget();
        //LookAtTarget();
        startPosition = transform.position;
    }

    void DetectedTarget()
    {
        foreach (GameObject enemy in Enemys)
        {
            sphere = enemy.GetComponent<Sphere>();

            if(sphere.state == State.fly)
            {
                FlySphere = enemy;
            } else if (sphere.state == State.walk)
            {
                WalkSphere = enemy;
            }

        }
        
        AnotherPlayer = WalkSphere;
    }

    void Update()
    {   
        if(AnotherPlayer == null)
            return;

        
        if (flag)
            {
                AnotherPlayer = WalkSphere;
                LookAtTarget();
            } else
            {
                AnotherPlayer = FlySphere;
                LookAtTarget();
            }

        //LookAtTarget();
        CycleMove();
    }

    private void CycleMove()
    {
        moveProgress = Mathf.PingPong(Time.time * moveSpeed, 1);
        transform.position = startPosition + (new Vector3(5, 0, 0) * moveProgress);
    }

    void LookAtTarget()
    {
        Vector3 dir = AnotherPlayer.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
    }
}

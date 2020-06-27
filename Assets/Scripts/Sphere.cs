using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum State {walk, fly}

public class Sphere : MonoBehaviour
{

    [SerializeField] public State state;

    void Start()
    {
        
    }

    public void DoSomething(string nameState)
    {
        print (nameState);
    }
    void Update()
    {
        
    }
}

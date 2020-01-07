using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    [SerializeField] private int cost, size, lifeSpan;
    float startTime;
    MeshRenderer mr;
    
    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        transform.localScale *= size;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {


        if (Time.time > startTime+lifeSpan)
        {
          KillFood();
        }



    }

    public void Eaten()
    {
        size--;
        if (size == 0)
        {
            KillFood();
        }
    }

    void KillFood()
    {
        Debug.Log("Kill ME");
        Destroy(this.gameObject);
    }

    
}

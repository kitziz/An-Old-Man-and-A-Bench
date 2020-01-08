using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    [SerializeField] private int cost, size, lifeSpan;
    float startTime;
    MeshRenderer mr;
    private Vector3 originalScale;
    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        originalScale = transform.localScale;
        transform.localScale = originalScale * size;
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
        else if (size > 0) transform.localScale = originalScale * size;

       
    }

    void KillFood()
    {
        Debug.Log("Kill ME");
        Destroy(this.gameObject);
    }

    
}

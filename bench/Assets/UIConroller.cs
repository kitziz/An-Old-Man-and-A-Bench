using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConroller : MonoBehaviour
{
    [SerializeField] int myScore;
        // Start is called before the first frame update
    void Start()
    {
        myScore = GameManager.score; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

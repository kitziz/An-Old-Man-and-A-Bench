using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAmbient : MonoBehaviour
{

    [SerializeField] AudioClip ambiant;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.PlaySingle(ambiant);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

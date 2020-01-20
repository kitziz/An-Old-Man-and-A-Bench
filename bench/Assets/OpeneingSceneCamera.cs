using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeneingSceneCamera : MonoBehaviour
{


    Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MainMenu() {

        Debug.Log("Main Menu");
        anim.speed = 0;
    }

    void ContinueClip() {
    }

    public void ExitMenu() {
        anim.speed = 1  ;
    }


    void StartGame() {
        anim.speed = 0;
        Destroy(gameObject);
    }
}

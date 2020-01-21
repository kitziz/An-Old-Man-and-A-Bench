using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Coin : MonoBehaviour
{

    public Animator anim;
    MeshRenderer myMR;
    public Material[] coinMaterial;
    public bool fade = false;
    public bool destroy = false;

    // Start is called before the first frame update
    void Start()
    {
        int ind=0;

        myMR = GetComponentInChildren<MeshRenderer>();

        for (int i = 0; i < coinMaterial.Length; i++)
        {
            if (gameObject.tag.Contains(i.ToString()))
            {
                ind = i;
            }
        }
        myMR.material = coinMaterial[ind];
        Debug.Log("============init Coin MR: "+ind+" "+ myMR.material + this.tag);
    }

    void Update()
    {
                if (destroy == true) Destroy(this.gameObject);
    }

    void FadeOut()
    {
        myMR.material.DOFade(0, 1f);
    }
}

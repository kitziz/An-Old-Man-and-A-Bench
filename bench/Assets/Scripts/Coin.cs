using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    MeshRenderer myMR;
    public Material[] coinMaterial;

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

    }


    /* logic replaced
    public void SetCoinType(int color = 0)
    {
        Debug.Log("============set Coin MR " + myMR + " " + this.name + " " + color + " " +  coinMaterial[color]);
        coinColor = color;
        updateColor = true;
    }

    public int GetCoinType()
    {
        return coinColor;
    }
    */

}

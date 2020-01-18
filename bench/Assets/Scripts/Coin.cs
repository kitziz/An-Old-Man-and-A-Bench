using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    MeshRenderer myMR;
    public Material[] coinMaterial;
    public int coinValue = 1;

    // Start is called before the first frame update
    void Start()
    {
        myMR = GetComponentInChildren<MeshRenderer>();
    }

    public void SetCoin(int Color = 0, int value = 1)
    {
        //myMR.materials[0] = coinMaterial[Color];
        coinValue = value;
    }
}

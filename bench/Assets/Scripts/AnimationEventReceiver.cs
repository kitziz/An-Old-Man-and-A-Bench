using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public Pigeon Pigeon;

    public void Bite()
    {
        Pigeon.myFood.Eaten();
        Pigeon.AddEnergy();
        Debug.Log("Bite!Bite!Bite!");
    }

    public void takeoff()
    {
        Pigeon.FlyOut();
    }
}


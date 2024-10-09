using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class ShakeCamera : MonoBehaviour
{

    public Shaker MyShaker;
    public ShakePreset MyShakePreset;

    // Start is called before the first frame update
    void Start()
    {
        MyShaker.Shake(MyShakePreset);
        Shaker.ShakeAll(MyShakePreset);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //플레이어 정보

    public int X, Y, Z, Jump;

    void Start()
    {
        X = 0; 
        Y = 0;
        Jump = 2;
    }
}
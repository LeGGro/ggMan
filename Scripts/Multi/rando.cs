using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rando : MonoBehaviour
{
    public static int[] mass = new int[400];
    public void Awake()
    {
        for (int i = 0; i < 400; i++)
        {
            mass[i] = Random.Range(1, 8);

        }
    }
}

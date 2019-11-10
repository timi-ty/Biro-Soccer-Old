using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenBehaviour : MonoBehaviour
{
    [Range(1.0f, 5.0f)]
    public float impactPower;
    public float weight;
    public float maxSpeed;
    [Range(1.0f, 5.0f)]
    public float powerGasTank;
    public float recoilPower;
    public PenComponentBehaviour cover, middle, tail;
    [HideInInspector]
    public PenComponentBehaviour rightComponent, leftComponent;
    //Physics breaks down when maxSpeed exceeds 100
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Crosshairs", order = 1)]
public class CrosshairScriptableObj : ScriptableObject
{

    
    public float width;
    public float thickness;
    public float gap;
    public float length;
    public bool isStatic;

}

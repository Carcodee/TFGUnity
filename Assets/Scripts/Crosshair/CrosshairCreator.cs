using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairCreator : MonoBehaviour
{
    CrosshairScriptableObj crosshairScriptableObj;
    public CrossHair crossHair;
    void Start()
    {
        crossHair = new CrossHair(crosshairScriptableObj.width,  crosshairScriptableObj.thickness, crosshairScriptableObj.gap, crosshairScriptableObj.length,crosshairScriptableObj.isStatic);
    }

    void Update()
    {
        
    }
}

public class CrossHair
{
    public float width;
    public float thickness;
    public float gap;
    public float length;
    public bool isStatic;
    public float gapBuffer;
    public CrossHair(float width, float thickness, float gap, float length, bool isStatic)
    {
        
        this.width = width;
        this.thickness = thickness;
        this.gap = gap;
        this.length = length;
        this.isStatic = isStatic;
        gapBuffer = gap;
    }
    public void SetWidth(float width)
    {
        this.width = width;
    }
    public void SetThickness(float thickness)
    {
        this.thickness = thickness;
    }
    public void SetGap(float gap)
    {
        this.gap = gap;
    }
    public void SetLength(float length)
    {
        this.length = length;
    }

    public void SetRecoilGap(float currentGapPrecision)
    {
        if (!isStatic)
        {
            gap =  gapBuffer * currentGapPrecision ;
        }
        
    }

}
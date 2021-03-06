using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightMesh : MonoBehaviour
{
    public bool isHighlighted = false;
    public Color highLightColor = Color.white;
    private bool donehighighting = false;
    private MeshRenderer meshrend;
    public Color baseColor;

    // Update is called once per frame sadas 
    void Update()
    {
        if (isHighlighted)
        {
            meshrend.material.SetColor("_Color", highLightColor);
            donehighighting = true;
        }
        else if (!isHighlighted && donehighighting)
        {
            //return to normal
            meshrend.material.SetColor("_Color", baseColor);
            donehighighting = false;
        }
    }

    public void Init(Color color, MeshRenderer meshRenderer)
    {
        meshrend = meshRenderer;
        baseColor = Color.gray;
        meshrend.material.SetColor("_Color", Color.gray);
    }
}

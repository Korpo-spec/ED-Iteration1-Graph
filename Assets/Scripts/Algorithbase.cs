using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Algorithbase : MonoBehaviour
{
    public string Name;
    public bool pathFound;
    public INode endNode;
    protected int startnode;
    protected int endnode;

    public virtual void OnStart(List<Ball> balls){}

    public virtual void AssembleGraph(List<Ball> balls)
    {
        
    }
    public abstract void StartAlgorithm( int startIndex, int endIndex);

    public virtual void DrawPath(LineRenderer lineRenderer)
    {
        if (!pathFound)
        {
            lineRenderer.positionCount = 0;
            return;
        }
    }
}

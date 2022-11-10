using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INode
{
    public float fCost => hCost + gCost;
    
    [HideInInspector]public float hCost { get; set; }
    [HideInInspector]public float gCost { get; set; }
    
    public INode parent { get; set; }

    public abstract List<Ball> Nieghbours { get; }
    
    public Vector3 position { get; }
}

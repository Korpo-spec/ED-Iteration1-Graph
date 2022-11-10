using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Algorithbase : MonoBehaviour
{
    public bool pathFound;
    public INode endNode;

    public virtual void AssembleGraph(List<Ball> balls)
    {
        
    }
    public abstract void StartAlgorithm( int startIndex, int endIndex);
}

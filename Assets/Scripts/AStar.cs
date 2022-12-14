using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class AStar : Algorithbase
{
    public override void AssembleGraph(List<Ball> balls)
    {
        nodes.Clear();
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetNearbyColliders();
            nodes.Add(balls[i]);
            nodes[i].hCost = 0;
            nodes[i].gCost = 0;
            nodes[i].parent = null;
        }
    }

    private List<INode> nodes = new List<INode>();
    
    HashSet<INode> openList = new HashSet<INode>();
    HashSet<INode> closedList = new HashSet<INode>();
    
    public override void StartAlgorithm( int startIndex, int endIndex)
    {
        pathFound = false;
        startnode = startIndex;
        openList.Clear();
        closedList.Clear();
        
        openList.Add(nodes[startIndex]);
        INode currentNode;
        while (openList.Count > 0)
        {
            Profiler.BeginSample("Sort", this);
            currentNode = openList.OrderBy(node => node.fCost).First();
            Profiler.EndSample();
            if (currentNode == nodes[endIndex])
            {
                pathFound = true;
                endNode = currentNode;
                break;
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            for (int i = 0; i < currentNode.Nieghbours.Count; i++)
            {
                INode node = currentNode.Nieghbours[i];
                if (closedList.Contains(node)) continue;

                if (!openList.Contains(node))
                {
                    openList.Add(node);
                    node.parent = currentNode;
                    node.gCost = currentNode.gCost + Vector3.Distance(currentNode.position, node.position);
                    node.hCost = Vector3.Distance(nodes[endIndex].position, node.position);
                }
                else
                {
                    if (node.gCost > currentNode.gCost +1)
                    {
                        node.parent = currentNode;
                        node.gCost = currentNode.gCost + 1;
                    }
                }
            }
        }

    }

    private List<Vector3> linepos = new List<Vector3>();
    public override void DrawPath(LineRenderer lineRenderer)
    {
        base.DrawPath(lineRenderer);
        if (pathFound)
        {
            
            linepos.Clear();
            
            DrawPath(endNode, nodes[startnode]);
            
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.positionCount = linepos.Count;
            lineRenderer.useWorldSpace = true;

            lineRenderer.SetPositions( linepos.ToArray());
        }
    }
    
    private void DrawPath(INode currentNode , INode startNode)
    {
        linepos.Add(currentNode.position);
        

        if (currentNode == startNode)
        {
            return;
        }
        DrawPath(currentNode.parent, startNode);
    }
}

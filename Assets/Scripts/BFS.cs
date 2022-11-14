using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : Algorithbase
{
    public override void AssembleGraph(List<Ball> balls)
    {
        
        Graph.Clear();
        
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetNearbyColliders();
            Graph.Add(balls[i]);
        }
        
    }

    private List<INode> Graph = new List<INode>();

    private Queue<INode> nodes = new Queue<INode>();
    private HashSet<INode> visited = new HashSet<INode>();
    public override void StartAlgorithm(int startIndex, int endIndex)
    {
        endNode = Graph[endIndex];
        startnode = startIndex;
        
        visited.Clear();
        nodes.Clear();
        pathFound = false;
        
        nodes.Enqueue(Graph[startIndex]);
        
        visited.Add(Graph[startIndex]);

        while (nodes.Count > 0)
        {
            INode node = nodes.Dequeue();

            for (int i = 0; i < node.Nieghbours.Count; i++)
            {
                if (!visited.Contains(node.Nieghbours[i]))
                {
                    node.Nieghbours[i].parent = node;
                    if (node.Nieghbours[i] == endNode)
                    {
                        
                        pathFound = true;
                        endNode = node.Nieghbours[i];
                        return;
                    }
                    nodes.Enqueue(node.Nieghbours[i]);
                    visited.Add(node.Nieghbours[i]);
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
            
            DrawPath(endNode, Graph[startnode]);
            
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

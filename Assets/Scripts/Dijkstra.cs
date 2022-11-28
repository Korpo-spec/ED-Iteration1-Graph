using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Dijkstra : Algorithbase
{
    

    public override void OnStart(List<Ball> balls)
    {
        this.balls = balls;
        graph = new List<List<(int Row, float Value)>>();

        for (int i = 0; i < balls.Count; i++)
        {
            graph.Add(new List<(int Row, float Value)>());
        }
    }

    private List<Ball> balls;
    private List<float> dist = new List<float>();
    public Dictionary<int, int> prevVertex = new Dictionary<int, int>();
    
    private HashSet<int> visited = new HashSet<int>();
    private HashSet<int> unvisited = new HashSet<int>();
    private HashSet<int> openNodes = new HashSet<int>();
    private List<List<(int Row, float Value)>> graph;
    
    public override void AssembleGraph(List<Ball> balls)
    {
        
        
        for (int i = 0; i < balls.Count; i++)
        {
            Ball b = balls[i];
            b.GetNearbyColliders();
            List<(int Rows, float Value)> row = graph[i];
            row.Clear();
            for (int j = 0; j < b.ballInRange.Count; j++)
            {
                float f = Vector3.Distance(b.transform.position, b.ballInRange[j].transform.position);
                f = f < 0.1f ? 0.1f : f;
                row.Add((b.ballInRange[j].ID, f ));
            }
            
        }
        
        
        
    }
    public override void StartAlgorithm(int startnode, int endNode)
    {
        this.startnode = startnode;
        this.endnode = endNode;
        visited.Clear();
        unvisited.Clear();
        openNodes.Clear();
        prevVertex.Clear();
        openNodes.Add(startnode);
        pathFound = false;
        //A priority queue could be used but it seems to work just fine like this
        for (int i = 0; i < graph.Count; i++)
        {
            unvisited.Add(i);
        }
        dist.Clear();
        prevVertex.Clear();
        for (int i = 0; i < graph.Count; i++)
        {
            dist.Add(float.PositiveInfinity);
        }
        dist[startnode] = 0;
        
        int currentNode = startnode;

        int amount = 0;

        while (openNodes.Count > 0)
        {

            if (currentNode == endNode)
            {
                pathFound = true;
                Debug.Log("found");
            }
            
            //Check surrounding
            for (int i = 0; i < graph[currentNode].Count; i++)
            {
                var value = graph[currentNode];
                
                if(visited.Contains(value[i].Row)) continue;
                openNodes.Add(value[i].Row);
                
                    var newValue = dist[currentNode] + value[i].Value;
                    if (dist[value[i].Row] > newValue)
                    {
                        dist[value[i].Row] = newValue;
                        
                        if (!prevVertex.TryGetValue(value[i].Row, out _))
                        {
                            
                            prevVertex.Add(value[i].Row, currentNode);
                        }
                        else
                        {
                            prevVertex[value[i].Row] = currentNode;
                        }
                        
                    }
                
            }

            unvisited.Remove(currentNode);
            openNodes.Remove(currentNode);
            visited.Add(currentNode);
            float smallestValue = 10000000;
            //picks out the smalllest value node
            foreach (var node in unvisited)
            {
                if (smallestValue > dist[node])
                {
                    smallestValue = dist[node];
                    currentNode = node;
                }
                
            }

            
            
        }
        
        

        
    }

    private List<Vector3> linepos = new List<Vector3>();

    public override void DrawPath(LineRenderer lineRenderer)
    {
        base.DrawPath(lineRenderer);
        if (!pathFound)
        {
            return;
        }
        
        
        lineRenderer.positionCount = 0;
        linepos.Clear();
        DrawToCheckPoint(prevVertex, endnode, startnode);
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.positionCount = linepos.Count;
        lineRenderer.useWorldSpace = true;

        lineRenderer.SetPositions( linepos.ToArray()); //x,y and z position of the starting point of the line
            
        
        
        
            
        
    }

    private void DrawToCheckPoint(Dictionary<int,int> prevVertex, int checkpoint, int startNode)
    {
        if (startNode == checkpoint)
        {
            return;
        }
        
        linepos.Add(balls[checkpoint].transform.position);
        linepos.Add(balls[prevVertex[checkpoint]].transform.position);
        checkpoint = prevVertex[checkpoint];
        DrawToCheckPoint(prevVertex, checkpoint, startNode);
        
    }
}

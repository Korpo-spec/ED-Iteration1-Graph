using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Dijkstra : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    private List<float> dist = new List<float>();
    public Dictionary<int, int> prevVertex = new Dictionary<int, int>();
    public bool foundEndPoint = false;
    private HashSet<int> visited = new HashSet<int>();
    private HashSet<int> unvisited = new HashSet<int>();
    private HashSet<int> openNodes = new HashSet<int>();
    public void StartDijkstra(List<List<(int Row, float Value)>> graph, int startnode, int endNode)
    {
        Profiler.BeginSample("Dijkstra", this);
        visited.Clear();
        unvisited.Clear();
        openNodes.Clear();
        prevVertex.Clear();
        openNodes.Add(startnode);
        foundEndPoint = false;
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
                foundEndPoint = true;
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
        
        Profiler.EndSample();

        
    }
}

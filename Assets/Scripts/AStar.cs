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
        }
    }

    private List<INode> nodes = new List<INode>();
    
    
    
    public override void StartAlgorithm( int startIndex, int endIndex)
    {
        pathFound = false;
        HashSet<INode> openList = new HashSet<INode>();
        HashSet<INode> closedList = new HashSet<INode>();
        
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
                    node.gCost = currentNode.gCost + 1;
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
}

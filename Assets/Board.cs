using System;
using BoardGame;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[ExecuteAlways]

public class Board : BoardParent
{
    // This function is called whenever the board or any tile inside the board
    // is modified.
     
    

    [SerializeField] private int steps;

    private List<Tile> tilelist;
    [SerializeField] private bool doAlgorithm;

    private int startID = 0;
    private List<int> checkPoints;
    public override void SetupBoard()
    {
        if (!doAlgorithm)
        {
            return;
        }
        // 1. Get the size of the board

        //vectors = GetComponent<VectorRenderer>();
        checkPoints = new List<int>();
        var boardSize = BoardSize;
        
        List<List<(int Column, int Value)>> columms = new List<List<(int Column, int Value)>>();
        // 2. Iterate over all tiles
        int amountBlocked = 0;
        tilelist = new List<Tile>();
        foreach (var tile in Tiles)
        {
            tilelist.Add(tile);
        }
        
        
        
        List<Tile> orderedTileList = new List<Tile>();
        for (int i = 0; i < boardSize.y; i++)
        {
            orderedTileList.AddRange(Tiles.Where(s => s.coordinate.y == i).OrderBy(r => r.coordinate.x).ToList());
        }
        
        Vector2Int[] surroundingTiles = new Vector2Int[4];
        surroundingTiles[0] = Vector2Int.up;
        surroundingTiles[1] = Vector2Int.right;
        surroundingTiles[2] = Vector2Int.down;
        surroundingTiles[3] = Vector2Int.left;
        
        List<int> blockedPositions = new List<int>();
        
        foreach (var tile in orderedTileList)
        {
            if (tile.IsBlocked)
            {
                amountBlocked++;
                blockedPositions.Add(amountBlocked);
                continue;
            }
            blockedPositions.Add(amountBlocked);
        }
        
        
        amountBlocked = 0;
        foreach (Tile tile in orderedTileList)
        {
            
            
            if (tile.IsBlocked)
            {
                
                //Debug.Log( tile.coordinate.x + (tile.coordinate.y * boardSize.y)+ " BLOCKED");
                
                amountBlocked++;
                
                continue;
            }
            
            

            if (tile.IsStartPoint)
            {
                startID = tile.coordinate.x + (tile.coordinate.y * boardSize.x)- amountBlocked;
            }

            if (tile.IsCheckPoint)
            {
                checkPoints.Add(tile.coordinate.x + (tile.coordinate.y * boardSize.x)- amountBlocked);
            }
            Vector2Int graphCoords = new Vector2Int();
            List<(int Rows, int Value)> row = new List<(int Column, int Value)>();
            if (tile.IsPortal(out Vector2Int portalcoords))
            {
                graphCoords.x = portalcoords.x + (portalcoords.y * boardSize.x);
                graphCoords.y = portalcoords.x + (portalcoords.y * boardSize.x)- blockedPositions[graphCoords.x];
                row.Add((graphCoords.y, 1));
            }
            
            
            tile.id = tile.coordinate.x + (tile.coordinate.y * boardSize.x) - (amountBlocked);
            
            for (int i = 0; i < surroundingTiles.Length; i++)
            {
                if (TryGetTile(tile.coordinate + surroundingTiles[i], out Tile neighbor))
                {
                    var coords = neighbor.coordinate;
                    graphCoords = new Vector2Int();


                    
                    
                    
                    if(neighbor.IsBlocked) continue;
                    
                    
                    
                    graphCoords.x = coords.x + (coords.y * boardSize.x);
                    //Debug.Log(amountBlocked);
                    
                    graphCoords.y = coords.x + (coords.y * boardSize.x) - (blockedPositions[graphCoords.x]);
                    
                    
                    
                    
                    if (neighbor.IsObstacle(out int penalty))
                    {
                        row.Add((graphCoords.y, penalty));
                    }
                    else
                    {
                        row.Add((graphCoords.y, 1));
                    }
                    /*
                    if (neighbor.IsPortal(out coords))
                    {
                        graphCoords.x = tile.coordinate.x + (tile.coordinate.y * boardSize.y);
                        graphCoords.y = coords.x + (coords.y * boardSize.y);
                        graph[graphCoords.x, graphCoords.y] = 1;
                    }
                    */
                        
                    
                    
                    
                }
                else
                {
                    
                    
                }
                
            }
            columms.Add(row);
            
        }

        
        //graph = removeBlockedRows(graph, blockedPositions);
        Dijkstra(columms, startID);
        
        // 3. Find a tile with a particular coordinate
        
    }

    private float[,] removeBlockedRows(float[,] graph, List<int> removePositions)
    {
        int size = graph.GetLength(0) - removePositions.Count;
        var result = new float[size, size];
        int indexC = 0;
        int indexR = 0;
        for (int i = 0; i < graph.GetLength(0); i++)
        {
            
            if (removePositions.Contains(i))
            {
                continue;
            }
            
            for (int j = 0; j < graph.GetLength(0); j++)
            {

                if (removePositions.Contains(j))
                {
                    
                    continue;
                }
                
                var v = graph[j, i];
                
                result[indexC, indexR] = v;
                indexC++;
                
            }
            
            indexC = 0;
            indexR++;

            
        }
        return result;
    }

    private List<float> dist;
    private Dictionary<int, int> prevVertex;
    private void Dijkstra(List<List<(int Row, int Value)>> graph, int startnode)
    {
        
        HashSet<int> visited = new HashSet<int>();
        HashSet<int> unvisited = new HashSet<int>();//A priority queue could be used but it seems to work just fine like this
        for (int i = 0; i < graph.Count; i++)
        {
            unvisited.Add(i);
        }
        dist = new List<float>();
        prevVertex = new Dictionary<int, int>();
        for (int i = 0; i < graph.Count; i++)
        {
            dist.Add(float.PositiveInfinity);
        }
        dist[startnode] = 0;
        
        int currentNode = startnode;
        
        

        while (unvisited.Count > 0)
        {
            
            
            
            //Check surrounding
            for (int i = 0; i < graph[currentNode].Count; i++)
            {
                var value = graph[currentNode];
                if(visited.Contains(value[i].Row)) continue;
                
                    var newValue = dist[currentNode] + value[i].Value;
                    if (dist[value[i].Row] > newValue)
                    {
                        dist[value[i].Row] = newValue;
                        if (prevVertex.TryGetValue(i, out _))
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

    private void DrawToCheckPoint(Dictionary<int,int> prevVertex, int checkpoint, int startNode, List<Tile> tiles)
    {
        if (startNode == checkpoint)
        {
            return;
        }
        
        checkpoint = prevVertex[checkpoint];
        DrawToCheckPoint(prevVertex, checkpoint, startNode , tiles);
        
    }
    void Update()
    {

        foreach (var tile in Tiles) {
            tile.OnUpdate((Board) this);
        }

        if (!doAlgorithm)
        {
            return;
        }
        List<Tile> noBlockedlist = tilelist.Where(p => !p.IsBlocked).OrderBy(p => p.id).ToList();
        //using (vectors.Begin())
        //{
            foreach (var checkPoint in checkPoints)
            {
                if (dist[checkPoint] <= steps)
                {
                    DrawToCheckPoint(prevVertex, checkPoint, startID, noBlockedlist);
                }
                
            }
        //}
        for (int i = 0; i < noBlockedlist.Count; i++)
        {
            if (dist[i]<= steps)
            {
                //vectors.Draw(noBlockedlist[i].transform.position, noBlockedlist[i].transform.position + new Vector3(0,0.5f,0), Color.blue);
                noBlockedlist[i].GetComponent<MeshRenderer>().material = noBlockedlist[i].walkable;

            }
        }
    }
}

readonly struct Key : IComparable<Key> {
    public readonly int id;
 
    public readonly int Weight;

    public Key(int ID, int weight) {
        id = ID;
        Weight = weight;
    }

    public int CompareTo(Key other) {
        if (id == other.id) return 0; // Make sure keys are unique (two keys with same weight can exist)
        return Weight - other.Weight; // Put smallest weight first in the list
    }
}

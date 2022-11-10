using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

public class BallManager : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Vector2Int amountToCreate;
    

    [SerializeField] private float spacing;

    [SerializeField] private Dijkstra dijkstra;
    [SerializeField] private Algorithbase algorithm;
    [SerializeField] private SimulationBoundaries boundaries;

    private int startNode;
    private int endNode;
    private readonly List<Ball> balls = new List<Ball>();
    private CustomSampler sampler;
    // Start is called before the first frame update
    void Start()
    {
        
        SpawnBalls(amountToCreate);

        
        sampler = CustomSampler.Create("Dijkstra");
    }

    public void SpawnBalls(Vector2Int vec)
    {
        
        float startposX = (spacing * vec.x) /2 - spacing/2;
        float startposY = (spacing * vec.y) / 2 - spacing/2;
        int currentID = 0;
        for (int i = 0; i < vec.y; i++)
        {
            for (int j = 0; j < vec.x; j++)
            {
                var g =Instantiate(ball, new Vector3(-startposX +j * spacing, -startposY + i * spacing, 0), quaternion.identity);
                Ball ballScript = g.GetComponent<Ball>();
                ballScript.ID = currentID;
                g.name = currentID.ToString();
                currentID++;
                
                balls.Add(ballScript);
            }
        }

        boundaries.boundaries = new Vector2(Mathf.Abs(startposX + spacing*3), Mathf.Abs(startposY + spacing*3));
        Debug.Log(startposX);
        graph = new List<List<(int Row, float Value)>>();

        for (int i = 0; i < balls.Count; i++)
        {
            graph.Add(new List<(int Row, float Value)>());
        }
    }

    

    public void DestroyBalls()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            Destroy(balls[i].gameObject);
        }
        balls.Clear();
    }
    private void OnDrawGizmosSelected()
    {
        
        float startposX = (spacing * amountToCreate.x) /2 - spacing/2;
        float startposY = (spacing * amountToCreate.y) /2 - spacing/2;
        for (int i = 0; i < amountToCreate.y; i++)
        {
            for (int j = 0; j < amountToCreate.x; j++)
            {
                Gizmos.DrawSphere(new Vector3(-startposX +j * spacing, -startposY +i * spacing, 0), ball.transform.localScale.x/2);
            }
        }
    }

    private List<List<(int Row, float Value)>> graph;
    private void AssembleGraph()
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
        
        GetRandomStartEndPos();
        
    }
    private void GetRandomStartEndPos()
    {
        startNode = Random.Range(0, balls.Count);

        do
        {
            endNode = Random.Range(0, balls.Count);
        } while (startNode == endNode);
    }

    private void OnDrawGizmos()
    {
        
        
    }
    LineRenderer lineRenderer;
    private List<Vector3> linepos = new List<Vector3>();
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

    public void StartAlgorithm()
    {
        
    }

    public void UpdateAlgorithm()
    {
        
        Profiler.BeginSample("Assemble", this);
        AssembleGraph();
        Profiler.EndSample();
        sampler.Begin();
        dijkstra.StartDijkstra(graph, startNode, endNode);
        sampler.End();
        Profiler.BeginSample("Drawing", this);
        if (dijkstra.foundEndPoint && Application.isPlaying)
        {
            linepos.Clear();
            DrawToCheckPoint(dijkstra.prevVertex, endNode, startNode);
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.positionCount = linepos.Count;
            lineRenderer.useWorldSpace = true;

            lineRenderer.SetPositions( linepos.ToArray()); //x,y and z position of the starting point of the line
            
        }
        else
        {
            lineRenderer.positionCount = 0;
            
        }
        Profiler.EndSample();

    }

    public void UpdateAlgorithm(int i)
    {
        GetRandomStartEndPos();
        Profiler.BeginSample("Assemble", this);
        algorithm.AssembleGraph(balls);
        Profiler.EndSample();
        sampler.Begin();
        algorithm.StartAlgorithm(startNode, endNode);
        sampler.End();
        Profiler.BeginSample("Drawing", this);
        if (algorithm.pathFound)
        {
            Debug.Log("PathFound");
            linepos.Clear();
            
            DrawPath(algorithm.endNode, balls[startNode]);
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.positionCount = linepos.Count;
            lineRenderer.useWorldSpace = true;

            lineRenderer.SetPositions( linepos.ToArray());
        }
        Profiler.EndSample();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}

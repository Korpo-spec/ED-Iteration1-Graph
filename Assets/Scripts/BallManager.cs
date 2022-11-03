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

    private readonly List<Ball> balls = new List<Ball>();
    private CustomSampler sampler;
    // Start is called before the first frame update
    void Start()
    {
        float timebegining = Time.realtimeSinceStartup;
        float startposX = (spacing * amountToCreate.x) /2 - spacing/2;
        float startposY = (spacing * amountToCreate.y) / 2 - spacing/2;
        int currentID = 0;
        for (int i = 0; i < amountToCreate.y; i++)
        {
            for (int j = 0; j < amountToCreate.x; j++)
            {
                var g =Instantiate(ball, new Vector3(-startposX +j * spacing, -startposY + i * spacing, 0), quaternion.identity);
                Ball ballScript = g.GetComponent<Ball>();
                ballScript.ID = currentID;
                currentID++;
                
                balls.Add(ballScript);
            }
        }
        Debug.Log(Time.realtimeSinceStartup - timebegining);
        graph = new List<List<(int Row, float Value)>>();

        for (int i = 0; i < balls.Count; i++)
        {
            graph.Add(new List<(int Row, float Value)>());
        }
        
        sampler = CustomSampler.Create("Dijkstra");
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
        //For drawing line in the world space, provide the x,y,z values
         //x,y and z position of the end point of the line
        //Gizmos.DrawLine(balls[checkpoint].transform.position, balls[prevVertex[checkpoint]].transform.position);
        checkpoint = prevVertex[checkpoint];
        DrawToCheckPoint(prevVertex, checkpoint, startNode);
        
    }

    public void StartAlgorithm()
    {
        
    }

    public void UpdateAlgorithm()
    {
        int checkpoint = 20;
        Profiler.BeginSample("Assemble", this);
        AssembleGraph();
        Profiler.EndSample();
        sampler.Begin();
        dijkstra.StartDijkstra(graph, 40, 20);
        sampler.End();
        Profiler.BeginSample("Drawing", this);
        if (dijkstra.foundEndPoint && Application.isPlaying)
        {
            linepos.Clear();
            DrawToCheckPoint(dijkstra.prevVertex, 20, 40);
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

    // Update is called once per frame
    void Update()
    {
        UpdateAlgorithm();
    }
}

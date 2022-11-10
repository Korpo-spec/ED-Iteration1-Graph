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
    
    [SerializeField] private Algorithbase algorithm;
    [SerializeField] private SimulationBoundaries boundaries;

    private int startNode;
    private int endNode;
    private readonly List<Ball> balls = new List<Ball>();
    private CustomSampler sampler;
    LineRenderer lineRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        
        SpawnBalls(amountToCreate);

        lineRenderer = GetComponent<LineRenderer>();
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
        algorithm.OnStart(balls);
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
    
    private void GetRandomStartEndPos()
    {
        startNode = Random.Range(0, balls.Count);

        do
        {
            endNode = Random.Range(0, balls.Count);
        } while (startNode == endNode);
    }
    

    public void UpdateAlgorithm()
    {
        GetRandomStartEndPos();
        Profiler.BeginSample("Assemble", this);
        algorithm.AssembleGraph(balls);
        Profiler.EndSample();
        sampler.Begin();
        algorithm.StartAlgorithm(startNode, endNode);
        sampler.End();
        Profiler.BeginSample("Drawing", this);
        algorithm.DrawPath(lineRenderer);
        
        Profiler.EndSample();
    }
    
}

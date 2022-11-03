using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{

    [SerializeField] private float speed = 5;

    public List<Ball> ballInRange = new List<Ball>();
    private LineRenderer line;
    public int ID;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 vec = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        vec.randomVec2();
        rb.velocity = vec * speed;
        line = GetComponent<LineRenderer>();
    }

    private List<Vector3> linepos = new List<Vector3>();
    // Update is called once per frame
    void Update()
    {
        
        /*
        for (int i = 0; i < ballInRange.Count; i++)
        {
            linepos.Add(transform.position);
            linepos.Add(ballInRange[i].transform.position);
            
        }
        line.SetPositions(linepos.ToArray());
        */
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < ballInRange.Count; i++)
        {
            Gizmos.DrawLine(transform.position, ballInRange[i].transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Ball>(out Ball ball))
        {
            ballInRange.Add(ball);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Ball>(out Ball ball))
        {
            ballInRange.Remove(ball);
        }
    }
}
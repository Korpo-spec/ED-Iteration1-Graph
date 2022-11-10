using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour, INode
{

    [SerializeField] private float speed = 5;
    [SerializeField] private LayerMask mask;
    [SerializeField] private SimulationBoundaries boundary;

    public List<Ball> ballInRange = new List<Ball>();
    public List<Ball> Nieghbours => ballInRange;
    public INode parent { get; set; }
    public Vector3 position => transform.position;
    private LineRenderer line;
    public int ID;

    private Vector2 direction;


    [HideInInspector]public float hCost { get; set; }
    [HideInInspector]public float gCost { get; set; }
    [HideInInspector]public float fCost => gCost + hCost;
    // Start is called before the first frame update
    void Start()
    {
        //Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 vec = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        vec.randomVec2();
        direction = vec;
        //rb.velocity = vec * speed;
        line = GetComponent<LineRenderer>();
    }

    private List<Vector3> linepos = new List<Vector3>();
    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(direction * (Time.deltaTime *speed), Space.World);

        if (Mathf.Abs(transform.position.x) > boundary.boundaries.x)
        {
            ReflectX();
        }

        if (Mathf.Abs(transform.position.y) > boundary.boundaries.y)
        {
            ReflectY();
        }
        
    }

    private void ReflectX()
    {
        transform.Translate(-direction * (Time.deltaTime *speed), Space.World);
        direction = Vector3.Reflect(direction, Vector3.left);
    }
    
    private void ReflectY()
    {
        transform.Translate(-direction * (Time.deltaTime *speed), Space.World);
        direction = Vector3.Reflect(direction, Vector3.up);
    }

    public void GetNearbyColliders()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 0.3f, mask);
        
        ballInRange.Clear();
        foreach (var c in col)
        {
            ballInRange.Add(c.gameObject.GetComponent<Ball>());
        }
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

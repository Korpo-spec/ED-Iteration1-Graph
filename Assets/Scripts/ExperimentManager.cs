using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class ExperimentManager : MonoBehaviour
{
    private Recorder recorder;

    private ResultWriter resultWriter;

    private List<double> elapsedTimeMilliSec = new List<double>();

    [SerializeField] private BallManager ballManager;

    [SerializeField] private int startCircleAmount;

    [SerializeField] private int endCircleAmount;

    private int currentCircleAmount;
    // Start is called before the first frame update
    void Start()
    {
        recorder = Recorder.Get("Dijkstra");
        resultWriter = new ResultWriter();
        
        resultWriter.WriteData("tests", new List<string>());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCircleAmount >= endCircleAmount) return;
        
        if (recorder.isValid)
        {
            elapsedTimeMilliSec.Add(recorder.elapsedNanoseconds / 1000000f);
        }

        if (elapsedTimeMilliSec.Count >500)
        {
            //resultWriter.WriteData("testData", elapsedTimeMilliSec);
            resultWriter.WriteDataAppend(elapsedTimeMilliSec.GetAvarage(), currentCircleAmount*2 * 25);
            elapsedTimeMilliSec.Clear();
            ballManager.DestroyBalls();
            currentCircleAmount++;
            if (currentCircleAmount < endCircleAmount)
            {
                ballManager.SpawnBalls(new Vector2Int(currentCircleAmount*2, 25));
                
            }
            
            
        }
        else 
        {
            ballManager.UpdateAlgorithm();
        }
    }

    
}

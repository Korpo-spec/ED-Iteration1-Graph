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
    // Start is called before the first frame update
    void Start()
    {
        recorder = Recorder.Get("Dijkstra");
        resultWriter = new ResultWriter();
    }

    // Update is called once per frame
    void Update()
    {
        if (recorder.isValid)
        {
            elapsedTimeMilliSec.Add(recorder.elapsedNanoseconds / 1000000f);
        }

        if (elapsedTimeMilliSec.Count >500)
        {
            resultWriter.WriteData("testData", elapsedTimeMilliSec);
            elapsedTimeMilliSec.Clear();
            ballManager.DestroyBalls();
            
            ballManager.SpawnBalls(new Vector2Int(50,20));
        }
        else
        {
            ballManager.UpdateAlgorithm();
        }
    }
}

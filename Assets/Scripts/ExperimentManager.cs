using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class ExperimentManager : MonoBehaviour
{
    private Recorder recorder;

    private ResultWriter resultWriter;

    private List<double> elapsedTimeMilliSec = new List<double>();

    private List<double> algorithmAvarage = new List<double>();


    [SerializeField] private BallManager ballManager;

    [SerializeField] private int startCircleAmount;

    [SerializeField] private int endCircleAmount;

    [SerializeField] private List<Algorithbase> algorithms;

    private int algorithmIndex = 0;

    private int currentCircleAmount;
    // Start is called before the first frame update
    void Start()
    {
        recorder = Recorder.Get("Dijkstra");
        resultWriter = new ResultWriter();
        List<string> names = new List<string>();
        for (int i = 0; i < algorithms.Count; i++)
        {
            names.Add(algorithms[i].Name);
        }
        
        resultWriter.WriteData("tests", names);
        currentCircleAmount = startCircleAmount;
        
        ballManager.SetAlgorithm(algorithms[0]);
        algorithmIndex = 0;
        ballManager.SpawnBalls(new Vector2Int(currentCircleAmount*2, 25));
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCircleAmount >= endCircleAmount) return;
        
        if (recorder.isValid)
        {
            elapsedTimeMilliSec.Add(recorder.elapsedNanoseconds / 1000000f);
        }

        if (elapsedTimeMilliSec.Count >200)
        {
            //resultWriter.WriteData("testData", elapsedTimeMilliSec);
            algorithmAvarage.Add(elapsedTimeMilliSec.GetAvarage());
            elapsedTimeMilliSec.Clear();
            ballManager.DestroyBalls();
            
            
            algorithmIndex++;
            Debug.Log(algorithmIndex + "Index");
            Debug.Log( currentCircleAmount + "Indexx");
            if (algorithmIndex >= algorithms.Count)
            {
                resultWriter.WriteDataAppend(algorithmAvarage, currentCircleAmount*2 * 25);
                algorithmAvarage.Clear();
                currentCircleAmount++;
                Debug.Log(algorithmIndex + "Indexxx");
                algorithmIndex = 0;
                
            }
            ballManager.SetAlgorithm(algorithms[algorithmIndex]);
            
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

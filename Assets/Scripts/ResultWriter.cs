using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResultWriter
{

    public void WriteData(string rubric, List<double> data)
    {
        using (StreamWriter sw = new StreamWriter("Result.csv"))
        {
            sw.WriteLine(rubric);
            for (int i = 0; i < data.Count; i++)
            {
                sw.WriteLine("");
                sw.Write(data[i].ToString(),true);
            }
        }
    }
    
    public void WriteDataAppend( List<double> data , int circleAmount)
    {
        using (StreamWriter sw = new StreamWriter("Result.csv", true))
        {
            sw.WriteLine();
            for (int i = 0; i < data.Count; i++)
            {
                sw.Write(";");
                sw.Write(data[i].ToString(),true);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResultWriter
{

    public void WriteData(string rubric, List<string> data)
    {
        using (StreamWriter sw = new StreamWriter("Result.csv"))
        {
            sw.Write(rubric);
            for (int i = 0; i < data.Count; i++)
            {
                sw.Write(";");
                sw.Write(data[i].ToString(),true);
            }
        }
    }
    
    public void WriteDataAppend( List<double> data , int circleAmount)
    {
        using (StreamWriter sw = new StreamWriter("Result.csv", true))
        {
            sw.WriteLine();
            sw.Write(circleAmount +";");
            for (int i = 0; i < data.Count; i++)
            {
                
                sw.Write(data[i].ToString(),true);
                sw.Write(";");
            }
        }
    }
    
    public void WriteDataAppend( double data , int circleAmount)
    {
        using (StreamWriter sw = new StreamWriter("Result.csv", true))
        {
            sw.WriteLine();
            sw.Write(circleAmount);
            sw.Write(";");
            sw.Write(data.ToString(),true);
            
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Utility
{

    public static void GetMinMax(double[,] data, out double min, out double max)
    {
        min = double.MaxValue;
        max = double.MinValue;
        int length0 = data.GetLength(0);
        int length1 = data.GetLength(1);
        for (int i = 0; i < length0; i++)
        {
            for (int j = 0; j < length1; j++)
            {
                min = Math.Min(min, data[i, j]);
                max = Math.Max(max, data[i, j]);
            }
        }
    }

    public static void SaveFile(int xn,int yn,double[] spx,double[] spy,double[,] value)
    {
        double min, max;
        GetMinMax(value, out min, out max);
        using (var stream = File.Create("gf.grd"))
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("DSAA");
                writer.WriteLine(xn.ToString() + " " + yn.ToString());
                writer.WriteLine(spx[0].ToString() + " " + spx[xn - 1].ToString());
                writer.WriteLine(spy[0].ToString() + " " + spy[yn - 1].ToString());
                writer.WriteLine(min.ToString() + " " + max.ToString());
                var line = new StringBuilder();
                for (int i = 0; i < xn; i++)
                {
                    for (int j = 0; j < yn; j++)
                    {
                        line.Append(value[i, j]);
                        line.Append(" ");
                    }

                    writer.WriteLine(line.ToString());
                    line.Clear();
                }
            }
        }
    }
}

using UnityEngine;

public class Cube : MonoBehaviour
{
    public double XLength, YLength, ZLength, XCenter, YCenter, ZCenter, Sigma, M, I, A;

    public void SetCube(double xLength, double yLength, double zLength, double xCenter, double yCenter, double zCenter, double sigma)
    {
        XLength = xLength;
        YLength = yLength;
        ZLength = zLength;
        XCenter = xCenter;
        YCenter = yCenter;
        ZCenter = zCenter;
        Sigma = sigma;
    }

    public void SetCube(double xLength, double yLength, double zLength, double xCenter, double yCenter, double zCenter, double m, double i, double a)
    {
        XLength = xLength;
        YLength = yLength;
        ZLength = zLength;
        XCenter = xCenter;
        YCenter = yCenter;
        ZCenter = zCenter;
        M = m;
        I = i;
        A = a;
    }
}

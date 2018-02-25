using UnityEngine;

public class Sphere : MonoBehaviour
{
    public double XCenter, YCenter, CenterDepth, Radius, Sigma, M, I, A;

    public void SetSphere(double xCenter, double yCenter, double centerDepth, double radius, double sigma)
    {
        XCenter = xCenter;
        YCenter = yCenter;
        CenterDepth = centerDepth;
        Radius = radius;
        Sigma = sigma;
    }

    public void SetSphere(double xCenter, double yCenter, double centerDepth, double radius, double m, double i, double a)
    {
        XCenter = xCenter;
        YCenter = yCenter;
        CenterDepth = centerDepth;
        Radius = radius;
        M = m;
        I = i;
        A = a;
    }
}

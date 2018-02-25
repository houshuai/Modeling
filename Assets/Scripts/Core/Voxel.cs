using UnityEngine;

public class Voxel : MonoBehaviour
{
    public int X, Y, Z;
    public double Sigma;
    public Rectangle Rect;

    public void SetVoxel(int x,int y,int z,double sigma,Rectangle rect)
    {
        X = x;
        Y = y;
        Z = z;
        Sigma = sigma;
        Rect = rect;
    }
}

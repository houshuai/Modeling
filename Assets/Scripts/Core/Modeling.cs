using System;
using System.Collections.Generic;
using UnityEngine;

struct Point3
{
    public int X, Y, Z;
    public Point3(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}

sealed class Modeling
{
    int xn, yn, zn, xm, ym, n;
    const double gamma = 6.67e-11;
    double[] spx, spy, px, py, pz, s, a;
    double[,,] rho;
    Point3[] sub;
    List<double> al;
    List<Point3> subl;

    public double[,] g;

    public Modeling(int _xn, int _yn, int _zn)
    {
        xn = _xn;
        yn = _yn;
        zn = _zn;

        px = new double[xn + 1];    //node position
        py = new double[yn + 1];
        pz = new double[zn + 1];
        rho = new double[xn + 2, yn + 2, zn + 2];

        al = new List<double>();    //node density
        subl = new List<Point3>();
    }

    public void SetSensorPosition(double[] xPos, double[] yPos, double[,] gVal)
    {
        xm = gVal.GetLength(1);
        ym = gVal.GetLength(0);
        g = gVal;

        double xStart = xPos[0];
        double xEnd = xPos[xm - 1];
        double xLen = xEnd - xStart;

        double yStart = yPos[0];
        double yEnd = yPos[ym - 1];
        double yLen = yEnd - yStart;

        double zLen = xLen;
        double dx = xLen / xn;
        double dy = yLen / yn;
        double dz = zLen / zn;

        for (int i = 0; i < xn + 1; i++)
        {
            px[i] = xStart + dx * i;
        }
        for (int i = 0; i < yn + 1; i++)
        {
            py[i] = yStart + dy * i;
        }
        for (int i = 0; i < zn + 1; i++)
        {
            pz[i] = 1 + dz * i;         //start from 1 meters underground
        }

        spx = xPos;
        spy = yPos;

    }

    public ChangedRectangle SetAnomalous(int x, int y, int z, double rho1)
    {
        if (x < 0 || x > xn - 1 || y < 0 || y > yn - 1 || z < 0 || z > zn - 1)
        {
            return null;
        }
        rho[x + 1, y + 1, z + 1] = rho1;

        return GetChangedRectangle(x, y, z);
    }

    public ChangedRectangle DeleteAnomalous(int x,int y,int z)
    {
        if (x < 0 || x > xn - 1 || y < 0 || y > yn - 1 || z < 0 || z > zn - 1)
        {
            return null;
        }
        rho[x + 1, y + 1, z + 1] = 0;

        var changed = GetChangedRectangle(x, y, z);
        var deleteChanged = new ChangedRectangle();
        deleteChanged.Added = changed.Removed;
        deleteChanged.Removed = changed.Added;
        return deleteChanged;
    }

    private ChangedRectangle GetChangedRectangle(int x, int y, int z)
    {
        var changed = new ChangedRectangle();

        if (x > 0 && rho[x, y + 1, z + 1] != 0)
        {
            changed.Removed.Add(new Rectangle(new Vector3((float)px[x], (float)py[y + 1], (float)pz[z]),
                new Vector3((float)px[x], (float)py[y], (float)pz[z + 1])));
        }
        else
        {
            changed.Added.Add(new Rectangle(new Vector3((float)px[x], (float)py[y], (float)pz[z]),
                new Vector3((float)px[x], (float)py[y + 1], (float)pz[z + 1])));
        }
        if (x < xn - 1 && rho[x + 2, y + 1, z + 1] != 0)
        {
            changed.Removed.Add(new Rectangle(new Vector3((float)px[x + 1], (float)py[y], (float)pz[z]),
                new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z + 1])));
        }
        else
        {
            changed.Added.Add(new Rectangle(new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z]),
                new Vector3((float)px[x + 1], (float)py[y], (float)pz[z + 1])));
        }

        if (y > 0 && rho[x + 1, y, z + 1] != 0)
        {
            changed.Removed.Add(new Rectangle(new Vector3((float)px[x], (float)py[y], (float)pz[z]),
                new Vector3((float)px[x + 1], (float)py[y], (float)pz[z + 1])));
        }
        else
        {
            changed.Added.Add(new Rectangle(new Vector3((float)px[x + 1], (float)py[y], (float)pz[z]),
                new Vector3((float)px[x], (float)py[y], (float)pz[z + 1])));
        }
        if (y < yn - 1 && rho[x + 1, y + 2, z + 1] != 0)
        {
            changed.Removed.Add(new Rectangle(new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z]),
                new Vector3((float)px[x], (float)py[y + 1], (float)pz[z + 1])));
        }
        else
        {
            changed.Added.Add(new Rectangle(new Vector3((float)px[x], (float)py[y + 1], (float)pz[z]),
                new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z + 1])));
        }

        if (z > 0 && rho[x + 1, y + 1, z] != 0)
        {
            changed.Removed.Add(new Rectangle(new Vector3((float)px[x], (float)py[y + 1], (float)pz[z]),
                new Vector3((float)px[x + 1], (float)py[y], (float)pz[z])));
        }
        else
        {
            changed.Added.Add(new Rectangle(new Vector3((float)px[x], (float)py[y], (float)pz[z]),
                new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z])));
        }
        if (z < zn - 1 && rho[x + 1, y + 1, z + 2] != 0)
        {
            changed.Removed.Add(new Rectangle(new Vector3((float)px[x], (float)py[y], (float)pz[z + 1]),
                new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z + 1])));
        }
        else
        {
            changed.Added.Add(new Rectangle(new Vector3((float)px[x], (float)py[y + 1], (float)pz[z + 1]),
                new Vector3((float)px[x + 1], (float)py[y], (float)pz[z + 1])));
        }

        return changed;
    }

    public void Compute()
    {
        al.Clear();
        subl.Clear();
        double temp;

        for (int i = 0; i < xn + 1; i++)
        {
            for (int j = 0; j < yn + 1; j++)
            {
                for (int k = 0; k < zn + 1; k++)
                {
                    temp = rho[i + 1, j + 1, k] - rho[i + 1, j, k] + rho[i, j, k] - rho[i, j + 1, k] -
                        rho[i + 1, j + 1, k + 1] + rho[i + 1, j, k + 1] - rho[i, j, k + 1] + rho[i, j + 1, k + 1];
                    if (temp != 0)
                    {
                        al.Add(temp);
                        subl.Add(new Point3(i, j, k));
                    }
                }
            }
        }

        n = al.Count;
        s = new double[n];
        a = al.ToArray();
        sub = subl.ToArray();

        for (int i = 0; i < xm; i++)
        {
            for (int j = 0; j < ym; j++)
            {
                dg(i, j);
            }
        }

    }

    private void dg(int x, int y)
    {
        double px_x, py_y, pz_z, r;

        for (int i = 0; i < n; i++)
        {
            px_x = px[sub[i].X] - spx[x];
            py_y = py[sub[i].Y] - spy[y];
            pz_z = pz[sub[i].Z];
            r = Math.Sqrt(px_x * px_x + py_y * py_y + pz_z * pz_z);
            s[i] = px_x * Math.Log(py_y + r) + py_y * Math.Log(px_x + r) - pz_z * Math.Atan(px_x * py_y / pz_z / r);
        }

        for (int i = 0; i < n; i++)
        {
            g[y, x] -= gamma * a[i] * s[i] * 100000;
        }
    }

    private int[,,] isSearched;        //0: not searched, 1: searched and have anomaly, 2: searched and have not anomaly
    private List<Rectangle> rectangleList;

    public List<Rectangle> CreateAnomalyMesh()
    {
        isSearched = new int[xn, yn, zn];
        rectangleList = new List<Rectangle>();
        BFS(0, 0, 0);
        return rectangleList;
    }

    private void BFS(int x, int y, int z)
    {
        if (rho[x + 1, y + 1, z + 1] != 0)
        {
            if (x > 0 && isSearched[x - 1, y, z] == 1)
            {
                rectangleList.Remove(new Rectangle(new Vector3((float)px[x], (float)py[y + 1], (float)pz[z]),
                    new Vector3((float)px[x], (float)py[y], (float)pz[z + 1])));
            }
            else
            {
                rectangleList.Add(new Rectangle(new Vector3((float)px[x], (float)py[y], (float)pz[z]),
                    new Vector3((float)px[x], (float)py[y + 1], (float)pz[z + 1])));
            }
            if (x < xn - 1 && isSearched[x + 1, y, z] == 1)
            {
                rectangleList.Remove(new Rectangle(new Vector3((float)px[x + 1], (float)py[y], (float)pz[z]),
                    new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z + 1])));
            }
            else
            {
                rectangleList.Add(new Rectangle(new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z]),
                    new Vector3((float)px[x + 1], (float)py[y], (float)pz[z + 1])));
            }

            if (y > 0 && isSearched[x, y - 1, z] == 1)
            {
                rectangleList.Remove(new Rectangle(new Vector3((float)px[x], (float)py[y], (float)pz[z]),
                    new Vector3((float)px[x + 1], (float)py[y], (float)pz[z + 1])));
            }
            else
            {
                rectangleList.Add(new Rectangle(new Vector3((float)px[x + 1], (float)py[y], (float)pz[z]),
                    new Vector3((float)px[x], (float)py[y], (float)pz[z + 1])));
            }
            if (y < yn - 1 && isSearched[x, y + 1, z] == 1)
            {
                rectangleList.Remove(new Rectangle(new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z]),
                    new Vector3((float)px[x], (float)py[y + 1], (float)pz[z + 1])));
            }
            else
            {
                rectangleList.Add(new Rectangle(new Vector3((float)px[x], (float)py[y + 1], (float)pz[z]),
                    new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z + 1])));
            }

            if (z > 0 && isSearched[x, y, z - 1] == 1)
            {
                rectangleList.Remove(new Rectangle(new Vector3((float)px[x], (float)py[y + 1], (float)pz[z]),
                    new Vector3((float)px[x + 1], (float)py[y], (float)pz[z])));
            }
            else
            {
                rectangleList.Add(new Rectangle(new Vector3((float)px[x], (float)py[y], (float)pz[z]),
                    new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z])));
            }
            if (z < zn - 1 && isSearched[x, y, z + 1] == 1)
            {
                rectangleList.Remove(new Rectangle(new Vector3((float)px[x], (float)py[y], (float)pz[z + 1]),
                    new Vector3((float)px[x + 1], (float)py[y + 1], (float)pz[z + 1])));
            }
            else
            {
                rectangleList.Add(new Rectangle(new Vector3((float)px[x], (float)py[y + 1], (float)pz[z + 1]),
                    new Vector3((float)px[x + 1], (float)py[y], (float)pz[z + 1])));
            }
            isSearched[x, y, z] = 1;
        }
        else
        {
            isSearched[x, y, z] = 2;
        }

        if (x > 0 && isSearched[x - 1, y, z] == 0)
        {
            BFS(x - 1, y, z);
        }

        if (x < xn - 1 && isSearched[x + 1, y, z] == 0)
        {
            BFS(x + 1, y, z);
        }

        if (y > 0 && isSearched[x, y - 1, z] == 0)
        {
            BFS(x, y - 1, z);
        }

        if (y < yn - 1 && isSearched[x, y + 1, z] == 0)
        {
            BFS(x, y + 1, z);
        }


        if (z > 0 && isSearched[x, y, z - 1] == 0)
        {
            BFS(x, y, z - 1);
        }

        if (z < zn - 1 && isSearched[x, y, z + 1] == 0)
        {
            BFS(x, y, z + 1);
        }


    }


}

public struct Rectangle
{
    public Vector3 LeftUp, RightBottom;

    public Rectangle(Vector3 leftUp, Vector3 rightBottom)
    {
        LeftUp = leftUp;
        RightBottom = rightBottom;
    }
}

public class ChangedRectangle
{
    public List<Rectangle> Added, Removed;

    public ChangedRectangle()
    {
        Added = new List<Rectangle>();
        Removed = new List<Rectangle>();
    }
}

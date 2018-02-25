using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Magnetism : MonoBehaviour
{
    public InputField XLenInput;
    public InputField YLenInput;
    public InputField XCountInput;
    public InputField YCountInput;
    public GameObject InitialMenu;
    public GameObject SensorMenu;
    public GameObject Contour;

    [HideInInspector]
    public float scale;
    private Material mat;
    private Texture2D contourTexture;
    private Colormap colormap = new Colormap();

    private const double mu0 = 4.0 * Math.PI * 1.0e-7;
    private double xlen = 10000;        //sensor area length of x orientation 
    private double ylen = 10000;        //sensor area length of y orientation 
    private int xn = 51;                //sensor count of x orientation
    private int yn = 51;                //sensor count of y orientation
    private double[,] magneticValue;    //result of magnetic modeling
    private double[] spx;              //sensor position of x coordinate
    private double[] spy;              //sensor position of y coordinate

    private List<Sphere> sphereList = new List<Sphere>();
    private List<Cube> cubeList = new List<Cube>();

    private void Start()
    {
        mat = Contour.GetComponent<MeshRenderer>().material;
        colormap.ColormapBrushType = Colormap.ColormapBrushEnum.Jet;
        colormap.SetBrushes();

        XLenInput.text = "10000";
        YLenInput.text = "10000";
        XCountInput.text = "201";
        YCountInput.text = "201";
    }

    public void SetSensor()
    {
        xlen = double.Parse(XLenInput.text);
        ylen = double.Parse(YLenInput.text);
        xn = int.Parse(XCountInput.text);
        yn = int.Parse(YCountInput.text);

        magneticValue = new double[yn, xn];
        spx = new double[xn];
        spy = new double[yn];

        scale = (float)xlen / 10;
        SetSensorPosition();

        contourTexture = new Texture2D(xn, yn, TextureFormat.ARGB32, false);

        SensorMenu.SetActive(false);
        InitialMenu.SetActive(false);
    }

    private void SetSensorPosition()
    {
        double dx = xlen / (xn - 1);
        double dy = ylen / (yn - 1);
        double xStart = -xlen / 2;
        double yStart = -ylen / 2;

        for (int i = 0; i < xn; i++)
        {
            spx[i] = xStart + dx * i;
        }
        for (int i = 0; i < yn; i++)
        {
            spy[i] = yStart + dy * i;
        }

    }

    public void Calc()
    {
        for (int i = 0; i < xn; i++)
        {
            for (int j = 0; j < yn; j++)
            {
                magneticValue[i, j] = 0;
            }
        }

        foreach (var sphere in sphereList)
        {
            SphereAnomaly(sphere);
        }

        foreach (var cube in cubeList)
        {
            CubeAnomaly(cube);
        }

        SetContour();
    }

    private void SetContour()
    {
        double min, max;
        Utility.GetMinMax(magneticValue, out min, out max);

        for (int y = 0; y < yn; y++)
        {
            for (int x = 0; x < xn; x++)
            {
                contourTexture.SetPixel(x, y, colormap.GetBrush(magneticValue[y, x], min, max));
            }
        }
        contourTexture.Apply();

        mat.SetTexture("_MainTex", contourTexture);
    }

    public void AddSphere(Sphere sphere)
    {
        sphereList.Add(sphere);
    }

    public void DeleteSphere(Sphere sphere)
    {
        sphereList.Remove(sphere);
    }

    public void AddCube(Cube cube)
    {
        cubeList.Add(cube);
    }

    public void DeleteCube(Cube cube)
    {
        cubeList.Remove(cube);
    }

    private void SphereAnomaly(Sphere sphere)
    {
        double xCenter = sphere.XCenter;
        double yCenter = sphere.YCenter;
        double centerDepth = sphere.CenterDepth;
        double radius = sphere.Radius;
        double M = sphere.M;
        double I = sphere.I * Math.PI / 180.0;
        double A = sphere.A * Math.PI / 180.0;

        double m = 4.0 * Math.PI * radius * radius * radius * M / 3.0;
        double mu0m_4pi = (mu0 * m) / (4.0 * Math.PI);
        double l0 = Math.Cos(I) * Math.Cos(A);
        double m0 = Math.Cos(I) * Math.Sin(A);
        double n0 = Math.Sin(I);
        double rx, ry, rz, r, r2, r5, rlmn, hx, hy, z;

        for (int y = 0; y < yn; y++)
        {
            for (int x = 0; x < xn; x++)
            {
                rx = spx[x] - xCenter;
                ry = spy[y] - yCenter;
                rz = centerDepth;
                r2 = rx * rx + ry * ry + rz * rz;
                r = Math.Sqrt(r2);
                r5 = r2 * r2 * r;
                rlmn = rx * l0 + ry * m0 - rz * n0;
                hx = -mu0m_4pi * (r2 * l0 - 3.0 * rx * rlmn) / r5;
                hy = -mu0m_4pi * (r2 * m0 - 3.0 * ry * rlmn) / r5;
                z = -mu0m_4pi * (r2 * n0 + 3.0 * rz * rlmn) / r5;
                magneticValue[y, x] += (hx * l0 + hy * m0 + z * n0) * 1.0e9;
            }
        }
    }

    private void CubeAnomaly(Cube cube)
    {
        double a = cube.XLength / 2;
        double b = cube.YLength / 2;
        double c = cube.ZLength / 2;
        double xCenter = cube.XCenter;
        double yCenter = cube.YCenter;
        double zCenter = cube.ZCenter;
        double M = cube.M;
        double I = cube.I * Math.PI / 180.0;
        double A = cube.A * Math.PI / 180.0;

        double mu0M_4pi = (mu0 * M) / (4.0 * Math.PI);
        double l0 = Math.Cos(I) * Math.Cos(A);
        double m0 = Math.Cos(I) * Math.Sin(A);
        double n0 = Math.Sin(I);
        double k1 = 2 * l0 * l0;
        double k2 = 2 * m0 * m0;
        double k3 = 2 * n0 * n0;
        double k4 = 2 * m0 * n0;
        double k5 = 2 * l0 * n0;
        double k6 = 2 * l0 * m0;
        double sx, sy, x1, x2, y1, y2, z1, z2;

        var calc = new CubeCalc(k1, k2, k3, k4, k5, k6);

        for (int y = 0; y < yn; y++)
        {
            for (int x = 0; x < xn; x++)
            {
                sx = spx[x]; sy = spy[y];
                x1 = xCenter - a - sx; x2 = xCenter + a - sx;
                y1 = yCenter - b - sy; y2 = yCenter + b - sy;
                z1 = zCenter - c; z2 = zCenter + c;
                
                var t = calc.Func(x2, y2, z2) - calc.Func(x1, y2, z2) - calc.Func(x2, y1, z2) + calc.Func(x1, y1, z2) -
                    calc.Func(x2, y2, z1) + calc.Func(x1, y2, z1) + calc.Func(x2, y1, z1) - calc.Func(x1, y1, z1);

                magneticValue[y, x] += t * mu0M_4pi * 1.0e9;
            }
        }
    }

    public void SaveFile()
    {
        Utility.SaveFile(xn, yn, spx, spy, magneticValue);
    }

    sealed class CubeCalc
    {
        private double k1, k2, k3, k4, k5, k6;
        private double sx, sy;

        public CubeCalc(double k1, double k2, double k3, double k4, double k5, double k6)
        {
            this.k1 = k1; this.k2 = k2; this.k3 = k3;
            this.k4 = k4; this.k5 = k5; this.k6 = k6;
        }

        public double Func(double x, double y, double z)
        {
            double r = Math.Sqrt(x * x + y * y + z * z);
            double result = -k1 * Math.Atan(x / (y + z + r)) - k2 * Math.Atan(y / (x + z + r)) + k3 * Math.Atan((x + y + r) / z) +
                k4 * Math.Log(x + r) + k5 * Math.Log(y + r) + k6 * Math.Log(z + r);
            return result;
        }
    }
}

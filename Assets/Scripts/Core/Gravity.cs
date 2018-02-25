using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Gravity : MonoBehaviour
{
    public InputField XLenInput;
    public InputField YLenInput;
    public InputField XCountInput;
    public InputField YCountInput;
    public InputField XGridInput;
    public InputField YGridInput;
    public InputField ZGridInput;
    public GameObject rectangle;
    public GameObject InitialMenu;
    public GameObject SensorMenu;
    public GameObject GridMenu;
    public GameObject Contour;

    [HideInInspector]
    public float scale;                //scale of the render model
    private Material mat;
    private Texture2D contourTexture;
    private Colormap colormap = new Colormap();

    private const double G = 6.672e-11;       //
    private double xlen = 10000;        //sensor area length of x orientation 
    private double ylen = 10000;        //sensor area length of y orientation 
    private int xn = 51;                //sensor count of x orientation
    private int yn = 51;                //sensor count of y orientation
    private double[,] gravityValue;    //result of gravity modeling
    private double[] spx;              //sensor position of x coordinate
    private double[] spy;              //sensor position of y coordinate
    private Modeling gm;

    private List<Sphere> sphereList = new List<Sphere>();
    private List<Cube> cubeList = new List<Cube>();

    private List<GameObject> rectObjList = new List<GameObject>();

    private void Start()
    {
        mat = Contour.GetComponent<MeshRenderer>().material;
        colormap.ColormapBrushType = Colormap.ColormapBrushEnum.Jet;
        colormap.SetBrushes();

        XLenInput.text = "10000";
        YLenInput.text = "10000";
        XCountInput.text = "201";
        YCountInput.text = "201";

        XGridInput.text = "10";
        YGridInput.text = "10";
        ZGridInput.text = "10";
    }

    public void SetSensor()
    {
        xlen = double.Parse(XLenInput.text);
        ylen = double.Parse(YLenInput.text);
        xn = int.Parse(XCountInput.text);
        yn = int.Parse(YCountInput.text);

        gravityValue = new double[yn, xn];
        spx = new double[xn];
        spy = new double[yn];

        SetSensorPosition();

        scale = (float)xlen / 10;
        contourTexture = new Texture2D(xn, yn, TextureFormat.ARGB32, false);

        SensorMenu.SetActive(false);
        GridMenu.SetActive(true);
    }

    public void SetGrid()
    {
        int xg = int.Parse(XGridInput.text);
        int yg = int.Parse(YGridInput.text);
        int zg = int.Parse(ZGridInput.text);

        gm = new Modeling(xg, yg, zg);
        gm.SetSensorPosition(spx, spy, gravityValue);

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

    private void SetAnomalyMesh()
    {
        var rectList = gm.CreateAnomalyMesh();
        foreach (var rect in rectList)
        {
            //InstantiateRectangle(rect);
        }
    }

    private void SetRectangle(ChangedRectangle rectangles, int x, int y, int z, double sigma, bool isDelete)
    {
        if (rectangles != null)
        {
            foreach (var added in rectangles.Added)
            {
                InstantiateRectangle(added, x, y, z, sigma, isDelete);
            }

            foreach (var removed in rectangles.Removed)
            {
                DestroyRectangle(removed);
            }
        }
    }

    private void InstantiateRectangle(Rectangle rect, int x, int y, int z, double sigma, bool isDeleteCustom)
    {
        var pos = new Vector3((rect.LeftUp.x + rect.RightBottom.x) / 2 / scale,
                -(rect.LeftUp.z + rect.RightBottom.z) / 2 / scale, (rect.LeftUp.y + rect.RightBottom.y) / 2 / scale);
        Vector3 direction, scal;
        if (rect.LeftUp.x == rect.RightBottom.x)
        {
            if (rect.LeftUp.y < rect.RightBottom.y)
            {
                direction = new Vector3(1, 0, 0);
                if (isDeleteCustom)
                {
                    x++;
                }
                scal = new Vector3((rect.RightBottom.y - rect.LeftUp.y) / scale, (rect.RightBottom.z - rect.LeftUp.z) / scale, 1);
            }
            else
            {
                direction = new Vector3(-1, 0, 0);
                if (isDeleteCustom)
                {
                    x--;
                }
                scal = new Vector3((rect.LeftUp.y - rect.RightBottom.y) / scale, (rect.RightBottom.z - rect.LeftUp.z) / scale, 1);
            }
        }
        else if (rect.LeftUp.y == rect.RightBottom.y)
        {
            if (rect.LeftUp.x < rect.RightBottom.x)
            {
                direction = new Vector3(0, 0, -1);
                if (isDeleteCustom)
                {
                    y--;
                }
                scal = new Vector3((rect.LeftUp.x - rect.RightBottom.x) / scale, (rect.RightBottom.z - rect.LeftUp.z) / scale, 1);
            }
            else
            {
                direction = new Vector3(0, 0, 1);
                if (isDeleteCustom)
                {
                    y++;
                }
                scal = new Vector3((rect.RightBottom.x - rect.LeftUp.x) / scale, (rect.RightBottom.z - rect.LeftUp.z) / scale, 1);
            }
        }
        else
        {
            if (rect.LeftUp.y < rect.RightBottom.y)
            {
                direction = new Vector3(0, -1, 0);
                if (isDeleteCustom)
                {
                    z++;
                }
                scal = new Vector3((rect.RightBottom.x - rect.LeftUp.x) / scale, (rect.RightBottom.y - rect.LeftUp.y) / scale, 1);
            }
            else
            {
                direction = new Vector3(0, 1, 0);
                if (isDeleteCustom)
                {
                    z--;
                }
                scal = new Vector3((rect.RightBottom.x - rect.LeftUp.x) / scale, (rect.LeftUp.y - rect.RightBottom.y) / scale, 1);
            }
        }

        var rectObj = Instantiate(rectangle, pos, Quaternion.identity);
        rectObj.tag = "Anomaly";
        rectObj.transform.forward = direction;
        rectObj.transform.localScale = scal;
        var voxel = rectObj.AddComponent<Voxel>();
        voxel.SetVoxel(x, y, z, sigma, rect);
        rectObjList.Add(rectObj);
    }

    private void DestroyRectangle(Rectangle rect)
    {
        var pos = new Vector3((rect.LeftUp.x + rect.RightBottom.x) / 2 / scale,
                -(rect.LeftUp.z + rect.RightBottom.z) / 2 / scale, (rect.LeftUp.y + rect.RightBottom.y) / 2 / scale);
        for (int i = 0; i < rectObjList.Count; i++)
        {
            var rectObj = rectObjList[i];
            if (rectObj.transform.position == pos)
            {
                rectObjList.Remove(rectObj);
                Destroy(rectObj);
            }
        }
    }

    public void Calc()
    {
        for (int j = 0; j < yn; j++)
        {
            for (int i = 0; i < xn; i++)
            {
                gravityValue[j, i] = 0;
            }
        }

        gm.Compute();

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
        Utility.GetMinMax(gravityValue, out min, out max);

        for (int y = 0; y < yn; y++)
        {
            for (int x = 0; x < xn; x++)
            {
                //contourTexture.SetPixel(i, j, colormap.GetBrush(g1.g[i, j], g1.min, g1.max));
                contourTexture.SetPixel(x, y, colormap.GetBrush(gravityValue[y, x], min, max));
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

    public void AddCustom(int x, int y, int z, double sigma)
    {
        var rectangles = gm.SetAnomalous(x, y, z, sigma);

        SetRectangle(rectangles, x, y, z, sigma, false);
    }

    public void DeleteCustom(Voxel voxel)
    {
        var rectangles = gm.DeleteAnomalous(voxel.X, voxel.Y, voxel.Z);

        SetRectangle(rectangles, voxel.X, voxel.Y, voxel.Z, voxel.Sigma, true);
    }

    private void SphereAnomaly(Sphere sphere)
    {
        double xCenter = sphere.XCenter;
        double yCenter = sphere.YCenter;
        double centerDepth = sphere.CenterDepth;
        double radius = sphere.Radius;
        double sigma = sphere.Sigma;

        double M = 4.0 / 3.0 * Math.PI * radius * radius * radius * sigma;
        double GMD = G * M * centerDepth;
        double xi, yj;

        for (int y = 0; y < yn; y++)
        {
            for (int x = 0; x < xn; x++)
            {
                xi = spx[x] - xCenter;
                xi *= xi;
                yj = spy[y] - yCenter;
                yj *= yj;
                gravityValue[y, x] += GMD / Math.Pow(xi + yj + centerDepth * centerDepth, 1.5) * 100000;
            }
        }

    }

    private void CubeAnomaly(Cube cube)
    {
        double a = cube.XLength / 2;
        double b = cube.YLength / 2;
        double c = cube.ZLength / 2;
        double sigma = cube.Sigma;
        double xCenter = cube.XCenter;
        double yCenter = cube.YCenter;
        double zCenter = cube.ZCenter;
        double sx, sy, x1, x2, y1, y2, z1, z2;

        var calc = new CubeCalc();

        for (int y = 0; y < yn; y++)
        {
            for (int x = 0; x < xn; x++)
            {
                sx = spx[x]; sy = spy[y];
                x1 = xCenter - a - sx; x2 = xCenter + a - sx;
                y1 = yCenter - b - sy; y2 = yCenter + b - sy;
                z1 = zCenter - c; z2 = zCenter + c;
                var g = calc.Func(x2, y2, z2) - calc.Func(x1, y2, z2) - calc.Func(x2, y1, z2) + calc.Func(x1, y1, z2) -
                    calc.Func(x2, y2, z1) + calc.Func(x1, y2, z1) + calc.Func(x2, y1, z1) - calc.Func(x1, y1, z1);

                gravityValue[y, x] += G * sigma * -g * 100000;
            }
        }
    }

    private int PowOfMinusOne(int count)
    {
        int result = -1;
        for (int i = 1; i < count; i++)
        {
            result *= -1;
        }
        return result;
    }

    public void SaveFile()
    {
        Utility.SaveFile(xn, yn, spx, spy, gravityValue);
    }

    sealed class CubeCalc
    {
        private double k1, k2, k3, k4, k5, k6;
        private double sx, sy;

        public CubeCalc()
        {

        }

        public double Func(double x, double y, double z)
        {
            double r = Math.Sqrt(x * x + y * y + z * z);
            double result = x * Math.Log(y + r) + y * Math.Log(x + r) - z * Math.Atan(x * y / z / r);
            return result;
        }
    }
}




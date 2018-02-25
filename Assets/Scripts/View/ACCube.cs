using UnityEngine;
using UnityEngine.UI;

public class ACCube : MonoBehaviour
{
    public InputField xLength;
    public InputField yLength;
    public InputField zLength;
    public InputField xCenter;
    public InputField yCenter;
    public InputField zCenter;
    public InputField M;
    public InputField I;
    public InputField A;
    public Material outlineMat;

    [HideInInspector]
    public GameObject SelectedCube
    {
        get { return selectedCube; }
        set
        {
            selectedCube = value;
            if (selectedCube)
            {
                cube = selectedCube.GetComponent<Cube>();
                xLength.text = cube.XLength.ToString();
                yLength.text = cube.YLength.ToString();
                zLength.text = cube.ZLength.ToString();
                xCenter.text = cube.XCenter.ToString();
                yCenter.text = cube.YCenter.ToString();
                zCenter.text = cube.ZCenter.ToString();
                M.text = cube.M.ToString();
                I.text = cube.I.ToString();
                A.text = cube.A.ToString();
            }
            else
            {
                ResetMenu();
            }
        }
    }

    private GameObject selectedCube;
    private Cube cube;
    private Magnetism magnetism;

    private void Start()
    {
        magnetism = FindObjectOfType<Magnetism>();
    }

    private void ResetMenu()
    {
        cube = null;
        xLength.text = null;
        yLength.text = null;
        zLength.text = null;
        xCenter.text = null;
        yCenter.text = null;
        zCenter.text = null;
        M.text = null;
        I.text = null;
        A.text = null;
    }

    public void AddOrChange()
    {
        var xl = double.Parse(xLength.text);
        var yl = double.Parse(yLength.text);
        var zl = double.Parse(zLength.text);
        var xc = double.Parse(xCenter.text);
        var yc = double.Parse(yCenter.text);
        var zc = double.Parse(zCenter.text);

        if (!selectedCube)
        {
            selectedCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            selectedCube.GetComponent<Renderer>().material = outlineMat;
            selectedCube.tag = "Anomaly";
            cube = selectedCube.AddComponent<Cube>();
            magnetism.AddCube(cube);
        }

        cube.SetCube(xl, yl, zl, xc, yc, zc, double.Parse(M.text), double.Parse(I.text), double.Parse(A.text));

        selectedCube.transform.position = new Vector3((float)xc / magnetism.scale, (float)-zc / magnetism.scale, (float)yc / magnetism.scale);
        selectedCube.transform.localScale = new Vector3((float)xl / magnetism.scale, (float)zl / magnetism.scale, (float)yl / magnetism.scale);
    }

    public void DeleteCube()
    {
        if (selectedCube)
        {
            magnetism.DeleteCube(cube);
            Destroy(selectedCube);
            ResetMenu();
        }
    }
}

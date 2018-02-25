using UnityEngine;
using UnityEngine.UI;

public class AddOrChangeCube : MonoBehaviour
{
    public InputField xLength;
    public InputField yLength;
    public InputField zLength;
    public InputField xCenter;
    public InputField yCenter;
    public InputField zCenter;
    public InputField sigma;
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
                sigma.text = (cube.Sigma / 1000).ToString();
            }
            else
            {
                ResetMenu();
            }
        }
    }

    private GameObject selectedCube;
    private Cube cube;
    private Gravity gravity;

    private void Start()
    {
        gravity = FindObjectOfType<Gravity>();
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
        sigma.text = null;
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
            gravity.AddCube(cube);
        }

        cube.SetCube(xl, yl, zl, xc, yc, zc, double.Parse(sigma.text) * 1000);

        selectedCube.transform.position = new Vector3((float)xc / gravity.scale, (float)-zc / gravity.scale, (float)yc / gravity.scale);
        selectedCube.transform.localScale = new Vector3((float)xl / gravity.scale, (float)zl / gravity.scale, (float)yl / gravity.scale);
    }

    public void DeleteCube()
    {
        if (selectedCube)
        {
            gravity.DeleteCube(cube);
            Destroy(selectedCube);
            ResetMenu();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ACSphere : MonoBehaviour
{
    public InputField xCenter;
    public InputField yCenter;
    public InputField centerDepth;
    public InputField radius;
    public InputField M;
    public InputField I;
    public InputField A;
    public Material outlineMat;

    [HideInInspector]
    public GameObject SelectedSphere
    {
        get { return selectedSphere; }
        set
        {
            selectedSphere = value;
            if (selectedSphere)
            {
                sphere = selectedSphere.GetComponent<Sphere>();
                xCenter.text = sphere.XCenter.ToString();
                yCenter.text = sphere.YCenter.ToString();
                centerDepth.text = sphere.CenterDepth.ToString();
                radius.text = sphere.Radius.ToString();
                M.text = sphere.M.ToString();
                I.text = sphere.I.ToString();
                A.text = sphere.A.ToString();
            }
            else
            {
                ResetMenu();
            }
        }
    }

    private GameObject selectedSphere;
    private Sphere sphere;
    private Magnetism magnetism;

    private void Start()
    {
        magnetism = FindObjectOfType<Magnetism>();
    }

    private void ResetMenu()
    {
        sphere = null;
        xCenter.text = null;
        yCenter.text = null;
        centerDepth.text = null;
        radius.text = null;
        M.text = null;
        I.text = null;
        A.text = null;
    }

    public void AddOrChange()
    {
        var x = double.Parse(xCenter.text);
        var y = double.Parse(yCenter.text);
        var z = double.Parse(centerDepth.text);
        var r = double.Parse(radius.text);

        if (!selectedSphere)
        {
            selectedSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            selectedSphere.GetComponent<Renderer>().material = outlineMat;
            selectedSphere.tag = "Anomaly";
            sphere = selectedSphere.AddComponent<Sphere>();
            magnetism.AddSphere(sphere);
        }

        sphere.SetSphere(x, y, z, r, double.Parse(M.text), double.Parse(I.text),double.Parse(A.text));

        var scale = (float)r * 2 / magnetism.scale;
        selectedSphere.transform.position = new Vector3((float)x / magnetism.scale, (float)-z / magnetism.scale, (float)y / magnetism.scale);
        selectedSphere.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void DeleteSphere()
    {
        magnetism.DeleteSphere(sphere);
        Destroy(selectedSphere);
        ResetMenu();
    }
}

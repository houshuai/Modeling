using UnityEngine;
using UnityEngine.UI;

public class AddOrChangeSphere : MonoBehaviour
{
    public InputField xCenter;
    public InputField yCenter;
    public InputField centerDepth;
    public InputField radius;
    public InputField sigma;
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
                sigma.text = (sphere.Sigma / 1000).ToString();
            }
            else
            {
                ResetMenu();
            }
        }
    }

    private GameObject selectedSphere;
    private Sphere sphere;
    private Gravity gravity;

    private void Start()
    {
        gravity = FindObjectOfType<Gravity>();
    }

    private void ResetMenu()
    {
        sphere = null;
        xCenter.text = null;
        yCenter.text = null;
        centerDepth.text = null;
        radius.text = null;
        sigma.text = null;
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
            gravity.AddSphere(sphere);
        }

        sphere.SetSphere(x, y, z, r, double.Parse(sigma.text) * 1000);

        var scale = (float)r * 2 / gravity.scale;
        selectedSphere.transform.position = new Vector3((float)x / gravity.scale, (float)-z / gravity.scale, (float)y / gravity.scale);
        selectedSphere.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void DeleteSphere()
    {
        gravity.DeleteSphere(sphere);
        Destroy(selectedSphere);
        ResetMenu();
    }
}

using UnityEngine;
using UnityEngine.UI;

class AddAnomaly : MonoBehaviour
{
    public InputField xInitial;
    public InputField yInitial;
    public InputField zInitial;
    public InputField sigma;

    private Gravity gravity;

    private void Start()
    {
        gravity = FindObjectOfType<Gravity>();
    }

    public void Add()
    {
        var x = int.Parse(xInitial.text);
        var y = int.Parse(yInitial.text);
        var z = int.Parse(zInitial.text);

        AddVoxel(x, y, z, double.Parse(sigma.text) * 1000);
    }

    public void AddVoxel(int x, int y, int z, double sigma)
    {
        gravity.AddCustom(x, y, z, sigma);
    }

    public void DeleteVoxel(Voxel voxel)
    {
        gravity.DeleteCustom(voxel);
    }
}


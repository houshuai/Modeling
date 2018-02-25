using UnityEngine;
using UnityEngine.UI;

public class SelectGraorMag : MonoBehaviour {

    public Toggle graTog;
    public Toggle magTog;
    public GameObject gra;
    public GameObject mag;

    public void Do()
    {
        if (graTog.isOn)
        {
            gra.SetActive(true);
        }
        else if (magTog.isOn)
        {
            mag.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}

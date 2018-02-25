using UnityEngine;
using UnityEngine.EventSystems;

public class SObject : MonoBehaviour
{
    public Material outlineMat;
    public Material defaultMat;
    public GameObject sphereMenu;
    public GameObject cubeMenu;
    public GameObject DeleteButton;

    private ACSphere sphereSet;
    private ACCube cubeSet;

    private int currIndex = 0;

    private void Start()
    {
        sphereSet = sphereMenu.GetComponent<ACSphere>();
        cubeSet = cubeMenu.GetComponent<ACCube>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (sphereSet.SelectedSphere)
            {
                sphereSet.SelectedSphere.GetComponent<Renderer>().material = defaultMat;
                sphereSet.SelectedSphere = null;
            }
            if (cubeSet.SelectedCube)
            {
                cubeSet.SelectedCube.GetComponent<Renderer>().material = defaultMat;
                cubeSet.SelectedCube = null;
            }

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.gameObject.tag == "Anomaly")
                {
                    var anomaly = hit.collider.gameObject;
                    
                    anomaly.GetComponent<Renderer>().material = outlineMat;
                    if (anomaly.GetComponent<Cube>())
                    {
                        cubeSet.SelectedCube = anomaly;
                        cubeMenu.SetActive(true);
                        sphereMenu.SetActive(false);
                    }
                    else if (anomaly.GetComponent<Sphere>())
                    {
                        cubeMenu.SetActive(false);
                        sphereMenu.SetActive(true);
                        sphereSet.SelectedSphere = anomaly;
                    }
                    DeleteButton.SetActive(true);
                    return;
                }
            }
            DeleteButton.SetActive(false);
            SelectAnomaly(currIndex);
        }
    }

    public void SelectAnomaly(int index)
    {
        if (index == 0)
        {
            currIndex = 0;
            cubeMenu.SetActive(true);
            sphereMenu.SetActive(false);
        }
        else if (index == 1)
        {
            currIndex = 1;
            cubeMenu.SetActive(false);
            sphereMenu.SetActive(true);
        }
        else if (index == 2)
        {
            currIndex = 2;
            cubeMenu.SetActive(false);
            sphereMenu.SetActive(false);
        }
    }

    public void DeleteAnomaly()
    {
        if (cubeSet.SelectedCube)
        {
            cubeSet.DeleteCube();
        }
        if (sphereSet.SelectedSphere)
        {
            sphereSet.DeleteSphere();
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class SelectObject : MonoBehaviour
{
    public Material outlineMat;
    public Material defaultMat;
    public GameObject sphereMenu;
    public GameObject cubeMenu;
    public GameObject customMenu;
    public GameObject DeleteButton;

    private AddOrChangeSphere sphereSet;
    private AddOrChangeCube cubeSet;
    private AddAnomaly anomalySet;

    private int currIndex = 0;

    private void Start()
    {
        sphereSet = sphereMenu.GetComponent<AddOrChangeSphere>();
        cubeSet = cubeMenu.GetComponent<AddOrChangeCube>();
        anomalySet = customMenu.GetComponent<AddAnomaly>();
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

                    var voxel = anomaly.GetComponent<Voxel>();
                    if (voxel!=null)
                    {
                        AddVoxel(voxel);
                        return;
                    }

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

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.gameObject.tag == "Anomaly")
                {
                    var anomaly = hit.collider.gameObject;

                    var voxel = anomaly.GetComponent<Voxel>();
                    if (voxel != null)
                    {
                        DeleteVoxel(voxel);
                    }
                    
                }
            }
        }
    }

    private void AddVoxel(Voxel voxel)
    {
        int x = voxel.X;
        int y = voxel.Y;
        int z = voxel.Z;
        var rect = voxel.Rect;

        if (rect.LeftUp.x == rect.RightBottom.x)
        {
            if (rect.LeftUp.y < rect.RightBottom.y)
            {
                x--;
            }
            else
            {
                x++;
            }
        }
        else if (rect.LeftUp.y == rect.RightBottom.y)
        {
            if (rect.LeftUp.x < rect.RightBottom.x)
            {
                y++;
            }
            else
            {
                y--;
            }
        }
        else
        {
            if (rect.LeftUp.y < rect.RightBottom.y)
            {
                z--;
            }
            else
            {
                z++;
            }
        }
        anomalySet.AddVoxel(x, y, z, voxel.Sigma);
    }

    private void DeleteVoxel(Voxel voxel)
    {
        anomalySet.DeleteVoxel(voxel);
    }

    public void SelectAnomaly(int index)
    {
        if (index == 0)
        {
            currIndex = 0;
            cubeMenu.SetActive(true);
            sphereMenu.SetActive(false);
            customMenu.SetActive(false);
        }
        else if (index == 1)
        {
            currIndex = 1;
            cubeMenu.SetActive(false);
            sphereMenu.SetActive(true);
            customMenu.SetActive(false);
        }
        else if (index==2)
        {
            currIndex = 2;
            cubeMenu.SetActive(false);
            sphereMenu.SetActive(false);
            customMenu.SetActive(true);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager2 : MonoBehaviour
{
    public GameObject[] shapePrefabs;               // Array of different shape prefabs
    public Material[] colorMaterials;               // Array of different color materials
    public TMP_InputField rowsInputField;           // Input field for number of rows
    public TMP_InputField columnsInputField;        // Input field for number of columns
    public TMP_InputField rowSpacingInputField;     // Input field for row spacing
    public TMP_InputField columnSpacingInputField;  // Input field for column spacing
    public Button generateButton;                   // Button to regenerate the grid
    public Camera mainCamera;                       // Main camera for y-axis control
    public Slider cameraYPositionSlider;            // Slider to control the camera's y-axis position
    public TMP_Text cameraPositionLabel;            // Text to display camera position

    //instanced meshes
    private Dictionary<Mesh, List<Matrix4x4>> meshInstances = new Dictionary<Mesh, List<Matrix4x4>>();
    //material
    private Dictionary<Mesh, Material> meshMaterialMap = new Dictionary<Mesh, Material>();

    void Start()
    {
        cameraYPositionSlider.value = 50;
        cameraPositionLabel.text = "Camera Height: " + mainCamera.transform.position.y.ToString("F2");
        generateButton.onClick.AddListener(Regen);
        cameraYPositionSlider.onValueChanged.AddListener(ChangeZoom);

        // Init data
        rowsInputField.text = "5";
        columnsInputField.text = "5";
        rowSpacingInputField.text = "3";
        columnSpacingInputField.text = "3";

        GenerateGrid(false);
    }

    public void Regen()
    {
        GenerateGrid(true);
    }

    private void GenerateGrid(bool notInit)
    {
        if (rowsInputField.text.Length == 0 ||
            columnsInputField.text.Length == 0 ||
            rowSpacingInputField.text.Length == 0 ||
            columnsInputField.text.Length == 0)
        {
            Debug.LogError("Fill all Input Fields");
            return;
        }

        // Clear existing grid data
        ClearGrid();

        // Parse values from input fields
        int rows = notInit ? int.Parse(rowsInputField.text) : 5;
        int columns = notInit ? int.Parse(columnsInputField.text) : 5;
        float rowSpacing = notInit ? float.Parse(rowSpacingInputField.text) : 3;
        float columnSpacing = notInit ? float.Parse(columnSpacingInputField.text) : 3;

        // Calculate the starting position to center the grid
        Vector3 startPos = new Vector3(-((rows - 1) * rowSpacing) / 2, 0, -((columns - 1) * columnSpacing) / 2);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Choose a random prefab and get its mesh
                GameObject shapePrefab = shapePrefabs[Random.Range(0, shapePrefabs.Length)];
                Mesh mesh = shapePrefab.GetComponent<MeshFilter>().sharedMesh;

                // Calculate position
                Vector3 position = startPos + new Vector3(i * rowSpacing, 0, j * columnSpacing);

                // Set random scale
                float scale = Random.Range(0.4f, 1.5f);
                Vector3 localScale = Vector3.one * scale;

                // Set random rotation
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);

                // Create the transformation matrix
                Matrix4x4 transformationMatrix = Matrix4x4.TRS(position, rotation, localScale);

                // Select a random material
                Material colorMaterial = colorMaterials[Random.Range(0, colorMaterials.Length)];

                // Group by mesh and store transformation
                if (!meshInstances.ContainsKey(mesh))
                {
                    meshInstances[mesh] = new List<Matrix4x4>();
                    meshMaterialMap[mesh] = colorMaterial;
                }
                meshInstances[mesh].Add(transformationMatrix);
            }
        }
    }

    public void ClearGrid()
    {
        meshInstances.Clear();
        meshMaterialMap.Clear();

        // Destroy instantiated objects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ChangeZoom(float newV)
    {
        if (mainCamera != null)
        {
            Vector3 cameraPosition = mainCamera.transform.position;
            cameraPosition.y = cameraYPositionSlider.value;
            mainCamera.transform.position = cameraPosition;

            cameraPositionLabel.text = "Camera Height: " + cameraPosition.y.ToString("F2");
        }
    }

    void Update()
    {
        // Draw instanced meshes
        foreach (var pair in meshInstances)
        {
            Mesh mesh = pair.Key;
            Material material = meshMaterialMap[mesh];
            List<Matrix4x4> transforms = pair.Value;

            // Draw instanced meshes using Graphics.DrawMeshInstanced
            Graphics.DrawMeshInstanced(mesh, 0, material, transforms, null, UnityEngine.Rendering.ShadowCastingMode.On, true, gameObject.layer);
        }
    }
}

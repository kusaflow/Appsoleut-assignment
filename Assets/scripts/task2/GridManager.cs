using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    public GameObject[] shapePrefabs;               // different shape
    public Material[] colorMaterials;               // different color
    public TMP_InputField rowsInputField;           // Input field for number of rows
    public TMP_InputField columnsInputField;        // Input field for number of columns
    public TMP_InputField rowSpacingInputField;     // Input field for row spacing
    public TMP_InputField columnSpacingInputField;  // Input field for column spacing
    public Button generateButton;                   // regenerate the grid
    public Camera mainCamera;                       // Main camera
    public Slider cameraYPositionSlider;            // Slider to control the camera's y-axis position
    public TMP_Text cameraPositionLabel;            // Text to display camera position

    private List<GameObject> gridObjects = new List<GameObject>(); // List to keep track of created grid objects

    void Start()
    {
        cameraYPositionSlider.value = 50;
        cameraPositionLabel.text = "Camera Height: " + mainCamera.transform.position.y.ToString("F2");
        // Add listener to the generate button
        generateButton.onClick.AddListener(Regen);
        cameraYPositionSlider.onValueChanged.AddListener(ChangeZoom);

        //init data
        rowsInputField.text = "5";
        columnsInputField.text = "5";
        rowSpacingInputField.text = "3";
        columnSpacingInputField.text = "3";


        // Initialize the grid
        GenerateGrid(false);


    }

    public void Regen()
    {
        GenerateGrid(true);
    }

    private void GenerateGrid(bool not_init)
    {
        if (rowsInputField.text.Length == 0 ||
            columnsInputField.text.Length == 0 ||
            rowSpacingInputField.text.Length == 0 ||
            columnsInputField.text.Length == 0)
        {
            Debug.LogError("Fill all Input Feilds");
            return;
        }

        // Clear existing grid
        ClearGrid();

        // Parse values from input fields
        int rows = not_init ? int.Parse(rowsInputField.text) : 5;
        int columns = not_init ? int.Parse(columnsInputField.text) : 5;
        float rowSpacing = not_init ? float.Parse(rowSpacingInputField.text) : 3;
        float columnSpacing = not_init ? float.Parse(columnSpacingInputField.text) : 3;

        
        // Calculate the starting position to center the grid
        Vector3 startPos = new Vector3(-((rows - 1) * rowSpacing) / 2, 0, -((columns - 1) * columnSpacing) / 2);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Choose a random prefab
                GameObject shapePrefab = shapePrefabs[Random.Range(0, shapePrefabs.Length)];

                // Instantiate the shape
                GameObject obj = Instantiate(shapePrefab, transform);

                // Set position relative to the center
                obj.transform.position = startPos + new Vector3(i * rowSpacing, 0, j * columnSpacing);

                // Set random scale
                float scale = Random.Range(0.4f, 1.5f);
                obj.transform.localScale = Vector3.one * scale;

                obj.transform.rotation = Quaternion.Euler(-90, 0, 0);


                // Set random color using MaterialManager
                Material colorMaterial = colorMaterials[Random.Range(0, colorMaterials.Length)];
                MaterialManager materialManager = obj.GetComponent<MaterialManager>();
                if (materialManager != null)
                {
                    materialManager.SetMaterial(colorMaterial);
                }

                // Add to list
                gridObjects.Add(obj);
            }
        }
    }

    public void ClearGrid()
    {
        foreach (GameObject obj in gridObjects)
        {
            Destroy(obj);
        }
        gridObjects.Clear();
    }


    public void ChangeZoom(float newV)
    {
        // Adjust the camera's y position based on the slider
        if (mainCamera != null)
        {
            Vector3 cameraPosition = mainCamera.transform.position;
            cameraPosition.y = cameraYPositionSlider.value;
            mainCamera.transform.position = cameraPosition;

            // Update the camera position label
            cameraPositionLabel.text = "Camera Height: " + cameraPosition.y.ToString("F2");
        }
    }
}

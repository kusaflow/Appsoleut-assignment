using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class SS_UIManager : MonoBehaviour
{
    public CinemachineVirtualCamera followCamera;           // Cinemachine camera for following the planets
    public List<CinemachineVirtualCamera> viewCameras;      // List of Cinemachine virtual cameras for predefined views
    public TMP_Dropdown cameraViewDropdown;                 // Dropdown for selecting camera views
    public Toggle orbitLinesToggle;                         // Toggle for orbit lines
    public TMP_Dropdown followPlanetDropdown;               // Dropdown for selecting a planet to follow
    public Slider sliderToChangeDistance; 

    public SolarSystemManager solarSystemManager;           // Reference to the SolarSystemManager script
    public Vector3 followOffset = new Vector3(0, 20, 0);    // Offset for the follow camera 

    private GameObject currentPlanet;                       // Currently followed planet
    private bool followingPlanet = false;

    void Start()
    {
        cameraViewDropdown.ClearOptions();
        List<string> cameraOptions = new List<string>();
        foreach (CinemachineVirtualCamera cam in viewCameras)
        {
            cameraOptions.Add(cam.name);
        }
        cameraViewDropdown.AddOptions(cameraOptions);

        // listeners
        cameraViewDropdown.onValueChanged.AddListener(OnCameraViewChanged);
        orbitLinesToggle.onValueChanged.AddListener(OnOrbitLinesToggled);
        followPlanetDropdown.onValueChanged.AddListener(OnFollowPlanetChanged);
        sliderToChangeDistance.onValueChanged.AddListener(ChangePlanetDistance);

        SetCameraPosition(0);

        StartCoroutine(InitPlanetList());
    }

    IEnumerator InitPlanetList()
    {
        yield return new WaitForSeconds(1.0f);

        followPlanetDropdown.ClearOptions();
        List<string> planetOptions = new List<string>();
        foreach (GameObject planet in solarSystemManager.planetInstances)
        {
            planetOptions.Add(planet.name);
        }
        followPlanetDropdown.AddOptions(planetOptions);
    }

    private void OnCameraViewChanged(int index)
    {
        if (index < viewCameras.Count)
        {
            SetCameraPosition(index);
            followingPlanet = false; // Stop following a planet if the view is changed
        }
    }

    private void OnOrbitLinesToggled(bool isEnabled)
    {
        foreach (GameObject g in solarSystemManager.lineRenderers)
            g.SetActive(isEnabled);
    }

    private void OnFollowPlanetChanged(int index)
    {
        if (index < solarSystemManager.planetInstances.Count)
        {
            currentPlanet = solarSystemManager.planetInstances[index];
            followingPlanet = true;
            ActivateFollowCamera();
        }
    }

    private void ChangePlanetDistance (float newv)
    {
        followOffset.x = newv;
    }
    void Update()
    {
        if (followingPlanet && currentPlanet != null)
        {
            FollowPlanet(currentPlanet);
        }
    }

    private void SetCameraPosition(int index)
    {
        DeactivateAllViewCameras();
        if (index < viewCameras.Count)
        {
            viewCameras[index].Priority = 100;
        }
    }

    private void DeactivateAllViewCameras()
    {
        foreach (var cam in viewCameras)
        {
            cam.Priority = 0; // Lower the priority to deactivate
        }
        followCamera.Priority = 10; // Default lower priority for follow camera
    }

    private void ActivateFollowCamera()
    {
        DeactivateAllViewCameras();
        followCamera.Priority = 100; // Give high priority to the follow camera
    }

    private void FollowPlanet(GameObject planet)
    {
        // Set the follow and look at target to the planet with the specified offset
        if (followCamera != null)
        {
            var composer = followCamera.GetCinemachineComponent<CinemachineTransposer>();
            composer.m_FollowOffset = followOffset;

            followCamera.LookAt = planet.transform;
            followCamera.Follow = planet.transform;
        }
    }
}

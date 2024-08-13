using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    public List<CinemachineVirtualCamera> virtualCameras; 
    public Button nextCameraButton;

    private int currentCameraIndex = 0;

    void Start()
    {
        SetCameraPriority(currentCameraIndex);

        // listener to button
        nextCameraButton.onClick.AddListener(SwitchToNextCamera);
    }

    private void SwitchToNextCamera()
    {
        // Set the current camera's priority to 0
        virtualCameras[currentCameraIndex].Priority = 0;

        // Move to the next camera in the list
        currentCameraIndex = (currentCameraIndex + 1) % virtualCameras.Count;

        // Set the next camera's priority to a higher value
        SetCameraPriority(currentCameraIndex);
    }

    private void SetCameraPriority(int index)
    {
        // Set all cameras to priority 0
        foreach (var cam in virtualCameras)
        {
            cam.Priority = 0;
        }

        // Set the selected camera's priority to a higher value
        virtualCameras[index].Priority = 10;
    }
}

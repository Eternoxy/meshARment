using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class PlacementInd : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;

    public GameObject[] tapePoints;

    int currentTapePoint = 0;

    private ARSessionOrigin arOrigin;
    ARRaycastManager aRRaycastManager;
    public Button plusButton;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool plusButtonPressed = false;

    public LineRenderer line;

    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        aRRaycastManager = GetComponent<ARRaycastManager>();
        plusButton.onClick.AddListener(OnPlusButtonClick);


    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (plusButtonPressed == true || currentTapePoint < 2)
        {
            PlacePoint(placementPose.position, currentTapePoint);
            currentTapePoint += 1;

            plusButtonPressed = false;
        }
        DrawLine();
    }



    public void PlacePoint(Vector3 pointPosition, int pointIndex)
    {
        tapePoints[pointIndex].SetActive(true);

        tapePoints[pointIndex].transform.position = pointPosition;

        if (currentTapePoint == 1)
        {
            DrawLine();
        }

        currentTapePoint += 1;
    }

    void DrawLine()
    {
        line.enabled = true;
        line.SetPosition(0, placementPose.position);
        if (currentTapePoint == 1)
        {
            line.SetPosition(1, placementPose.position);

        }
        else if (currentTapePoint == 2)
        {
            line.SetPosition(1, tapePoints[1].transform.position);

        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
    public void OnPlusButtonClick()
    {
        plusButtonPressed = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;


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
    float measurement = 0f;
    public TMP_Text distanceText;

    public TMP_Text floatingDistanceText;
    public GameObject floatingDistanceObject;

    Mode currentMode = Mode.d;

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
        UpdateMeasurement();
        PlaceFloatingText();

        if (plusButtonPressed == true && currentTapePoint < 2)
        {
            PlacePoint(placementPose.position, currentTapePoint);
            plusButtonPressed = false;
        }
        if (currentTapePoint == 1)
        {
            DrawLine();
        }

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
        line.SetPosition(0, tapePoints[0].transform.position);
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
    void UpdateMeasurement()
    {
        switch (currentMode)
        {
            case Mode.d:
                if (currentTapePoint == 0)
                {
                    measurement = 0f;
                }
                else if (currentTapePoint == 1)
                {
                    measurement = Vector3.Distance(tapePoints[0].transform.position, placementPose.position);
                }
                else if (currentTapePoint == 2)
                {
                    measurement = Vector3.Distance(tapePoints[0].transform.position, tapePoints[1].transform.position);
                }
                break;
            case Mode.i:
                if (currentTapePoint == 0)
                {
                    measurement = 0f;
                }
                else if (currentTapePoint == 1)
                {
                    measurement = Vector3.Angle(tapePoints[0].transform.position, placementPose.position);
                }
                else if (currentTapePoint == 2)
                {
                    measurement = Vector3.Angle(tapePoints[0].transform.position, tapePoints[1].transform.position);
                }
                break;


        }
        string distanceStr = measurement.ToString("#.##") + currentMode;

        distanceText.text = distanceStr;
        floatingDistanceText.text = distanceStr;


    }
    void PlaceFloatingText()
    {
        if (currentTapePoint == 0)
        {
            floatingDistanceObject.SetActive(false);
        }
        else if (currentTapePoint == 1)
        {
            floatingDistanceObject.SetActive(true);
            floatingDistanceObject.transform.position = Vector3.Lerp(tapePoints[0].transform.position, placementPose.position, 0.5f);
        }
        else if (currentTapePoint == 2)
        {
            floatingDistanceObject.SetActive(true);
            floatingDistanceObject.transform.position = Vector3.Lerp(tapePoints[0].transform.position, tapePoints[1].transform.position, 0.5f);
        }

        floatingDistanceObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);

    }

    public void OnPlusButtonClick()
    {
        plusButtonPressed = true;
    }

    public void ChangeMode(string mode)
    {
        currentMode = (Mode)System.Enum.Parse(typeof(Mode), mode);
    }
}

public enum Mode
{
    d,
    i
}
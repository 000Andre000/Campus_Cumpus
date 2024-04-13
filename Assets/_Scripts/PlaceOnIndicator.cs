using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.IO;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnIndicator : MonoBehaviour
{
    [SerializeField]
    GameObject placementIndicator;
    [SerializeField]
    GameObject placedPrefab;

    GameObject spawnedObject;

    [SerializeField]
    InputAction touchInput;

    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Structure to hold position and object name
    public struct PlacedObjectData
    {
        public string objectName;
        public Vector3 position;
        public Quaternion rotation;
    }

    // List to store the positions and names of objects
    public List<PlacedObjectData> placedObjectDataList = new List<PlacedObjectData>();

    // Reference to AR Session Origin
    public ARSession arSessionOrigin;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();

        touchInput.performed += _ => { PlaceObject(); };

        placementIndicator.SetActive(false);
    }

    private void OnEnable()
    {
        touchInput.Enable();
    }

    private void OnDisable()
    {
        touchInput.Disable();
    }

    private void Update()
    {
        if (aRRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            placementIndicator.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);

            if (!placementIndicator.activeInHierarchy)
                placementIndicator.SetActive(true);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    public void PlaceObject()
    {
        if (!placementIndicator.activeInHierarchy)
            return;

        // Place the object at the indicator position and rotation
        spawnedObject = Instantiate(placedPrefab, placementIndicator.transform.position, placementIndicator.transform.rotation);

        // Make the spawned object a child of the AR Session Origin
        spawnedObject.transform.parent = arSessionOrigin.transform;

        // Save object data
        SaveObjectData(spawnedObject.transform.localPosition, spawnedObject.transform.localRotation);

        // Write data to CSV
        WriteDataToCSV();
    }

    void SaveObjectData(Vector3 position, Quaternion rotation)
    {
        // Create default name (a number)
        string objectName = (placedObjectDataList.Count + 1).ToString();

        // Add object name and position to the list
        PlacedObjectData placedObjectData = new PlacedObjectData();
        placedObjectData.objectName = objectName;
        placedObjectData.position = position;
        placedObjectData.rotation = rotation;
        placedObjectDataList.Add(placedObjectData);
    }

    void WriteDataToCSV()
    {
        string assetsPath = Application.dataPath;
        string filePath = Path.Combine(Application.persistentDataPath, "placedObjectData.csv");

        // Create or append to the CSV file
        using (StreamWriter sw = File.CreateText(filePath))
        {
            foreach (PlacedObjectData data in placedObjectDataList)
            {
                string line = $"{data.objectName},{data.position.x},{data.position.y},{data.position.z},{data.rotation.x},{data.rotation.y},{data.rotation.z}";
                sw.WriteLine(line);
            }
        }

        Debug.Log("Data written to CSV: " + filePath);
    }
}

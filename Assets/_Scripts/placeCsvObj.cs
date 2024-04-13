using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using System.IO;

[HelpURL("https://youtu.be/HkNVp04GOEI")]
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceCsvObj : MonoBehaviour
{
    [SerializeField]
    GameObject placedPrefab;

    GameObject spawnedObject;

    TouchControls controls;

    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public List<GameObject> placedGameObjects = new List<GameObject>();

    void Start()
    {
        // Check for audio listeners in the scene
        CheckAudioListener();
        string assetsPath = Application.dataPath;
        string filePath = Path.Combine(Application.persistentDataPath, "placedObjectData.csv");
        ReadCSVAndPlaceObjects(filePath);
        // Read CSV file and place objects accordingly

    }

    void CheckAudioListener()
    {
        // Check if there is more than one audio listener in the scene
        if (FindObjectsOfType<AudioListener>().Length > 1)
        {
            // If there is more than one, destroy the excess audio listeners
            AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
            for (int i = 1; i < audioListeners.Length; i++)
            {
                Destroy(audioListeners[i]);
            }
        }
    }

    void ReadCSVAndPlaceObjects(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            string[] values = line.Split(',');

            // Extract position data
            float xp = float.Parse(values[1]);
            float yp = float.Parse(values[2]);
            float zp = float.Parse(values[3]);
            float xr = float.Parse(values[4]);
            float yr = float.Parse(values[5]);
            float zr = float.Parse(values[6]);


            // Place object at extracted position
            Vector3 position = new Vector3(xp, yp, zp);
            Quaternion rotation = Quaternion.Euler(xr, yr, zr);

            PlaceObject(position,rotation);
        }
    }

    void PlaceObject(Vector3 position , Quaternion rotation)
    {
        // Check if the position is on a detected trackable
        //if (aRRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
        //{
        //    // Raycast hits are sorted by distance, so the first hit means the closest.
        //    var hitPose = hits[0].pose;

            // Instantiate the prefab
            spawnedObject = Instantiate(placedPrefab,position,rotation);
            GameObject newObj = spawnedObject;
            placedGameObjects.Add(newObj);
           
        

        // To make the spawned object always look at the camera. Delete if not needed.

        //}
    }
}

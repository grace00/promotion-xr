// using UnityEngine;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;
// using System.Collections.Generic;

// public class PlacementIndicatorController : MonoBehaviour
// {
//     private ARRaycastManager raycastManager;
//     private GameObject visual;
//     private List<ARRaycastHit> hits = new List<ARRaycastHit>();

//     void Start()
//     {
//         raycastManager = FindObjectOfType<ARRaycastManager>();
//         visual = transform.GetChild(0).gameObject;
//         visual.SetActive(false);
//     }

//     void Update()
//     {
//         if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
//         {
//             transform.position = hits[0].pose.position;
//             transform.rotation = hits[0].pose.rotation;

//             if (!visual.activeInHierarchy)
//             {
//                 visual.SetActive(true);
//             }
//         }
//     }
// }

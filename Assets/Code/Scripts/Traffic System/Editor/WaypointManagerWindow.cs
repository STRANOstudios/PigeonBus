using UnityEditor;
using UnityEngine;

namespace TrafficSystem
{
    public class WaypointManagerWindow : EditorWindow
    {
        [MenuItem("Tools/Traffic System/Waypoint Manager")]
        public static void ShowWindow()
        {
            GetWindow<WaypointManagerWindow>("Waypoint Manager");
        }

        private Transform waypointRoot;

        private SerializedObject obj;

        private void OnEnable()
        {
            // Create a new SerializedObject to update the waypointRoot field.
            obj = new SerializedObject(this);
        }

        private void OnGUI()
        {
            // Update the SerializedObject to reflect any changes to the waypointRoot field.
            obj.Update();

            // Draw an ObjectField to select the waypointRoot transform.
            EditorGUILayout.ObjectField(obj.FindProperty("waypointRoot"));

            // If no waypointRoot is selected, show an error message.
            if (waypointRoot == null)
            {
                EditorGUILayout.HelpBox("root transform must be set", MessageType.Error);
            }
            else
            {
                // Draw a vertical box containing the waypoint management buttons.
                EditorGUILayout.BeginVertical("box");
                DrawButtons();
                EditorGUILayout.EndVertical();
            }

            // Apply any changes to the SerializedObject.
            obj.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the waypoint management buttons.
        /// </summary>
        private void DrawButtons()
        {
            // If a waypoint is currently selected, show buttons to add a branch waypoint, create a waypoint before or after the selected waypoint, and remove the selected waypoint.
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>() is Waypoint selectedWaypoint)
            {
                if (GUILayout.Button("Add Branch Waypoint"))
                {
                    CreateBranch(selectedWaypoint);
                }
                if (GUILayout.Button("Create Waypoint Before"))
                {
                    CreateWaypointBefore(selectedWaypoint);
                }
                if (GUILayout.Button("Create Waypoint After"))
                {
                    CreateWaypointAfter(selectedWaypoint);
                }
                if (GUILayout.Button("Remove Waypoint"))
                {
                    RemoveWaypoint(selectedWaypoint);
                }
            }

            // Show buttons to create a new waypoint and create an intersection.
            if (GUILayout.Button("Create Waypoint"))
            {
                CreateWaypoint();
            }
            if (GUILayout.Button("Create Intersection"))
            {
                CreateIntersection();
            }
        }

        /// <summary>
        /// Creates a new waypoint at the end of the waypoint system.
        /// </summary>
        private void CreateWaypoint()
        {
            // Create a new GameObject to hold the waypoint.
            GameObject waypointObject = new GameObject("Waypoint" + waypointRoot.childCount);
            waypointObject.AddComponent<Waypoint>();
            waypointObject.transform.SetParent(waypointRoot, false);

            // Get the Waypoint component of the new waypoint.
            Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

            // If this is not the first waypoint, set its previous waypoint to the last waypoint in the system.
            if (waypointRoot.childCount > 1)
            {
                waypoint.prevWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
                waypoint.prevWaypoint.nextWaypoint = waypoint;

                // Set the position and forward direction of the new waypoint to match its previous waypoint.
                waypoint.transform.position = waypoint.prevWaypoint.transform.position;
                waypoint.transform.forward = waypoint.prevWaypoint.transform.forward;
            }

            // Select the new waypoint.
            Selection.activeGameObject = waypoint.gameObject;
        }

        /// <summary>
        /// Creates a new waypoint before the specified waypoint.
        /// </summary>
        /// <param name="selectedWaypoint">The waypoint before which to create the new waypoint.</param>
        private void CreateWaypointBefore(Waypoint selectedWaypoint)
        {
            // Create a new GameObject to hold the waypoint.
            GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount);
            waypointObject.AddComponent<Waypoint>();
            waypointObject.transform.SetParent(waypointRoot, false);

            // Get the Waypoint component of the new waypoint.
            Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

            // Set the position and forward direction of the new waypoint to match the selected waypoint.
            waypointObject.transform.position = selectedWaypoint.transform.position;
            waypointObject.transform.forward = selectedWaypoint.transform.forward;

            // If the selected waypoint has a previous waypoint, set the new waypoint's previous waypoint to the selected waypoint's previous waypoint.
            if (selectedWaypoint.prevWaypoint != null)
            {
                newWaypoint.prevWaypoint = selectedWaypoint.prevWaypoint;
                selectedWaypoint.prevWaypoint.nextWaypoint = newWaypoint;
            }

            // Set the new waypoint's next waypoint to the selected waypoint.
            newWaypoint.nextWaypoint = selectedWaypoint;

            // Set the selected waypoint's previous waypoint to the new waypoint.
            selectedWaypoint.prevWaypoint = newWaypoint;

            // Set the sibling index of the new waypoint to match the selected waypoint.
            newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

            // Select the new waypoint.
            Selection.activeGameObject = newWaypoint.gameObject;
        }

        /// <summary>
        /// Creates a new waypoint after the specified waypoint.
        /// </summary>
        /// <param name="selectedWaypoint">The waypoint after which to create the new waypoint.</param>
        private void CreateWaypointAfter(Waypoint selectedWaypoint)
        {
            // Create a new GameObject to hold the waypoint.
            GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount);
            waypointObject.AddComponent<Waypoint>();
            waypointObject.transform.SetParent(waypointRoot, false);

            // Get the Waypoint component of the new waypoint.
            Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

            // Set the position and forward direction of the new waypoint to match the selected waypoint.
            waypointObject.transform.position = selectedWaypoint.transform.position;
            waypointObject.transform.forward = selectedWaypoint.transform.forward;

            // Set the new waypoint's previous waypoint to the selected waypoint.
            newWaypoint.prevWaypoint = selectedWaypoint;

            // If the selected waypoint has a next waypoint, set the new waypoint's next waypoint to the selected waypoint's next waypoint.
            if (selectedWaypoint.nextWaypoint != null)
            {
                selectedWaypoint.nextWaypoint.prevWaypoint = newWaypoint;
                newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            }

            // Set the selected waypoint's next waypoint to the new waypoint.
            selectedWaypoint.nextWaypoint = newWaypoint;

            // Set the sibling index of the new waypoint to match the selected waypoint.
            newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

            // Select the new waypoint.
            Selection.activeGameObject = newWaypoint.gameObject;
        }

        /// <summary>
        /// Creates a new intersection waypoint.
        /// </summary>
        private void CreateIntersection()
        {
            // Create a new GameObject to hold the intersection waypoint.
            GameObject waypointObject = new GameObject("Intersection " + waypointRoot.childCount);
            waypointObject.AddComponent<WaypointIntersection>();
            waypointObject.transform.SetParent(waypointRoot, false);

            // Get the Waypoint component of the new intersection waypoint.
            Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

            // Get the currently selected waypoint.
            Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

            // Set the position and forward direction of the new intersection waypoint to match the selected waypoint.
            waypointObject.transform.position = selectedWaypoint.transform.position;
            waypointObject.transform.forward = selectedWaypoint.transform.forward;

            // Set the new intersection waypoint's previous waypoint to the selected waypoint.
            newWaypoint.prevWaypoint = selectedWaypoint;

            // If the selected waypoint has a next waypoint, set the new intersection waypoint's next waypoint to the selected waypoint's next waypoint.
            if (selectedWaypoint.nextWaypoint != null)
            {
                selectedWaypoint.nextWaypoint.prevWaypoint = newWaypoint;
                newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            }

            // Set the selected waypoint's next waypoint to the new intersection waypoint.
            selectedWaypoint.nextWaypoint = newWaypoint;

            // Set the sibling index of the new intersection waypoint to match the selected waypoint.
            newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

            // Select the new intersection waypoint.
            Selection.activeGameObject = newWaypoint.gameObject;
        }

        /// <summary>
        /// Creates a new branch waypoint from the specified waypoint.
        /// </summary>
        /// <param name="selectedWaypoint">The waypoint from which to create the new branch waypoint.</param>
        private void CreateBranch(Waypoint selectedWaypoint)
        {
            // Create a new GameObject to hold the branch waypoint.
            GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount);
            waypointObject.AddComponent<Waypoint>();
            waypointObject.transform.SetParent(waypointRoot, false);

            // Get the Waypoint component of the new branch waypoint.
            Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

            // Add the new branch waypoint to the selected waypoint's branches.
            selectedWaypoint.branches.Add(waypoint);

            // Set the position and forward direction of the new branch waypoint to match the selected waypoint.
            waypoint.transform.position = selectedWaypoint.transform.position;
            waypoint.transform.forward = selectedWaypoint.transform.forward;

            // Select the new branch waypoint.
            Selection.activeGameObject = waypoint.gameObject;
        }

        /// <summary>
        /// Removes the specified waypoint from the waypoint system.
        /// </summary>
        /// <param name="selectedWaypoint">The waypoint to remove.</param>
        private void RemoveWaypoint(Waypoint selectedWaypoint)
        {
            // If the selected waypoint has a next waypoint, set its previous waypoint to the selected waypoint's previous waypoint.
            if (selectedWaypoint.nextWaypoint != null)
            {
                selectedWaypoint.nextWaypoint.prevWaypoint = selectedWaypoint.prevWaypoint;
            }

            // If the selected waypoint has a previous waypoint, set its next waypoint to the selected waypoint's next waypoint.
            if (selectedWaypoint.prevWaypoint != null)
            {
                selectedWaypoint.prevWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
                Selection.activeGameObject = selectedWaypoint.prevWaypoint.gameObject;
            }

            // Destroy the selected waypoint's GameObject.
            DestroyImmediate(selectedWaypoint.gameObject);
        }
    }
}
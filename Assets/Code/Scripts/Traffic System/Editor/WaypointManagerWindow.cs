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

        public Transform waypointRoot;

        private void OnGUI()
        {
            SerializedObject obj = new(this);

            EditorGUILayout.ObjectField(obj.FindProperty("waypointRoot"));

            if (waypointRoot == null)
            {
                EditorGUILayout.HelpBox("root transform must be set", MessageType.Error);
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
                DrawButtons();
                EditorGUILayout.EndVertical();
            }

            obj.ApplyModifiedProperties();
        }

        private void DrawButtons()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
            {
                if (GUILayout.Button("Create Waypoint Before"))
                {
                    CreateWaypointBefore();
                }
                if (GUILayout.Button("Create Waypoint After"))
                {
                    CreateWaypointAfter();
                }
                if (GUILayout.Button("Remove Waypoint"))
                {
                    RemoveWaypoint();
                }
            }
            if (GUILayout.Button("Create Waypoint"))
            {
                CreateWaypoint();
            }
            if(GUILayout.Button("Create Intersection"))
            {
                CreateIntersection();
            }
        }

        void CreateWaypoint()
        {
            GameObject waypointObject = new("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
            waypointObject.transform.SetParent(waypointRoot, false);

            Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

            if (waypointRoot.childCount > 1)
            {
                waypoint.prevWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
                waypoint.prevWaypoint.nextWaypoint = waypoint;

                waypoint.transform.position = waypoint.prevWaypoint.transform.position;
                waypoint.transform.forward = waypoint.prevWaypoint.transform.forward;
            }

            Selection.activeGameObject = waypoint.gameObject;
        }

        void CreateWaypointBefore()
        {
            GameObject waypointObject = new("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
            waypointObject.transform.SetParent(waypointRoot, false);

            Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

            Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

            waypointObject.transform.position = selectedWaypoint.transform.position;
            waypointObject.transform.forward = selectedWaypoint.transform.forward;

            if (selectedWaypoint.prevWaypoint != null)
            {
                newWaypoint.prevWaypoint = selectedWaypoint.prevWaypoint;
                selectedWaypoint.prevWaypoint.nextWaypoint = newWaypoint;
            }

            newWaypoint.nextWaypoint = selectedWaypoint;

            selectedWaypoint.prevWaypoint = newWaypoint;

            newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

            Selection.activeGameObject = newWaypoint.gameObject;
        }

        void CreateWaypointAfter()
        {
            GameObject waypointObject = new("Waypoint" + waypointRoot.childCount, typeof(Waypoint));
            waypointObject.transform.SetParent(waypointRoot, false);

            Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

            Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

            waypointObject.transform.position = selectedWaypoint.transform.position;
            waypointObject.transform.forward = selectedWaypoint.transform.forward;

            newWaypoint.prevWaypoint = selectedWaypoint;

            if(selectedWaypoint.nextWaypoint != null)
            {
                selectedWaypoint.nextWaypoint.prevWaypoint = newWaypoint;
                newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            }

            selectedWaypoint.nextWaypoint = newWaypoint;

            newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

            Selection.activeGameObject = newWaypoint.gameObject;
        }

        void CreateIntersection()
        {
            GameObject waypointObject = new("Waypoint" + waypointRoot.childCount, typeof(WaypointIntersection));
            waypointObject.transform.SetParent(waypointRoot, false);

            Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();

            Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

            waypointObject.transform.position = selectedWaypoint.transform.position;
            waypointObject.transform.forward = selectedWaypoint.transform.forward;

            newWaypoint.prevWaypoint = selectedWaypoint;

            if (selectedWaypoint.nextWaypoint != null)
            {
                selectedWaypoint.nextWaypoint.prevWaypoint = newWaypoint;
                newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            }

            selectedWaypoint.nextWaypoint = newWaypoint;

            newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());

            Selection.activeGameObject = newWaypoint.gameObject;
        }

        void RemoveWaypoint()
        {
            Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

            if (selectedWaypoint.nextWaypoint != null)
            {
                selectedWaypoint.nextWaypoint.prevWaypoint = selectedWaypoint.prevWaypoint;
            }

            if (selectedWaypoint.prevWaypoint != null)
            {
                selectedWaypoint.prevWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
                Selection.activeGameObject = selectedWaypoint.prevWaypoint.gameObject;
            }

            DestroyImmediate(selectedWaypoint.gameObject);
        }
    }
}

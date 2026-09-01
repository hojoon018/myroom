using UnityEditor;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    [CustomEditor(typeof(ProceduralRoomRandomizer))]
    public class ProceduralRoomRandomizerEditor : Editor
    {
        private SerializedProperty roomWidth, roomLength;
        private SerializedProperty layoutSeed;
        private SerializedProperty generateWallsAndFloor, wallHeight, wallThickness;
        private SerializedProperty availableThemes;
        private SerializedProperty bedPrefab, tablePrefab, chairPrefab, shelfPrefab, lampPrefab;
        private SerializedProperty roomContainer;

        private void OnEnable()
        {
            roomWidth = serializedObject.FindProperty("roomWidth");
            roomLength = serializedObject.FindProperty("roomLength");
            layoutSeed = serializedObject.FindProperty("layoutSeed");
            generateWallsAndFloor = serializedObject.FindProperty("generateWallsAndFloor");
            wallHeight = serializedObject.FindProperty("wallHeight");
            wallThickness = serializedObject.FindProperty("wallThickness");
            availableThemes = serializedObject.FindProperty("availableThemes");
            bedPrefab = serializedObject.FindProperty("bedPrefab");
            tablePrefab = serializedObject.FindProperty("tablePrefab");
            chairPrefab = serializedObject.FindProperty("chairPrefab");
            shelfPrefab = serializedObject.FindProperty("shelfPrefab");
            lampPrefab = serializedObject.FindProperty("lampPrefab");
            roomContainer = serializedObject.FindProperty("roomContainer");
        }

        public override void OnInspectorGUI()
        {
            ProceduralRoomRandomizer script = (ProceduralRoomRandomizer)target;
            serializedObject.Update();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Procedural Room Generation", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Shuffle the seed to preview different footprints in the Scene View, then click Generate to build the models.", MessageType.Info);
            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(roomWidth);
            EditorGUILayout.PropertyField(roomLength);
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(generateWallsAndFloor);
            if (generateWallsAndFloor.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(wallHeight);
                EditorGUILayout.PropertyField(wallThickness);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(availableThemes);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(bedPrefab);
            EditorGUILayout.PropertyField(tablePrefab);
            EditorGUILayout.PropertyField(chairPrefab);
            EditorGUILayout.PropertyField(shelfPrefab);
            EditorGUILayout.PropertyField(lampPrefab);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(roomContainer);
            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(layoutSeed);
            if (GUILayout.Button("SHUFFLE", GUILayout.Width(80)))
            {
                layoutSeed.intValue = Random.Range(0, 999999);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            GUI.backgroundColor = new Color(0.2f, 0.6f, 1.0f);
            if (GUILayout.Button("GENERATE RANDOM BEDROOM", GUILayout.Height(50)))
            {
                serializedObject.ApplyModifiedProperties();
                Undo.RecordObject(script, "Generate Random Room");
                script.GenerateRandomBedroom();
                EditorUtility.SetDirty(script);
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}

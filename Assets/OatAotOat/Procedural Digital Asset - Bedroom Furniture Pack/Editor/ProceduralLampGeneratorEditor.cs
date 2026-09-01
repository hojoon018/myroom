using UnityEditor;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    [CustomEditor(typeof(ProceduralLampGenerator))]
    public class ProceduralLampGeneratorEditor : ProceduralGeneratorEditor
    {
        private int currentTab = 0;
        private int settingsTab = 0;
        private readonly string[] settingsTabNames = { "Base & Frame", "Shade & Extras" };
        private bool showReferences = false;
        private readonly string[] tabNames = { "Main Settings", "Style" };

        public override void OnInspectorGUI()
        {
            ProceduralLampGenerator script = (ProceduralLampGenerator)target;
            serializedObject.Update();

            showReferences = EditorGUILayout.Foldout(showReferences, "Model References", true, EditorStyles.foldoutHeader);
            if (showReferences)
            {
                ShowReferencesFoldout();
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lampStyle"));
            EditorGUILayout.Space();

            currentTab = GUILayout.Toolbar(currentTab, tabNames, GUILayout.Height(30));
            EditorGUILayout.Space(10);

            switch (currentTab)
            {
                case 0: ShowSettingsSection(script); break;
                case 1: DrawAppearanceSection(script, "Frame Color", "Shade Color"); break;
            }

            DrawBakingSection(script, "Lamps");
            serializedObject.ApplyModifiedProperties();
        }

        #region --- GUI DRAWERS ---
        void ShowReferencesFoldout()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lampBase"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pillar"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lowerArm"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("upperArm"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("joint1"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("joint2"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("joint3"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shade"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bulb"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseRoundMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("basePillarMesh"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseDrumShadeMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseDomeShadeMesh"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseJointMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseHalfJointMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseBulbMesh"));
            EditorGUI.indentLevel--;
        }

        void ShowSettingsSection(ProceduralLampGenerator script)
        {
            settingsTab = GUILayout.Toolbar(settingsTab, settingsTabNames, GUILayout.Height(25));
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical("box");
            switch (settingsTab)
            {
                case 0:
                    if (script.lampStyle == LampStyle.Polygon)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("polygonSides"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("polygonRoundness"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("roundArcSegments"));
                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("baseRadius"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("baseThickness"));
                    EditorGUILayout.Space();

                    if (script.lampStyle == LampStyle.Desk)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("lowerArmLength"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("upperArmLength"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("armThickness"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("lowerArmAngle"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("upperArmAngle"));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("lampHeight"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("pillarThickness"));
                    }
                    break;

                case 1:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("shadeHeight"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("shadeBottomRadius"));

                    if (script.lampStyle == LampStyle.Polygon)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("polygonShadeThickness"));
                    }

                    if (script.lampStyle != LampStyle.Desk)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shadeTopRadius"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shadeOffset"));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("shadeAngle"));
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hasBulb"));
                    if (script.hasBulb)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("bulbSize"));
                        EditorGUI.indentLevel--;
                    }
                    break;
            }
            EditorGUILayout.EndVertical();
        }
        #endregion
    }
}

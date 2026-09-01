using UnityEditor;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    [CustomEditor(typeof(ProceduralBedGenerator))]
    public class ProceduralBedGeneratorEditor : ProceduralGeneratorEditor
    {
        private int currentTab = 0;

        private int settingsTab = 0;
        private readonly string[] settingsTabNames = { "Base & Frame", "Boards", "Mattress & Pillows" };

        private bool showReferences = false;
        private readonly string[] tabNames = { "Main Settings", "Style" };

        public override void OnInspectorGUI()
        {
            ProceduralBedGenerator script = (ProceduralBedGenerator)target;
            serializedObject.Update();

            showReferences = EditorGUILayout.Foldout(showReferences, "Model References", true, EditorStyles.foldoutHeader);
            if (showReferences)
            {
                ShowReferencesFoldout();
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bedType"));
            EditorGUILayout.Space();

            currentTab = GUILayout.Toolbar(currentTab, tabNames, GUILayout.Height(30));
            EditorGUILayout.Space(10);

            switch (currentTab)
            {
                case 0: ShowSettingsSection(script); break;
                case 1: DrawAppearanceSection(script, "Frame Color", "Bedding Color"); break;
            }

            DrawBakingSection(script, "Beds");
            serializedObject.ApplyModifiedProperties();
        }

        #region --- GUI DRAWERS ---
        void ShowReferencesFoldout()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frontLeftLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frontRightLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backLeftLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backRightLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("platformBase"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("frameLeft"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frameRight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frameFront"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frameBack"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardBottomRail"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardCushion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftHeadboardPost"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightHeadboardPost"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardSlatPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardSlatContainer"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardCushion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftFootboardPost"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightFootboardPost"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardSlatPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardSlatContainer"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("mattress"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftPillow"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightPillow"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseLegMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("basePlatformMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseFrameMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseBoardMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("basePostMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseMattressMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("basePillowMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseCushionMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseSlatMesh"));
            EditorGUI.indentLevel--;
        }

        void ShowSettingsSection(ProceduralBedGenerator script)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bedWidth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bedDepth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bedClearanceHeight"));
            EditorGUILayout.Space();

            EditorGUILayout.Space(5);
            settingsTab = GUILayout.Toolbar(settingsTab, settingsTabNames, GUILayout.Height(25));
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical("box");
            switch (settingsTab)
            {
                case 0: ShowFrameAndBaseSection(script); break;
                case 1: ShowBoardsSection(script); break;
                case 2: ShowMattressSection(script); break;
            }
            EditorGUILayout.EndVertical();
        }

        void ShowFrameAndBaseSection(ProceduralBedGenerator script)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frameHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frameThickness"));
            EditorGUILayout.Space();

            if (script.bedType == BedType.FourLeg)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("legThickness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("legEndThickness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("legSplayAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("legInset"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("platformInset"));
            }
        }

        void ShowBoardsSection(ProceduralBedGenerator script)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasHeadboard"));
            if (script.hasHeadboard)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardThickness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("laybackAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardLift"));

                if (script.headboardType == BoardType.Slat)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardRailHeight"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardSlatSpacing"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardSlatThickness"));
                }
                else if (script.headboardType == BoardType.Cushion)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardCushionMargin"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("headboardCushionProjection"));
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasFootboard"));
            if (script.hasFootboard)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardThickness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardLaybackAngle"));

                if (script.footboardType == BoardType.Slat)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardTopRailHeight"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardSlatSpacing"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardSlatThickness"));
                }
                else if (script.footboardType == BoardType.Cushion)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardCushionMargin"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("footboardCushionProjection"));
                }
                EditorGUI.indentLevel--;
            }
        }

        void ShowMattressSection(ProceduralBedGenerator script)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mattressThickness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mattressInset"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasPillows"));
            if (script.hasPillows)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pillowWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pillowDepth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pillowThickness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pillowOffset"));
                EditorGUI.indentLevel--;
            }
        }
        #endregion
    }
}
using UnityEditor;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    [CustomEditor(typeof(ProceduralChairGenerator))]
    public class ProceduralChairGeneratorEditor : ProceduralGeneratorEditor
    {
        private int currentTab = 0;
        private bool showReferences = false;
        private readonly string[] tabNames = { "Main Settings", "Armrests", "Backrests", "Style" };

        public override void OnInspectorGUI()
        {
            ProceduralChairGenerator script = (ProceduralChairGenerator)target;
            serializedObject.Update();

            showReferences = EditorGUILayout.Foldout(showReferences, "Model References", true, EditorStyles.foldoutHeader);
            if (showReferences) { ShowReferencesFoldout(); }

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("chairType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("chairShape"));
            EditorGUILayout.Space();

            currentTab = GUILayout.Toolbar(currentTab, tabNames, GUILayout.Height(30));
            EditorGUILayout.Space(10);

            switch (currentTab)
            {
                case 0:
                    ShowMainSettingsSection(script);
                    break;
                case 1:
                    ShowArmrestsSection();
                    break;
                case 2:
                    ShowBackrestsSection(script);
                    break;
                case 3:
                    DrawAppearanceSection(script, "Frame Color", "Cushion Color");
                    break;
            }

            DrawBakingSection(script, "Chairs");
            serializedObject.ApplyModifiedProperties();
        }

        #region --- GUI DRAWERS ---
        void ShowReferencesFoldout()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("seat"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frontLeftLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frontRightLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backLeftLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backRightLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftRocker"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightRocker"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pedestalPillar"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pedestalBase"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("boatSeat"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backrest"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftSeatFrame"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightSeatFrame"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backSeatFrame"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftBackrestFrame"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightBackrestFrame"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("topBackrestFrame"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftArmrest"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightArmrest"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftArmrestSupport"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightArmrestSupport"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftArmrestSupportBack"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightArmrestSupportBack"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftArmrestCushion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightArmrestCushion"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("spindlePrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spindleContainer"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseLegMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("basePedestalMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseBoatSeatMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseBackrestMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseSideFrameMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseSpindleMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseRoundSeatMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseRectangularSeatMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseRockerMesh"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        void ShowMainSettingsSection(ProceduralChairGenerator script)
        {
            if (script.chairShape == ChairShape.Rectangular)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("seatWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("seatDepth"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("seatDiameter"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("seatHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("legHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frameThickness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cushionThickness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frameInset"));
            EditorGUILayout.Space();

            if (script.chairType == ChairType.FourLeg)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("legEndThickness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("frontLegOffsetAngle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("backLegOffsetAngle"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("hasRockers"));
                if (script.hasRockers)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("extraRockerLength"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("rockerHeight"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("rockerThickness"));
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pillarThickness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("baseRadius"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("baseThickness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("hasBoatSeat"));

                if (script.hasBoatSeat)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("boatSeatRadius"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("boatSeatThickness"));
                    EditorGUI.indentLevel--;
                }
            }
        }

        void ShowArmrestsSection()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasArmrests"));

            if (serializedObject.FindProperty("hasArmrests").boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("armrestHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("armrestWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("armrestDepth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("armrestThickness"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("hasArmrestSupports"));
                if (serializedObject.FindProperty("hasArmrestSupports").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("doubleSupportThreshold"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("hasArmrestCushions"));
                if (serializedObject.FindProperty("hasArmrestCushions").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("armCushionThickness"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("armCushionWidthOffset"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("armCushionDepthOffset"));
                    EditorGUI.indentLevel--;
                }
            }
        }

        void ShowBackrestsSection(ProceduralChairGenerator script)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backrestType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backrestHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backrestDepth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("laybackAngle"));
            if (script.backrestType == BackrestType.Spindles)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spindleSpacing"));
            }
        }
        #endregion
    }
}

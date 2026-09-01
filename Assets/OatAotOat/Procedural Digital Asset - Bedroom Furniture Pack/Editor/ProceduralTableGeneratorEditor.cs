using UnityEditor;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    [CustomEditor(typeof(ProceduralTableGenerator))]
    public class ProceduralTableGeneratorEditor : ProceduralGeneratorEditor
    {
        private int currentTab = 0;

        private int fourLegTab = 0;
        private readonly string[] fourLegTabNames = { "Legs", "Skirts", "Drawers", "Stretchers" };

        private bool showReferences = false;
        private readonly string[] tabNames = { "Main Settings", "Style" };

        public override void OnInspectorGUI()
        {
            ProceduralTableGenerator script = (ProceduralTableGenerator)target;
            serializedObject.Update();

            showReferences = EditorGUILayout.Foldout(showReferences, "Model References", true, EditorStyles.foldoutHeader);
            if (showReferences)
            {
                ShowReferencesFoldout();
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("tableType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tableShape"));
            EditorGUILayout.Space();

            currentTab = GUILayout.Toolbar(currentTab, tabNames, GUILayout.Height(30));
            EditorGUILayout.Space(10);

            switch (currentTab)
            {
                case 0: ShowDimensionsSection(script); break;
                case 1: DrawAppearanceSection(script, "Surface Color", "Leg Color"); break;
            }

            DrawBakingSection(script, "Tables");
            serializedObject.ApplyModifiedProperties();
        }

        #region --- GUI DRAWERS ---
        void ShowReferencesFoldout()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tableTop"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frontLeftLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frontRightLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backLeftLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backRightLeg"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pedestalPillar"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pedestalBase"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("frontSkirt"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backSkirt"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftSkirt"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightSkirt"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("drawerPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("drawerContainer"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("stretcherLeft"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stretcherRight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stretcherFront"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stretcherBack"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stretcherCross1"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stretcherCross2"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("footrestBoard"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseLegMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("basePedestalMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseStretcherMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseRectangularTopMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseRoundTopMesh"));
            EditorGUI.indentLevel--;
        }

        void ShowDimensionsSection(ProceduralTableGenerator script)
        {
            if (script.tableShape == TableShape.Rectangular)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("tableWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("tableDepth"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("tableDiameter"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("tableHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("topThickness"));
            EditorGUILayout.Space();

            if (script.tableType == TableType.FourLeg)
            {
                EditorGUILayout.Space(5);
                fourLegTab = GUILayout.Toolbar(fourLegTab, fourLegTabNames, GUILayout.Height(25));
                EditorGUILayout.Space(5);

                EditorGUILayout.BeginVertical("box");
                switch (fourLegTab)
                {
                    case 0:
                        ShowLegsSection();
                        break;
                    case 1:
                        ShowSkirtSection(script);
                        break;
                    case 2:
                        ShowDrawerSection(script);
                        break;
                    case 3:
                        ShowStrectherSection(script);
                        break;
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pillarThickness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("baseRadius"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("baseThickness"));
            }
        }

        void ShowLegsSection()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("legThickness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("legInset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("legSplayAngle"));
        }

        void ShowSkirtSection(ProceduralTableGenerator script)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasSkirt"));
            if (script.hasSkirt)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skirtStyle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skirtHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skirtThickness"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skirtInset"));
                EditorGUI.indentLevel--;
            }
        }

        void ShowDrawerSection(ProceduralTableGenerator script)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasDrawers"));
            if (script.hasDrawers)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("drawerCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("drawerHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("drawerDepth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("drawerSpacing"));
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("hasHandle"));
                if (script.hasHandle)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("handleWidth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("handleDepth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("handleHeight"));
                    EditorGUI.indentLevel--;
                }
            }
        }

        void ShowStrectherSection(ProceduralTableGenerator script)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasStretchers"));
            if (script.hasStretchers)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stretcherStyle"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stretcherHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stretcherThickness"));

                if (script.stretcherStyle == StretcherStyle.H)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hStretcherOffset"));
                }
                else if (script.stretcherStyle == StretcherStyle.Box)
                {
                    EditorGUI.indentLevel--;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("hasFootrest"));
                    if (script.hasFootrest)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("footrestThickness"));
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
        #endregion
    }
}

using UnityEngine;
using UnityEditor;
using System.Reflection;

public class ProceduralFuzzer : EditorWindow
{
    private ProceduralGenerator targetGenerator;
    private int iterations = 1000;
    private float floatMin = -5f;
    private float floatMax = 5f;

    private bool haltOnError = true;
    private bool ignoreColliders = true;
    private bool respectRangeAttributes = false;
    private int currentIteration = 0;

    [MenuItem("Tools/Procedural Fuzz Tester")]
    public static void ShowWindow()
    {
        GetWindow<ProceduralFuzzer>("Fuzz Tester");
    }

    private void OnGUI()
    {
        GUILayout.Label("Procedural Generator Fuzzer", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This tool rapidly injects extreme, randomized values into your generator to uncover math explosions, NaN vertices, and missing colliders.", MessageType.Info);

        targetGenerator = (ProceduralGenerator)EditorGUILayout.ObjectField("Target Generator", targetGenerator, typeof(ProceduralGenerator), true);

        iterations = EditorGUILayout.IntSlider("Iterations", iterations, 100, 10000);

        GUILayout.Label("Fuzzing Ranges (Intentional Stress Testing)");
        floatMin = EditorGUILayout.FloatField("Min Float/Int", floatMin);
        floatMax = EditorGUILayout.FloatField("Max Float/Int", floatMax);

        GUILayout.Space(5);
        haltOnError = EditorGUILayout.Toggle("Halt On Error", haltOnError);
        ignoreColliders = EditorGUILayout.Toggle("Ignore Missing Colliders", ignoreColliders);
        respectRangeAttributes = EditorGUILayout.Toggle("Respect [Range] Attributes", respectRangeAttributes);

        GUILayout.Space(10);
        GUI.enabled = targetGenerator != null;
        if (GUILayout.Button("START FUZZING", GUILayout.Height(40)))
        {
            RunFuzzTest();
        }
        GUI.enabled = true;
    }

    private void RunFuzzTest()
    {
        if (targetGenerator == null) { return; }

        FieldInfo[] fields = targetGenerator.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        int passed = 0;
        int failed = 0;

        for (currentIteration = 0; currentIteration < iterations; currentIteration++)
        {
            RandomizeFields(fields);

            try
            {
                targetGenerator.ForceGenerate();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Fuzz Fail] Exception thrown on iteration {currentIteration}:\n{e.Message}", targetGenerator);
                failed++;
                if (haltOnError) { break; }
            }

            if (!ValidateGeometry(out string errorReason))
            {
                Debug.LogError($"[Fuzz Fail] Geometry corrupted on iteration {currentIteration}: {errorReason}", targetGenerator);
                failed++;
                if (haltOnError) { break; }
            }

            passed++;

            if (currentIteration % 50 == 0)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Fuzzing...", $"Iteration {currentIteration}/{iterations}", (float)currentIteration / iterations))
                {
                    break;
                }
            }
        }

        EditorUtility.ClearProgressBar();

        if (failed == 0)
        {
            Debug.Log($"<color=green><b>[FUZZ PASS]</b></color> Generator survived {passed} extreme mutations without breaking!");
        }
    }

    private void RandomizeFields(FieldInfo[] fields)
    {
        foreach (var field in fields)
        {
            RangeAttribute rangeAttr = null;
            if (respectRangeAttributes)
            {
                rangeAttr = field.GetCustomAttribute<RangeAttribute>();
            }

            if (field.FieldType == typeof(float))
            {
                float min = rangeAttr != null ? rangeAttr.min : floatMin;
                float max = rangeAttr != null ? rangeAttr.max : floatMax;
                field.SetValue(targetGenerator, Random.Range(min, max));
            }
            else if (field.FieldType == typeof(int))
            {
                int min = rangeAttr != null ? Mathf.RoundToInt(rangeAttr.min) : Mathf.RoundToInt(floatMin);
                int max = rangeAttr != null ? Mathf.RoundToInt(rangeAttr.max) : Mathf.RoundToInt(floatMax);
                field.SetValue(targetGenerator, Random.Range(min, max));
            }
            else if (field.FieldType == typeof(bool))
            {
                field.SetValue(targetGenerator, Random.value > 0.5f);
            }
            else if (field.FieldType.IsEnum)
            {
                System.Array enumValues = System.Enum.GetValues(field.FieldType);
                field.SetValue(targetGenerator, enumValues.GetValue(Random.Range(0, enumValues.Length)));
            }
        }
    }

    private bool ValidateGeometry(out string reason)
    {
        reason = "";
        MeshFilter[] filters = targetGenerator.GetComponentsInChildren<MeshFilter>(false);

        foreach (var filter in filters)
        {
            if (filter.sharedMesh == null)
            {
                reason = $"Missing Mesh on {filter.gameObject.name}";
                return false;
            }

            foreach (Vector3 vertex in filter.sharedMesh.vertices)
            {
                if (float.IsNaN(vertex.x) || float.IsInfinity(vertex.x) ||
                    float.IsNaN(vertex.y) || float.IsInfinity(vertex.y) ||
                    float.IsNaN(vertex.z) || float.IsInfinity(vertex.z))
                {
                    reason = $"NaN or Infinity vertex detected on {filter.gameObject.name}!";
                    return false;
                }
            }

            if (!ignoreColliders)
            {
                if (!filter.TryGetComponent(out MeshCollider mc) || mc.sharedMesh == null)
                {
                    reason = $"Missing MeshCollider on {filter.gameObject.name}";
                    return false;
                }
            }
        }
        return true;
    }
}
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    [ExecuteAlways]
    public abstract class ProceduralGenerator : MonoBehaviour
    {
        public Color primaryColor = new(0.6f, 0.4f, 0.2f);
        public Color secondaryColor = new(0.2f, 0.5f, 0.8f);
        public ColorPalette primaryColorPalette;
        public ColorPalette secondaryColorPalette;

        [Header("Baking Settings")]
        public string prefabName = "GeneratedAsset";

        protected virtual void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) { return; }
                ForceGenerate();
            };
        }

        public void ForceGenerate()
        {
            GenerateGeometry();
            ApplyColors();
        }

        protected abstract void GenerateGeometry();
        public abstract void OrganizeHierarchy();
        public abstract void ApplyColors();
        public abstract void ApplyBakeMaterials(Material primaryMat, Material secondaryMat);

#if UNITY_EDITOR
        [ContextMenu("Reset Settings Only")]
        public void ResetSettingsOnly()
        {
            UnityEditor.Undo.RecordObject(this, "Reset Generator Settings");

            GameObject tempGO = new("TempDefaultHarvester")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            Component defaultComponent = tempGO.AddComponent(this.GetType());

            FieldInfo[] fields = this.GetType().GetFields(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.FlattenHierarchy);

            foreach (var field in fields)
            {
                if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                {
                    field.SetValue(this, field.GetValue(defaultComponent));
                }
                else if (typeof(IList).IsAssignableFrom(field.FieldType))
                {
                    if (field.GetValue(defaultComponent) is IList defaultList &&
                        field.GetValue(this) is IList currentList)
                    {
                        currentList.Clear();
                        foreach (var item in defaultList)
                        {
                            currentList.Add(item);
                        }
                    }
                }
            }

            DestroyImmediate(tempGO);
            ForceGenerate();
            UnityEditor.EditorUtility.SetDirty(this);

            Debug.Log($"[{this.GetType().Name}] Settings reset to default. Model references preserved.");
        }
#endif
    }
}

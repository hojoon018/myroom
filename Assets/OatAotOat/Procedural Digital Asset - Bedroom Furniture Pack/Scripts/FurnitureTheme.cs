using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OatAotOat.ProceduralDigitalAsset
{
    [CreateAssetMenu(fileName = "New Furniture Theme", menuName = "Procedural Tools/Furniture Theme")]
    public class FurnitureTheme : ScriptableObject
    {
        [Header("Theme Colors")]
        public Color primaryColor = new(0.6f, 0.4f, 0.2f);
        public Color secondaryColor = new(0.8f, 0.8f, 0.8f);

        [Header("Global Dimensions")]
        [Range(0.02f, 0.15f)] public float globalLegThickness = 0.05f;
        [Range(0.01f, 0.1f)] public float globalFrameThickness = 0.03f;
        [Range(0f, 25f)] public float globalLegSplayAngle = 0f;
        [Range(0.01f, 0.15f)] public float globalCushionThickness = 0.05f;

        [Header("Global Styles")]
        [Tooltip("Forces items like Chairs and Tables to use 4 Legs instead of Pedestals to unify the room.")]
        public bool forceFourLegs = true;
        public bool useSkirtsAndStretchers = true;

#if UNITY_EDITOR
        public void ApplyThemeTo(ProceduralGenerator generator)
        {
            Undo.RecordObject(generator, "Apply Furniture Theme");

            generator.primaryColor = primaryColor;
            generator.secondaryColor = secondaryColor;

            if (generator is ProceduralChairGenerator chair)
            {
                chair.frameThickness = globalFrameThickness;
                chair.cushionThickness = globalCushionThickness * 0.5f;
                if (forceFourLegs) chair.chairType = ChairType.FourLeg;

                chair.frontLegOffsetAngle = globalLegSplayAngle;
                chair.backLegOffsetAngle = globalLegSplayAngle;

                chair.legEndThickness = globalLegSplayAngle > 2f ? globalLegThickness * 0.5f : globalLegThickness;
            }
            else if (generator is ProceduralTableGenerator table)
            {
                table.legThickness = globalLegThickness;
                table.legSplayAngle = globalLegSplayAngle;
                if (forceFourLegs) table.tableType = TableType.FourLeg;

                table.hasSkirt = useSkirtsAndStretchers;
                table.hasStretchers = useSkirtsAndStretchers;
            }
            else if (generator is ProceduralBedGenerator bed)
            {
                bed.legThickness = globalLegThickness;
                bed.frameThickness = globalFrameThickness;
                bed.legSplayAngle = globalLegSplayAngle;
                if (forceFourLegs) bed.bedType = BedType.FourLeg;
            }
            else if (generator is ProceduralShelfGenerator shelf)
            {
                shelf.panelThickness = globalFrameThickness;
            }

            generator.ForceGenerate();
            EditorUtility.SetDirty(generator);
        }
#endif
    }
}

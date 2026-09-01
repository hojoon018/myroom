using System.Collections.Generic;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    public enum TableType { FourLeg, Pedestal }
    public enum TableShape { Rectangular, Round }
    public enum SkirtStyle { All, WidthSides, LengthSides }
    public enum StretcherStyle { Box, H, X }

    public class ProceduralTableGenerator : ProceduralGenerator
    {
        #region --- INSTRUCTOR REFERENCES ---
        [Header("Table Style")]
        public TableType tableType = TableType.FourLeg;
        public TableShape tableShape = TableShape.Rectangular;

        [Header("Model References")]
        public Transform tableTop;
        public Transform frontLeftLeg, frontRightLeg, backLeftLeg, backRightLeg;
        public Transform pedestalPillar, pedestalBase;

        [Header("Skirt References")]
        public Transform frontSkirt, backSkirt, leftSkirt, rightSkirt;

        [Header("Stretcher References")]
        public Transform stretcherLeft, stretcherRight, stretcherFront, stretcherBack;
        public Transform stretcherCross1, stretcherCross2, footrestBoard;

        [Header("Drawer References")]
        public GameObject drawerPrefab;
        public Transform drawerContainer;

        [Header("Source Geometry")]
        public Mesh baseLegMesh;
        public Mesh basePedestalMesh;
        public Mesh baseStretcherMesh;
        public Mesh baseRectangularTopMesh;
        public Mesh baseRoundTopMesh;
        #endregion

        #region --- SETTINGS ---
        [Header("Dimensions")]
        [Range(0.4f, 2f)] public float tableWidth = 0.8f;
        [Range(0.4f, 3f)] public float tableDepth = 1.5f;
        [Range(0.4f, 3f)] public float tableDiameter = 1.2f;
        [Range(0.3f, 1.5f)] public float tableHeight = 0.75f;
        [Range(0.02f, 0.1f)] public float topThickness = 0.03f;

        [Header("Four-Leg Settings")]
        [Range(0.02f, 0.2f)] public float legThickness = 0.06f;
        [Range(0f, 0.3f)] public float legInset = 0.05f;
        [Range(0f, 25f)] public float legSplayAngle = 5f;

        [Header("Skirt Settings")]
        public bool hasSkirt = true;
        public SkirtStyle skirtStyle = SkirtStyle.All;
        [Range(0.02f, 0.2f)] public float skirtHeight = 0.08f;
        [Range(0.01f, 0.1f)] public float skirtThickness = 0.02f;
        [Range(0f, 0.1f)] public float skirtInset = 0.02f;

        [Header("Drawer Settings")]
        public bool hasDrawers = false;
        [Range(1, 5)] public int drawerCount = 2;
        [Range(0.05f, 0.3f)] public float drawerHeight = 0.1f;
        [Range(0.2f, 1f)] public float drawerDepth = 0.4f;
        [Range(0.01f, 0.1f)] public float drawerSpacing = 0.02f;

        [Header("Handle Settings")]
        public bool hasHandle = false;
        [Range(0.05f, 0.5f)] public float handleWidth = 0.15f;
        [Range(0.01f, 0.1f)] public float handleDepth = 0.03f;
        [Range(0.01f, 0.1f)] public float handleHeight = 0.02f;

        [Header("Stretcher Settings")]
        public bool hasStretchers = true;
        public StretcherStyle stretcherStyle = StretcherStyle.Box;
        [Range(0.05f, 0.5f)] public float stretcherHeight = 0.2f;
        [Range(0.01f, 0.1f)] public float stretcherThickness = 0.03f;
        [Range(-1f, 1f)] public float hStretcherOffset = 0.0f;

        [Header("Footrest Settings")]
        public bool hasFootrest = false;
        [Range(0.01f, 0.1f)] public float footrestThickness = 0.02f;

        [Header("Pedestal Settings")]
        [Range(0.05f, 0.3f)] public float pillarThickness = 0.15f;
        [Range(0.2f, 1.5f)] public float baseRadius = 0.4f;
        [Range(0.02f, 0.2f)] public float baseThickness = 0.05f;

        private float CurrentDrawerDepth => Mathf.Min(drawerDepth, tableWidth);
        private float CurrentStrectherHeight => Mathf.Min(stretcherHeight, tableHeight / 2f);

        private Mesh shearedFrontLeft, shearedFrontRight, shearedBackLeft, shearedBackRight;
        #endregion

        #region --- CACHED DATA PART LISTS ---
        private IEnumerable<Transform> GetFourLegParts() => new[] { frontLeftLeg, frontRightLeg, backLeftLeg, backRightLeg };
        private IEnumerable<Transform> GetPedestalParts() => new[] { pedestalPillar, pedestalBase };
        private IEnumerable<Transform> GetSkirtParts() => new[] { frontSkirt, backSkirt, leftSkirt, rightSkirt };
        private IEnumerable<Transform> GetStretcherParts() => new[] { stretcherLeft, stretcherRight, stretcherFront, stretcherBack, stretcherCross1, stretcherCross2, footrestBoard };
        #endregion

        #region --- BASE CLASS IMPLEMENTATION ---
        protected override void GenerateGeometry()
        {
            if (tableTop == null) { return; }

            bool isRound = tableShape == TableShape.Round;
            float currentLength = isRound ? tableDiameter : tableDepth;
            float currentWidth = isRound ? tableDiameter : tableWidth;

            tableTop.localScale = new Vector3(currentLength, topThickness, currentWidth);
            tableTop.localPosition = new Vector3(0, tableHeight * 2f, 0);

            Mesh topMesh = isRound ? baseRoundTopMesh : baseRectangularTopMesh;
            if (topMesh != null) { ProceduralUtility.SetMeshAndCollider(tableTop, topMesh); }

            bool isFourLeg = tableType == TableType.FourLeg;
            bool isPedestal = tableType == TableType.Pedestal;

            foreach (Transform t in GetFourLegParts()) { if (t != null) t.gameObject.SetActive(isFourLeg); }
            foreach (Transform t in GetSkirtParts()) { if (t != null) t.gameObject.SetActive(isFourLeg); }
            foreach (Transform t in GetStretcherParts()) { if (t != null) t.gameObject.SetActive(isFourLeg); }

            if (drawerContainer != null) { drawerContainer.gameObject.SetActive(isFourLeg); }
            if (pedestalPillar != null) { pedestalPillar.gameObject.SetActive(isPedestal); }
            if (pedestalBase != null) { pedestalBase.gameObject.SetActive(isPedestal); }

            if (isFourLeg)
            {
                SetupFourLegs();
                SetupSkirts();
                SetupDrawers();
                SetupStretchers();
            }

            if (isPedestal) { SetupPedestal(); }
        }

        public override void OrganizeHierarchy()
        {
            tableTop.SetParent(transform);

            Transform rootGroup = ProceduralUtility.GetOrCreateGroup(transform, "Four-Leg Components");
            Transform pedestalGroup = ProceduralUtility.GetOrCreateGroup(transform, "Pedestal Components");
            Transform skirtGroup = ProceduralUtility.GetOrCreateGroup(transform, "Skirt Components");
            Transform stretcherGroup = ProceduralUtility.GetOrCreateGroup(transform, "Stretcher Components");

            ParentToGroup(GetFourLegParts(), rootGroup);
            ParentToGroup(GetPedestalParts(), pedestalGroup);
            ParentToGroup(GetSkirtParts(), skirtGroup);
            ParentToGroup(GetStretcherParts(), stretcherGroup);

            if (drawerContainer != null) { ProceduralUtility.ParentToGroup(drawerContainer, skirtGroup); }
        }

        public override void ApplyColors()
        {
            ProceduralUtility.SetColorToPart(tableTop, primaryColor);

            ApplyColorTo(GetFourLegParts(), secondaryColor);
            ApplyColorTo(GetPedestalParts(), secondaryColor);
            ApplyColorTo(GetSkirtParts(), secondaryColor);
            ApplyColorTo(GetStretcherParts(), secondaryColor);

            if (drawerContainer != null)
            {
                foreach (Transform drawer in drawerContainer)
                {
                    foreach (Transform child in drawer) { ProceduralUtility.SetColorToPart(child, secondaryColor); }
                }
            }
        }

        public override void ApplyBakeMaterials(Material primaryMat, Material secondaryMat)
        {
            ProceduralUtility.SetMaterialToPart(tableTop, primaryMat);

            ApplyMaterialTo(GetFourLegParts(), secondaryMat);
            ApplyMaterialTo(GetPedestalParts(), secondaryMat);
            ApplyMaterialTo(GetSkirtParts(), secondaryMat);
            ApplyMaterialTo(GetStretcherParts(), secondaryMat);

            if (drawerContainer != null)
            {
                foreach (Transform drawer in drawerContainer)
                {
                    ProceduralUtility.SetMaterialToPart(drawer, secondaryMat);

                    foreach (Transform child in drawer)
                    {
                        ProceduralUtility.SetMaterialToPart(child, secondaryMat);
                    }
                }
            }
        }
        #endregion

        #region --- UTILITY HELPERS ---
        void ApplyColorTo(IEnumerable<Transform> parts, Color color)
        {
            foreach (Transform t in parts) { if (t != null) ProceduralUtility.SetColorToPart(t, color); }
        }

        void ApplyMaterialTo(IEnumerable<Transform> parts, Material mat)
        {
            foreach (Transform t in parts) { if (t != null) ProceduralUtility.SetMaterialToPart(t, mat); }
        }

        void ParentToGroup(IEnumerable<Transform> parts, Transform group)
        {
            foreach (Transform t in parts) { if (t != null) ProceduralUtility.ParentToGroup(t, group); }
        }
        #endregion

        #region --- GEOMETRY SETUP MATH ---
        void SetupFourLegs()
        {
            float actualHeight = tableHeight / Mathf.Cos(legSplayAngle * Mathf.Deg2Rad);
            Vector3 legScale = new(legThickness, actualHeight, legThickness);

            bool isRound = tableShape == TableShape.Round;
            float currentLength = isRound ? (tableDiameter * 0.7071f) : tableDepth;
            float currentWidth = isRound ? (tableDiameter * 0.7071f) : tableWidth;

            float xPos = currentLength - legThickness - legInset;
            float zPos = currentWidth - legThickness - legInset;

            frontLeftLeg.SetLocalPositionAndRotation(new Vector3(-xPos, 0, zPos), Quaternion.Euler(-legSplayAngle, 0, -legSplayAngle));
            frontRightLeg.SetLocalPositionAndRotation(new Vector3(xPos, 0, zPos), Quaternion.Euler(-legSplayAngle, 0, legSplayAngle));
            backLeftLeg.SetLocalPositionAndRotation(new Vector3(-xPos, 0, -zPos), Quaternion.Euler(legSplayAngle, 0, -legSplayAngle));
            backRightLeg.SetLocalPositionAndRotation(new Vector3(xPos, 0, -zPos), Quaternion.Euler(legSplayAngle, 0, legSplayAngle));

            frontLeftLeg.localScale = legScale;
            frontRightLeg.localScale = legScale;
            backLeftLeg.localScale = legScale;
            backRightLeg.localScale = legScale;

            if (baseLegMesh == null && frontLeftLeg.TryGetComponent(out MeshFilter mf) && !mf.sharedMesh.name.Contains("Sheared"))
            {
                baseLegMesh = mf.sharedMesh;
            }

            if (baseLegMesh != null)
            {
                shearedFrontLeft = ProceduralUtility.GenerateUniversalShear(baseLegMesh, new Vector2(legSplayAngle, legSplayAngle), legScale, false, ref shearedFrontLeft);
                shearedFrontRight = ProceduralUtility.GenerateUniversalShear(baseLegMesh, new Vector2(legSplayAngle, -legSplayAngle), legScale, false, ref shearedFrontRight);
                shearedBackLeft = ProceduralUtility.GenerateUniversalShear(baseLegMesh, new Vector2(-legSplayAngle, legSplayAngle), legScale, false, ref shearedBackLeft);
                shearedBackRight = ProceduralUtility.GenerateUniversalShear(baseLegMesh, new Vector2(-legSplayAngle, -legSplayAngle), legScale, false, ref shearedBackRight);

                ProceduralUtility.SetMeshAndCollider(frontLeftLeg, shearedFrontLeft);
                ProceduralUtility.SetMeshAndCollider(frontRightLeg, shearedFrontRight);
                ProceduralUtility.SetMeshAndCollider(backLeftLeg, shearedBackLeft);
                ProceduralUtility.SetMeshAndCollider(backRightLeg, shearedBackRight);
            }
        }

        void SetupPedestal()
        {
            if (pedestalPillar != null)
            {
                pedestalPillar.localScale = new Vector3(pillarThickness, tableHeight - baseThickness, pillarThickness);
                pedestalPillar.SetLocalPositionAndRotation(new Vector3(0, baseThickness * 2f, 0), Quaternion.identity);
            }

            if (pedestalBase != null)
            {
                pedestalBase.localScale = new Vector3(baseRadius, baseThickness, baseRadius);
                pedestalBase.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
            }
        }

        void SetupSkirts()
        {
            if (frontSkirt == null || backSkirt == null || leftSkirt == null || rightSkirt == null) { return; }

            bool isRound = tableShape == TableShape.Round;
            float currentLength = isRound ? (tableDiameter * 0.7071f) : tableDepth;
            float currentWidth = isRound ? (tableDiameter * 0.7071f) : tableWidth;

            bool showWidthSides = hasSkirt && (skirtStyle == SkirtStyle.All || skirtStyle == SkirtStyle.WidthSides);
            bool showLengthSides = hasSkirt && (skirtStyle == SkirtStyle.All || skirtStyle == SkirtStyle.LengthSides);

            frontSkirt.gameObject.SetActive(showWidthSides);
            backSkirt.gameObject.SetActive(showWidthSides);
            leftSkirt.gameObject.SetActive(showLengthSides);
            rightSkirt.gameObject.SetActive(showLengthSides);

            if (!hasSkirt) { return; }

            if (hasDrawers)
            {
                frontSkirt.gameObject.SetActive(false);
            }

            float yPos = tableHeight * 2f;
            float widthScale = Mathf.Max(0.01f, currentLength - (skirtInset + legInset) * 2f);
            float lengthScale = Mathf.Max(0.01f, currentWidth - (skirtInset + skirtThickness + legInset) * 2f);

            float zOffset = currentWidth - skirtThickness - skirtInset - legThickness - legInset;
            float xOffset = currentLength - skirtThickness - skirtInset - legThickness - legInset;

            if (showWidthSides)
            {
                frontSkirt.localScale = new Vector3(widthScale, skirtHeight, skirtThickness);
                backSkirt.localScale = new Vector3(widthScale, skirtHeight, skirtThickness);

                frontSkirt.SetLocalPositionAndRotation(new Vector3(0, yPos, zOffset), Quaternion.identity);
                backSkirt.SetLocalPositionAndRotation(new Vector3(0, yPos, -zOffset), Quaternion.identity);
            }

            if (showLengthSides)
            {
                leftSkirt.localScale = new Vector3(skirtThickness, skirtHeight, lengthScale);
                rightSkirt.localScale = new Vector3(skirtThickness, skirtHeight, lengthScale);

                leftSkirt.SetLocalPositionAndRotation(new Vector3(-xOffset, yPos, 0), Quaternion.identity);
                rightSkirt.SetLocalPositionAndRotation(new Vector3(xOffset, yPos, 0), Quaternion.identity);
            }
        }

        void SetupDrawers()
        {
            if (drawerPrefab == null || drawerContainer == null) { return; }

            while (drawerContainer.childCount > 0)
            {
                DestroyImmediate(drawerContainer.GetChild(0).gameObject);
            }

            if (!hasDrawers) { return; }

            bool isRound = tableShape == TableShape.Round;
            float currentLength = isRound ? (tableDiameter * 0.7071f) : tableDepth;
            float currentWidth = isRound ? (tableDiameter * 0.7071f) : tableWidth;

            float drawerTotalSpace = (currentLength - skirtThickness * 2f - skirtInset * 2f - legThickness - legInset) * 2f;
            float actualSpacing = drawerSpacing;
            float requiredSpaceForSpacing = actualSpacing * (drawerCount - 1);
            if (requiredSpaceForSpacing >= drawerTotalSpace * 0.8f)
            {
                actualSpacing = (drawerTotalSpace * 0.8f) / Mathf.Max(1, (drawerCount - 1));
            }

            float rawDrawerWidth = (drawerTotalSpace - (actualSpacing * (drawerCount - 1))) / drawerCount;
            float singleDrawerWidth = Mathf.Max(0.001f, rawDrawerWidth);

            float startX = -(drawerTotalSpace / 2f) + (singleDrawerWidth / 2f);
            float yPos = tableHeight * 2f;
            float zOffset = currentWidth - skirtThickness - skirtInset - legThickness - legInset - (CurrentDrawerDepth / 4f);

            for (int i = 0; i < drawerCount; i++)
            {
                GameObject newDrawer = Instantiate(drawerPrefab, drawerContainer);
                newDrawer.transform.localPosition = new Vector3(startX + (i * (singleDrawerWidth + actualSpacing)), yPos, zOffset);
                newDrawer.transform.localScale = new Vector3(singleDrawerWidth, drawerHeight, CurrentDrawerDepth);
                newDrawer.transform.localRotation = Quaternion.identity;

                Transform handle = newDrawer.transform.Find("Handle");
                if (handle != null) { handle.gameObject.SetActive(hasHandle); }

                if (!hasHandle || handle == null) { continue; }

                handle.localScale = new Vector3(
                    Mathf.Min(handleWidth, singleDrawerWidth / 2f) / singleDrawerWidth,
                    Mathf.Min(handleHeight, drawerHeight / 4f) / drawerHeight, handleDepth);
            }
        }

        void SetupStretchers()
        {
            bool useStretcher = hasStretchers && tableType == TableType.FourLeg;

            if (stretcherLeft != null) { stretcherLeft.gameObject.SetActive(useStretcher && (stretcherStyle == StretcherStyle.Box || stretcherStyle == StretcherStyle.H)); }
            if (stretcherRight != null) { stretcherRight.gameObject.SetActive(useStretcher && (stretcherStyle == StretcherStyle.Box || stretcherStyle == StretcherStyle.H)); }
            if (stretcherFront != null) { stretcherFront.gameObject.SetActive(useStretcher && stretcherStyle == StretcherStyle.Box); }
            if (stretcherBack != null) { stretcherBack.gameObject.SetActive(useStretcher && stretcherStyle == StretcherStyle.Box); }
            if (stretcherCross1 != null) { stretcherCross1.gameObject.SetActive(useStretcher && (stretcherStyle == StretcherStyle.H || stretcherStyle == StretcherStyle.X)); }
            if (stretcherCross2 != null) { stretcherCross2.gameObject.SetActive(useStretcher && stretcherStyle == StretcherStyle.X); }

            if (footrestBoard != null) { footrestBoard.gameObject.SetActive(useStretcher && hasFootrest); }

            if (!useStretcher) { return; }

            bool isRound = tableShape == TableShape.Round;
            float currentLength = isRound ? (tableDiameter * 0.7071f) : tableDepth;
            float currentWidth = isRound ? (tableDiameter * 0.7071f) : tableWidth;

            float xPos = currentLength - legThickness - legInset;
            float zPos = currentWidth - legThickness - legInset;

            float actualStretcherY = CurrentStrectherHeight * 2f;

            Quaternion rotFL = Quaternion.Euler(-legSplayAngle, 0, -legSplayAngle);
            Quaternion rotFR = Quaternion.Euler(-legSplayAngle, 0, legSplayAngle);
            Quaternion rotBL = Quaternion.Euler(legSplayAngle, 0, -legSplayAngle);
            Quaternion rotBR = Quaternion.Euler(legSplayAngle, 0, legSplayAngle);

            Vector3 dirFL = rotFL * Vector3.up;
            Vector3 dirFR = rotFR * Vector3.up;
            Vector3 dirBL = rotBL * Vector3.up;
            Vector3 dirBR = rotBR * Vector3.up;

            Vector3 pivotFL = new(-xPos, 0, zPos);
            Vector3 pivotFR = new(xPos, 0, zPos);
            Vector3 pivotBL = new(-xPos, 0, -zPos);
            Vector3 pivotBR = new(xPos, 0, -zPos);

            Vector3 pFL = pivotFL + dirFL * (actualStretcherY / dirFL.y);
            Vector3 pFR = pivotFR + dirFR * (actualStretcherY / dirFR.y);
            Vector3 pBL = pivotBL + dirBL * (actualStretcherY / dirBL.y);
            Vector3 pBR = pivotBR + dirBR * (actualStretcherY / dirBR.y);

            if (stretcherStyle == StretcherStyle.Box)
            {
                PlaceStretcher(stretcherLeft, pFL, pBL);
                PlaceStretcher(stretcherRight, pFR, pBR);
                PlaceStretcher(stretcherFront, pFL, pFR);
                PlaceStretcher(stretcherBack, pBL, pBR);

                if (hasFootrest && footrestBoard != null)
                {
                    Vector3 centerPoint = (pFL + pFR + pBL + pBR) / 4f;
                    footrestBoard.localPosition = new Vector3(0, centerPoint.y + stretcherThickness / 2f - footrestThickness / 2f, 0);

                    float boardX = Vector3.Distance(pFL, pFR);
                    float boardZ = Vector3.Distance(pFL, pBL);

                    float meshX = 1f; float meshZ = 1f;
                    if (footrestBoard.TryGetComponent(out MeshFilter mf) && mf.sharedMesh != null)
                    {
                        meshX = mf.sharedMesh.bounds.size.x;
                        meshZ = mf.sharedMesh.bounds.size.z;
                    }
                    if (meshX <= 0.01f) meshX = 1f;
                    if (meshZ <= 0.01f) meshZ = 1f;

                    footrestBoard.localScale = new Vector3(boardX / meshX, footrestThickness, boardZ / meshZ);

                    if (baseRectangularTopMesh != null)
                    {
                        ProceduralUtility.SetMeshAndCollider(footrestBoard, baseRectangularTopMesh);
                    }
                }
            }
            else if (stretcherStyle == StretcherStyle.H)
            {
                PlaceStretcher(stretcherLeft, pFL, pBL);
                PlaceStretcher(stretcherRight, pFR, pBR);
                float actualOffset = hStretcherOffset > 0 ? Mathf.Min(zPos, hStretcherOffset) : Mathf.Max(hStretcherOffset, -zPos);
                Vector3 midL = (pFL + pBL) / 2f + new Vector3(0, 0, actualOffset);
                Vector3 midR = (pFR + pBR) / 2f + new Vector3(0, 0, actualOffset);
                PlaceStretcher(stretcherCross1, midL, midR);

                if (footrestBoard != null) { footrestBoard.gameObject.SetActive(false); }
            }
            else if (stretcherStyle == StretcherStyle.X)
            {
                PlaceStretcher(stretcherCross1, pFL, pBR);
                PlaceStretcher(stretcherCross2, pFR, pBL);

                if (footrestBoard != null) { footrestBoard.gameObject.SetActive(false); }
            }

            Mesh meshToUse = baseStretcherMesh != null ? baseStretcherMesh : baseLegMesh;
            if (meshToUse != null)
            {
                foreach (Transform t in new[] { stretcherLeft, stretcherRight, stretcherFront, stretcherBack,
                                            stretcherCross1, stretcherCross2 })
                {
                    ProceduralUtility.SetMeshAndCollider(t, meshToUse);
                }
            }
        }

        void PlaceStretcher(Transform t, Vector3 p1, Vector3 p2)
        {
            if (t == null) { return; }

            Vector3 midPoint = (p1 + p2) / 2f;
            t.SetLocalPositionAndRotation(midPoint, Quaternion.FromToRotation(Vector3.up, p2 - p1));

            float meshHeight = 1f;
            if (t.TryGetComponent(out MeshFilter mf) && mf.sharedMesh != null)
            {
                meshHeight = mf.sharedMesh.bounds.size.y;
            }
            if (meshHeight <= 0.01f) { meshHeight = 1f; }

            t.localScale = new Vector3(stretcherThickness, Vector3.Distance(p1, p2) / meshHeight, stretcherThickness);
        }
        #endregion
    }
}

using UnityEngine;
using System.Collections.Generic;

namespace OatAotOat.ProceduralDigitalAsset
{
    public enum ChairType { FourLeg, Pedestal }
    public enum ChairShape { Rectangular, Round }
    public enum BackrestType { Cushion, Spindles }

    public class ProceduralChairGenerator : ProceduralGenerator
    {
        #region --- INSTRUCTOR REFERENCES ---
        [Header("Chair Style")]
        public ChairType chairType = ChairType.FourLeg;
        public ChairShape chairShape = ChairShape.Rectangular;

        [Header("Model References")]
        public Transform seat;
        public Transform frontLeftLeg, frontRightLeg, backLeftLeg, backRightLeg;
        public Transform leftRocker, rightRocker;
        public Transform pedestalPillar, pedestalBase, boatSeat;
        public Transform backrest;

        [Header("Procedural Frames")]
        public Transform leftSeatFrame;
        public Transform rightSeatFrame, backSeatFrame;
        public Transform leftBackrestFrame, rightBackrestFrame, topBackrestFrame;

        [Header("Procedural Armrests")]
        public Transform leftArmrest;
        public Transform rightArmrest, leftArmrestSupport, rightArmrestSupport;
        public Transform leftArmrestSupportBack, rightArmrestSupportBack;
        public Transform leftArmrestCushion, rightArmrestCushion;

        [Header("Procedural Backrest")]
        public GameObject spindlePrefab;
        public Transform spindleContainer;

        [Header("Source Geometry (Prevents Cache Bugs)")]
        public Mesh baseLegMesh;
        public Mesh basePedestalMesh, baseBoatSeatMesh, baseBackrestMesh, baseSideFrameMesh;
        public Mesh baseSpindleMesh, baseRoundSeatMesh, baseRectangularSeatMesh, baseRockerMesh;
        #endregion

        #region --- SETTINGS ---
        [Header("Main Setting")]
        [Range(0.3f, 1f)] public float seatWidth = 0.45f;
        [Range(0.3f, 1f)] public float seatDepth = 0.45f;
        [Range(0.3f, 1f)] public float seatDiameter = 0.45f;
        [Range(0.02f, 0.2f)] public float seatHeight = 0.05f;
        [Range(0.2f, 1f)] public float legHeight = 0.45f;
        [Range(0.02f, 0.1f)] public float frameThickness = 0.04f;
        [Range(0f, 0.1f)] public float cushionThickness = 0.025f;
        [Range(0.0f, 0.2f)] public float frameInset = 0.05f;

        [Header("Four-Leg Settings")]
        [Range(0.01f, 0.1f)] public float legEndThickness = 0.02f;
        [Range(0f, 30f)] public float frontLegOffsetAngle = 5f;
        [Range(0f, 30f)] public float backLegOffsetAngle = 10f;

        [Header("Rocker Settings")]
        public bool hasRockers = false;
        [Range(0.2f, 1.5f)] public float extraRockerLength = 0.5f;
        [Range(0.05f, 0.3f)] public float rockerHeight = 0.1f;
        [Range(0.02f, 0.1f)] public float rockerThickness = 0.04f;

        [Header("Pedestal Settings")]
        [Range(0.02f, 0.2f)] public float pillarThickness = 0.08f;
        [Range(0.2f, 1f)] public float baseRadius = 0.3f;
        [Range(0.02f, 0.1f)] public float baseThickness = 0.04f;
        public bool hasBoatSeat = false;
        [Range(0.1f, 0.5f)] public float boatSeatRadius = 0.2f;
        [Range(0.01f, 0.1f)] public float boatSeatThickness = 0.02f;

        [Header("Backrest Settings")]
        public BackrestType backrestType = BackrestType.Cushion;
        [Range(0.2f, 1.5f)] public float backrestHeight = 0.5f;
        [Range(0.1f, 0.5f)] public float backrestDepth = 0.2f;
        [Range(0f, 45f)] public float laybackAngle = 10f;
        [Range(0.05f, 0.3f)] public float spindleSpacing = 0.1f;

        [Header("Armrest Settings")]
        public bool hasArmrests = true;
        [Range(0.1f, 0.5f)] public float armrestHeight = 0.25f;
        [Range(0.02f, 0.1f)] public float armrestWidth = 0.05f;
        [Range(0.2f, 0.8f)] public float armrestDepth = 0.4f;
        [Range(0.01f, 0.1f)] public float armrestThickness = 0.02f;

        public bool hasArmrestSupports = true;
        [Range(0.2f, 1f)] public float doubleSupportThreshold = 0.3f;

        public bool hasArmrestCushions = true;
        [Range(0.01f, 0.1f)] public float armCushionThickness = 0.025f;
        [Range(0.0f, 0.1f)] public float armCushionWidthOffset = 0.025f;
        [Range(0.0f, 0.1f)] public float armCushionDepthOffset = 0.025f;
        #endregion

        #region --- CACHED DATA & PROPERTIES ---
        private Mesh shearedFrontLegMesh, shearedBackLegMesh;
        private Mesh shearedBackrestMesh;
        private Mesh shearedSideFrameMesh;
        private Mesh shearedSpindleMesh;

        private bool IsRoundShape => chairShape == ChairShape.Round;
        private bool IsCushionBackrest => backrestType == BackrestType.Cushion;
        private float CurrentSeatWidth => IsRoundShape ? seatDiameter * 0.6071f : seatWidth;
        private float CurrentSeatDepth => IsRoundShape ? seatDiameter * 0.6071f : seatDepth;

        private float ActualFrameInset => Mathf.Min(CurrentSeatDepth / 2f, frameInset);
        private float FrameWidth => IsRoundShape ? CurrentSeatWidth : CurrentSeatWidth + frameThickness;
        private float FrameDepth => (CurrentSeatDepth - frameThickness) - ActualFrameInset;
        private float TopSeat => legHeight * 2f + seatHeight;
        private float ArmY => TopSeat + armrestHeight;

        private float ActualArmrestDepth
        {
            get
            {
                float frontZ = FrameDepth - (frameThickness * 2f);
                float backrestBaseZ = -CurrentSeatDepth - (frameThickness / 2f) + (backrestDepth / 20f);
                float backZ = backrestBaseZ / 2f - (armrestHeight * Mathf.Tan(laybackAngle * Mathf.Deg2Rad));
                float maxLength = frontZ - backZ - ActualFrameInset / 2f;
                return Mathf.Max(0.01f, Mathf.Min(armrestDepth, maxLength));
            }
        }

        private IEnumerable<Transform> GetFrameParts()
        {
            return new[] {
            leftSeatFrame, rightSeatFrame, backSeatFrame,
            leftBackrestFrame, rightBackrestFrame, topBackrestFrame,
            frontLeftLeg, frontRightLeg, backLeftLeg, backRightLeg,
            leftRocker, rightRocker,
            pedestalPillar, pedestalBase, boatSeat,
            leftArmrest, rightArmrest,
            leftArmrestSupport, rightArmrestSupport,
            leftArmrestSupportBack, rightArmrestSupportBack
        };
        }

        private IEnumerable<Transform> GetCushionParts()
        {
            return new[] { seat, backrest, leftArmrestCushion, rightArmrestCushion };
        }
        #endregion

        #region --- BASE CLASS IMPLEMENTATION ---
        protected override void GenerateGeometry()
        {
            if (seat == null || backrest == null) { return; }

            if (leftSeatFrame != null) { leftSeatFrame.gameObject.SetActive(!IsRoundShape); }
            if (rightSeatFrame != null) { rightSeatFrame.gameObject.SetActive(!IsRoundShape); }
            if (backSeatFrame != null) { backSeatFrame.gameObject.SetActive(!IsRoundShape); }

            SetupSeat();
            SetupBackrest();
            GenerateArmrests();
            GenerateArmrestSupports();
            GenerateSpindles();

            bool isFourLeg = chairType == ChairType.FourLeg;
            bool isPedestal = chairType == ChairType.Pedestal;

            if (frontLeftLeg != null) { frontLeftLeg.gameObject.SetActive(isFourLeg); }
            if (frontRightLeg != null) { frontRightLeg.gameObject.SetActive(isFourLeg); }
            if (backLeftLeg != null) { backLeftLeg.gameObject.SetActive(isFourLeg); }
            if (backRightLeg != null) { backRightLeg.gameObject.SetActive(isFourLeg); }

            if (pedestalPillar != null) { pedestalPillar.gameObject.SetActive(isPedestal); }
            if (pedestalBase != null) { pedestalBase.gameObject.SetActive(isPedestal); }
            if (boatSeat != null) { boatSeat.gameObject.SetActive(isPedestal && hasBoatSeat); }

            if (isFourLeg)
            {
                SetupLegs();
                SetupRockers();
            }
            else
            {
                if (leftRocker != null) { leftRocker.gameObject.SetActive(false); }
                if (rightRocker != null) { rightRocker.gameObject.SetActive(false); }
            }

            if (isPedestal)
            {
                SetupPedestal();
            }
        }

        public override void OrganizeHierarchy()
        {
            Transform seatGroup = ProceduralUtility.GetOrCreateGroup(transform, "Seat Components");
            Transform legGroup = ProceduralUtility.GetOrCreateGroup(transform, "Leg Components");
            Transform backrestGroup = ProceduralUtility.GetOrCreateGroup(transform, "Backrest Components");
            Transform armrestGroup = ProceduralUtility.GetOrCreateGroup(transform, "Armrest Components");

            ParentToGroup(new[] { seat, leftSeatFrame, rightSeatFrame, backSeatFrame }, seatGroup);
            ParentToGroup(new[] { frontLeftLeg, frontRightLeg, backLeftLeg, backRightLeg, leftRocker, rightRocker, pedestalPillar, pedestalBase, boatSeat }, legGroup);
            ParentToGroup(new[] { backrest, leftBackrestFrame, rightBackrestFrame, topBackrestFrame, spindleContainer }, backrestGroup);
            ParentToGroup(new[] { leftArmrest, rightArmrest, leftArmrestSupport, rightArmrestSupport, leftArmrestSupportBack, rightArmrestSupportBack, leftArmrestCushion, rightArmrestCushion }, armrestGroup);
        }

        public override void ApplyColors()
        {
            ApplyColorTo(GetFrameParts(), primaryColor);
            ApplyColorTo(GetCushionParts(), secondaryColor);

            if (spindleContainer != null)
            {
                foreach (Transform spindle in spindleContainer) { ProceduralUtility.SetColorToPart(spindle, primaryColor); }
            }
        }

        public override void ApplyBakeMaterials(Material primaryMat, Material secondaryMat)
        {
            ApplyMaterialTo(GetFrameParts(), primaryMat);
            ApplyMaterialTo(GetCushionParts(), secondaryMat);

            if (spindleContainer != null)
            {
                foreach (Transform spindle in spindleContainer) { ProceduralUtility.SetMaterialToPart(spindle, primaryMat); }
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

        void ClearContainer(Transform container)
        {
            if (container == null) { return; }
            while (container.childCount > 0) { DestroyImmediate(container.GetChild(0).gameObject); }
        }
        #endregion

        #region --- GEOMETRY SETUP MATH ---
        void SetupSeat()
        {
            bool isRound = chairShape == ChairShape.Round;
            float width = isRound ? seatDiameter : seatWidth;
            float length = isRound ? seatDiameter : seatDepth;

            seat.localScale = new Vector3(width, seatHeight + cushionThickness, length);
            seat.localPosition = new Vector3(0, legHeight * 2f, IsRoundShape ? 0 : frameThickness);

            Mesh seatMeshToUse = isRound ? baseRoundSeatMesh : baseRectangularSeatMesh;
            if (seatMeshToUse != null)
            {
                ProceduralUtility.SetMeshAndCollider(seat, seatMeshToUse);
            }

            if (IsRoundShape) { return; }

            if (leftSeatFrame != null && rightSeatFrame != null && backSeatFrame != null)
            {
                leftSeatFrame.localScale = new Vector3(frameThickness, seatHeight, CurrentSeatDepth + frameThickness);
                leftSeatFrame.localPosition = new Vector3(-FrameWidth, legHeight * 2f, 0);

                rightSeatFrame.localScale = new Vector3(frameThickness, seatHeight, CurrentSeatDepth + frameThickness);
                rightSeatFrame.localPosition = new Vector3(FrameWidth, legHeight * 2f, 0);

                backSeatFrame.localScale = new Vector3(CurrentSeatWidth, seatHeight, frameThickness);
                backSeatFrame.localPosition = new Vector3(0, legHeight * 2f, -FrameDepth - frameThickness - ActualFrameInset);
            }
        }

        void SetupLegs()
        {
            if (frontLeftLeg == null) { return; }

            float legY = hasRockers ? (rockerHeight + rockerThickness) / 4f : 0;

            frontLeftLeg.localPosition = new Vector3(-FrameWidth, legY, FrameDepth + frameThickness);
            frontRightLeg.localPosition = new Vector3(FrameWidth, legY, FrameDepth + frameThickness);
            backLeftLeg.localPosition = new Vector3(-FrameWidth, legY, -FrameDepth - frameThickness);
            backRightLeg.localPosition = new Vector3(FrameWidth, legY, -FrameDepth - frameThickness);

            float visualLegHeight = hasRockers ? Mathf.Max(0.01f, legHeight + seatHeight - legY) : legHeight;

            float actualFrontLegHeight = visualLegHeight / Mathf.Cos(frontLegOffsetAngle * Mathf.Deg2Rad);
            float actualBackLegHeight = visualLegHeight / Mathf.Cos(backLegOffsetAngle * Mathf.Deg2Rad);

            Vector3 frontLegScale = new(frameThickness, actualFrontLegHeight, frameThickness);
            Vector3 backLegScale = new(frameThickness, actualBackLegHeight, frameThickness);

            frontLeftLeg.localScale = frontLegScale;
            frontRightLeg.localScale = frontLegScale;
            backLeftLeg.localScale = backLegScale;
            backRightLeg.localScale = backLegScale;

            frontLeftLeg.localRotation = Quaternion.Euler(-frontLegOffsetAngle, 0, 0);
            frontRightLeg.localRotation = Quaternion.Euler(-frontLegOffsetAngle, 0, 0);
            backLeftLeg.localRotation = Quaternion.Euler(backLegOffsetAngle, 0, 0);
            backRightLeg.localRotation = Quaternion.Euler(backLegOffsetAngle, 0, 0);

            if (baseLegMesh == null && frontLeftLeg.TryGetComponent(out MeshFilter mf) &&
                mf.sharedMesh != null && !mf.sharedMesh.name.Contains("Sheared"))
            {
                baseLegMesh = mf.sharedMesh;
            }

            if (baseLegMesh != null)
            {
                float safeFrameThickness = Mathf.Max(0.001f, frameThickness);
                float taperRatio = legEndThickness / safeFrameThickness;

                shearedFrontLegMesh = ProceduralUtility.GenerateUniversalShear(baseLegMesh,
                    new Vector2(frontLegOffsetAngle, 0), frontLegScale, false, ref shearedFrontLegMesh, taperRatio);
                shearedBackLegMesh = ProceduralUtility.GenerateUniversalShear(baseLegMesh,
                    new Vector2(-backLegOffsetAngle, 0), backLegScale, false, ref shearedBackLegMesh, taperRatio);

                ProceduralUtility.SetMeshAndCollider(frontLeftLeg, shearedFrontLegMesh);
                ProceduralUtility.SetMeshAndCollider(frontRightLeg, shearedFrontLegMesh);
                ProceduralUtility.SetMeshAndCollider(backLeftLeg, shearedBackLegMesh);
                ProceduralUtility.SetMeshAndCollider(backRightLeg, shearedBackLegMesh);
            }
        }

        void SetupRockers()
        {
            if (leftRocker == null || rightRocker == null) { return; }

            leftRocker.gameObject.SetActive(hasRockers);
            rightRocker.gameObject.SetActive(hasRockers);

            if (!hasRockers) { return; }

            float visualLegHeight = Mathf.Max(0.01f, legHeight - rockerHeight);
            float frontBottomZ = FrameDepth + frameThickness + (visualLegHeight * Mathf.Tan(frontLegOffsetAngle * Mathf.Deg2Rad));
            float backBottomZ = -FrameDepth - frameThickness - (visualLegHeight * Mathf.Tan(backLegOffsetAngle * Mathf.Deg2Rad));

            float midZ = (frontBottomZ + backBottomZ) / 2f;

            leftRocker.localPosition = new Vector3(-FrameWidth, 0, midZ);
            rightRocker.localPosition = new Vector3(FrameWidth, 0, midZ);

            Vector3 rScale = new(rockerThickness, rockerHeight, extraRockerLength + CurrentSeatDepth);
            leftRocker.localScale = rScale;
            rightRocker.localScale = rScale;

            if (baseRockerMesh != null)
            {
                ProceduralUtility.SetMeshAndCollider(leftRocker, baseRockerMesh);
                ProceduralUtility.SetMeshAndCollider(rightRocker, baseRockerMesh);
            }
        }

        void SetupPedestal()
        {
            if (pedestalPillar != null)
            {
                pedestalPillar.localScale = new Vector3(pillarThickness, legHeight - baseThickness, pillarThickness);
                pedestalPillar.SetLocalPositionAndRotation(new Vector3(0, baseThickness * 2f, 0), Quaternion.identity);
            }

            if (pedestalBase != null)
            {
                pedestalBase.localScale = new Vector3(baseRadius, baseThickness, baseRadius);
                pedestalBase.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
            }

            if (boatSeat != null && hasBoatSeat)
            {
                boatSeat.localScale = new Vector3(boatSeatRadius, boatSeatThickness, boatSeatRadius);
                boatSeat.SetLocalPositionAndRotation(new Vector3(0, legHeight, 0), Quaternion.identity);

                if (baseBoatSeatMesh != null)
                {
                    ProceduralUtility.SetMeshAndCollider(boatSeat, baseBoatSeatMesh);
                }
            }
        }

        void SetupBackrest()
        {
            Vector3 backrestPos = new(0, TopSeat, -CurrentSeatDepth - frameThickness / 2f + backrestDepth / 20f);

            if (baseBackrestMesh == null && backrest.TryGetComponent(out MeshFilter mfB) &&
                mfB.sharedMesh != null && !mfB.sharedMesh.name.Contains("Sheared"))
            {
                baseBackrestMesh = mfB.sharedMesh;
            }

            if (baseSideFrameMesh == null && leftBackrestFrame.TryGetComponent(out MeshFilter mfF) &&
                mfF.sharedMesh != null && !mfF.sharedMesh.name.Contains("Sheared"))
            {
                baseSideFrameMesh = mfF.sharedMesh;
            }

            backrest.gameObject.SetActive(IsCushionBackrest);

            if (IsCushionBackrest)
            {
                backrest.localPosition = backrestPos + new Vector3(0, 0, cushionThickness);
                Vector3 backrestScale = new(CurrentSeatWidth, backrestHeight - frameThickness, backrestDepth + cushionThickness * 10f);
                backrest.localScale = backrestScale;
                backrest.localRotation = Quaternion.Euler(-laybackAngle, 0, 0);

                if (baseBackrestMesh != null)
                {
                    shearedBackrestMesh = ProceduralUtility.GenerateUniversalShear(baseBackrestMesh,
                        new Vector2(laybackAngle, 0), backrestScale, true, ref shearedBackrestMesh);
                    ProceduralUtility.SetMeshAndCollider(backrest, shearedBackrestMesh);
                }
            }

            if (leftBackrestFrame != null && rightBackrestFrame != null && topBackrestFrame != null)
            {
                float sideFrameHeight = backrestHeight - frameThickness;
                Vector3 sideFrameScale = new(frameThickness, sideFrameHeight, backrestDepth);

                leftBackrestFrame.localScale = new Vector3(frameThickness, sideFrameHeight, backrestDepth);
                leftBackrestFrame.SetLocalPositionAndRotation(
                    new Vector3(-FrameWidth, backrestPos.y, backrestPos.z), Quaternion.Euler(-laybackAngle, 0, 0));

                rightBackrestFrame.localScale = new Vector3(frameThickness, sideFrameHeight, backrestDepth);
                rightBackrestFrame.SetLocalPositionAndRotation(
                    new Vector3(FrameWidth, backrestPos.y, backrestPos.z), Quaternion.Euler(-laybackAngle, 0, 0));

                if (baseSideFrameMesh != null)
                {
                    shearedSideFrameMesh = ProceduralUtility.GenerateUniversalShear(baseSideFrameMesh,
                        new Vector2(laybackAngle, 0), sideFrameScale, true, ref shearedSideFrameMesh);
                    ProceduralUtility.SetMeshAndCollider(leftBackrestFrame, shearedSideFrameMesh);
                    ProceduralUtility.SetMeshAndCollider(rightBackrestFrame, shearedSideFrameMesh);
                }

                float sideMeshTop = 0.5f;
                if (leftBackrestFrame.TryGetComponent(out MeshFilter mf1) && mf1.sharedMesh != null)
                {
                    sideMeshTop = mf1.sharedMesh.bounds.max.y;
                }

                float exactHeightOffset = (sideFrameHeight * sideMeshTop);
                Vector3 topPos = new Vector3(0, backrestPos.y, backrestPos.z) + (Quaternion.Euler(-laybackAngle, 0, 0) * Vector3.up * exactHeightOffset);

                topBackrestFrame.localScale = new Vector3(FrameWidth + frameThickness, frameThickness, backrestDepth);
                topBackrestFrame.SetLocalPositionAndRotation(topPos, Quaternion.Euler(-laybackAngle, 0, 0));
            }
        }

        void GenerateArmrests()
        {
            if (leftArmrest == null || rightArmrest == null) { return; }

            leftArmrest.gameObject.SetActive(hasArmrests);
            rightArmrest.gameObject.SetActive(hasArmrests);

            if (leftArmrestCushion != null && rightArmrestCushion != null)
            {
                leftArmrestCushion.gameObject.SetActive(hasArmrests && hasArmrestCushions);
                rightArmrestCushion.gameObject.SetActive(hasArmrests && hasArmrestCushions);
            }

            if (!hasArmrests) { return; }

            float armZPosition = FrameDepth - (ActualArmrestDepth / 2f) + (frameThickness / 2f);

            leftArmrest.localPosition = new Vector3(-FrameWidth, ArmY, armZPosition);
            rightArmrest.localPosition = new Vector3(FrameWidth, ArmY, armZPosition);

            Vector3 armScale = new(armrestWidth, armrestThickness, ActualArmrestDepth);
            leftArmrest.localScale = armScale;
            rightArmrest.localScale = armScale;

            if (hasArmrestCushions && leftArmrestCushion != null && rightArmrestCushion != null)
            {
                float cushionY = ArmY + armrestThickness * 2f;
                leftArmrestCushion.SetLocalPositionAndRotation(
                    new Vector3(-FrameWidth, cushionY, armZPosition), Quaternion.identity);
                rightArmrestCushion.SetLocalPositionAndRotation(
                    new Vector3(FrameWidth, cushionY, armZPosition), Quaternion.identity);

                float safeWidth = Mathf.Max(0.001f, armrestWidth - armCushionWidthOffset);
                float safeDepth = Mathf.Max(0.001f, ActualArmrestDepth - armCushionDepthOffset);

                Vector3 cushionScale = new(safeWidth, armCushionThickness, safeDepth);
                leftArmrestCushion.localScale = cushionScale;
                rightArmrestCushion.localScale = cushionScale;
            }
        }

        void GenerateArmrestSupports()
        {
            if (leftArmrestSupport == null || rightArmrestSupport == null) { return; }

            bool useBackSupports = ActualArmrestDepth >= doubleSupportThreshold;

            leftArmrestSupport.gameObject.SetActive(hasArmrests && hasArmrestSupports);
            rightArmrestSupport.gameObject.SetActive(hasArmrests && hasArmrestSupports);

            if (leftArmrestSupportBack != null && rightArmrestSupportBack != null)
            {
                leftArmrestSupportBack.gameObject.SetActive(hasArmrests && hasArmrestSupports && useBackSupports);
                rightArmrestSupportBack.gameObject.SetActive(hasArmrests && hasArmrestSupports && useBackSupports);
            }

            if (!hasArmrests || !hasArmrestSupports) { return; }

            float supportY = armrestHeight / 2f;
            Vector3 supportScale = new(frameThickness, supportY, frameThickness);

            leftArmrestSupport.localScale = supportScale;
            rightArmrestSupport.localScale = supportScale;

            float frontSupportZ = FrameDepth + frameThickness;
            leftArmrestSupport.SetLocalPositionAndRotation(
                new Vector3(-FrameWidth, TopSeat + baseThickness / 2f, frontSupportZ), Quaternion.identity);
            rightArmrestSupport.SetLocalPositionAndRotation(
                new Vector3(FrameWidth, TopSeat + baseThickness / 2f, frontSupportZ), Quaternion.identity);

            if (useBackSupports && leftArmrestSupportBack != null && rightArmrestSupportBack != null)
            {
                leftArmrestSupportBack.localScale = supportScale;
                rightArmrestSupportBack.localScale = supportScale;

                float backSupportZ = frontSupportZ - ActualArmrestDepth + frameThickness;

                leftArmrestSupportBack.SetLocalPositionAndRotation(
                    new Vector3(-FrameWidth, TopSeat + baseThickness / 2f, backSupportZ), Quaternion.identity);
                rightArmrestSupportBack.SetLocalPositionAndRotation(
                    new Vector3(FrameWidth, TopSeat + baseThickness / 2f, backSupportZ), Quaternion.identity);
            }
        }

        void GenerateSpindles()
        {
            if (spindlePrefab == null || spindleContainer == null) { return; }

            if (baseSpindleMesh == null && spindlePrefab.TryGetComponent(out MeshFilter mfS) &&
                mfS.sharedMesh != null && !mfS.sharedMesh.name.Contains("Sheared"))
            {
                baseSpindleMesh = mfS.sharedMesh;
            }

            ClearContainer(spindleContainer);

            if (IsCushionBackrest) { return; }

            Vector3 spindleScale = new(frameThickness * 0.5f, backrestHeight - frameThickness, frameThickness * 0.5f);

            if (baseSpindleMesh != null)
            {
                shearedSpindleMesh = ProceduralUtility.GenerateUniversalShear(baseSpindleMesh,
                    new Vector2(laybackAngle, 0), spindleScale, true, ref shearedSpindleMesh);
            }

            int spindleCount = Mathf.Max(1, Mathf.FloorToInt((CurrentSeatWidth * 2f) / spindleSpacing));
            float startX = -((spindleCount - 1) * spindleSpacing) / 2f;

            for (int i = 0; i < spindleCount; i++)
            {
                GameObject newSpindle = Instantiate(spindlePrefab, spindleContainer);
                newSpindle.transform.localPosition = new Vector3(startX + (i * spindleSpacing), TopSeat, -CurrentSeatDepth);
                newSpindle.transform.localScale = spindleScale;
                newSpindle.transform.localRotation = Quaternion.Euler(-laybackAngle, 0, 0);

                if (shearedSpindleMesh != null)
                {
                    ProceduralUtility.SetMeshAndCollider(newSpindle.transform, shearedSpindleMesh);
                }
            }
        }
        #endregion
    }
}

using UnityEngine;
using System.Collections.Generic;

namespace OatAotOat.ProceduralDigitalAsset
{
    public enum BedType { FourLeg, Platform }
    public enum BoardType { Blank, Slat, Cushion }

    public class ProceduralBedGenerator : ProceduralGenerator
    {
        #region --- INSTRUCTOR REFERENCES ---
        [Header("Bed Style")]
        public BedType bedType = BedType.FourLeg;

        [Header("Model References")]
        public Transform frontLeftLeg;
        public Transform frontRightLeg, backLeftLeg, backRightLeg, platformBase;

        [Header("Frame References")]
        public Transform frameLeft;
        public Transform frameRight, frameFront, frameBack;

        [Header("Headboard References")]
        public Transform headboardPanel;
        public Transform headboardBottomRail;
        public Transform headboardCushion, leftHeadboardPost, rightHeadboardPost;
        public GameObject headboardSlatPrefab;
        public Transform headboardSlatContainer;

        [Header("Footboard References")]
        public Transform footboardPanel;
        public Transform footboardCushion, leftFootboardPost, rightFootboardPost;
        public GameObject footboardSlatPrefab;
        public Transform footboardSlatContainer;

        [Header("Mattress & Pillows")]
        public Transform mattress;
        public Transform leftPillow, rightPillow;

        [Header("Source Geometry")]
        public Mesh baseLegMesh;
        public Mesh basePlatformMesh, baseFrameMesh, baseBoardMesh, basePostMesh;
        public Mesh baseMattressMesh, basePillowMesh, baseCushionMesh, baseSlatMesh;
        #endregion

        #region --- SETTINGS ---
        [Header("Dimensions")]
        [Range(1f, 3f)] public float bedWidth = 1.6f;
        [Range(1.5f, 3f)] public float bedDepth = 2f;
        [Range(0.2f, 0.8f)] public float bedClearanceHeight = 0.3f;

        [Header("Frame Settings")]
        [Range(0.1f, 0.4f)] public float frameHeight = 0.2f;
        [Range(0.02f, 0.1f)] public float frameThickness = 0.05f;

        [Header("Leg Settings")]
        [Range(0.05f, 0.2f)] public float legThickness = 0.08f;
        [Range(0.01f, 0.2f)] public float legEndThickness = 0.04f;
        [Range(0f, 15f)] public float legSplayAngle = 5f;
        [Range(0.0f, 0.2f)] public float legInset = 0.05f;

        [Header("Platform Settings")]
        [Range(0.0f, 0.5f)] public float platformInset = 0.1f;

        [Header("Headboard Settings")]
        public bool hasHeadboard = true;
        public BoardType headboardType = BoardType.Blank;
        [Range(0.5f, 1.5f)] public float headboardHeight = 0.8f;
        [Range(0.02f, 0.05f)] public float headboardThickness = 0.05f;
        [Range(0f, 20f)] public float laybackAngle = 10f;
        [Range(0f, 0.5f)] public float headboardLift = 0.1f;

        [Range(0.05f, 0.2f)] public float headboardRailHeight = 0.1f;
        [Range(0.05f, 0.3f)] public float headboardSlatSpacing = 0.15f;
        [Range(0.02f, 0.1f)] public float headboardSlatThickness = 0.03f;
        [Range(0.02f, 0.2f)] public float headboardCushionMargin = 0.05f;
        [Range(0.01f, 0.1f)] public float headboardCushionProjection = 0.02f;

        [Header("Footboard Settings")]
        public bool hasFootboard = false;
        public BoardType footboardType = BoardType.Blank;
        [Range(0.2f, 1.0f)] public float footboardHeight = 0.4f;
        [Range(0.02f, 0.05f)] public float footboardThickness = 0.05f;
        [Range(0f, 20f)] public float footboardLaybackAngle = 0f;

        [Range(0.05f, 0.2f)] public float footboardTopRailHeight = 0.1f;
        [Range(0.05f, 0.3f)] public float footboardSlatSpacing = 0.15f;
        [Range(0.02f, 0.1f)] public float footboardSlatThickness = 0.03f;
        [Range(0.02f, 0.2f)] public float footboardCushionMargin = 0.05f;
        [Range(0.01f, 0.1f)] public float footboardCushionProjection = 0.02f;

        [Header("Mattress Settings")]
        [Range(0.1f, 0.4f)] public float mattressThickness = 0.2f;
        [Range(0.0f, 0.2f)] public float mattressInset = 0.1f;

        [Header("Pillow Settings")]
        public bool hasPillows = true;
        [Range(0.3f, 0.8f)] public float pillowWidth = 0.6f;
        [Range(0.2f, 0.5f)] public float pillowDepth = 0.4f;
        [Range(0.05f, 0.2f)] public float pillowThickness = 0.1f;
        [Range(0.1f, 0.5f)] public float pillowOffset = 0.1f;

        private Mesh shearedFrontLeft, shearedFrontRight, shearedBackLeft, shearedBackRight;
        private Mesh shearedHeadboard, shearedHeadboardPost, shearedHeadboardSlat, shearedHeadboardBottom;
        private Mesh shearedFootboard, shearedFootboardPost, shearedFootboardSlat;
        #endregion

        #region --- CACHED DATA PART LISTS ---
        private IEnumerable<Transform> GetBaseLegParts() => new[] { frontLeftLeg, frontRightLeg, backLeftLeg, backRightLeg, platformBase };
        private IEnumerable<Transform> GetFrameParts() => new[] { frameLeft, frameRight, frameFront, frameBack };
        private IEnumerable<Transform> GetHeadboardParts() => new[] { headboardPanel, headboardBottomRail, leftHeadboardPost, rightHeadboardPost };
        private IEnumerable<Transform> GetFootboardParts() => new[] { footboardPanel, leftFootboardPost, rightFootboardPost };
        private IEnumerable<Transform> GetCushionParts() => new[] { mattress, leftPillow, rightPillow, headboardCushion, footboardCushion };
        #endregion

        #region --- BASE CLASS IMPLEMENTATION ---
        protected override void GenerateGeometry()
        {
            foreach (Transform t in GetFrameParts()) { if (t != null) t.gameObject.SetActive(true); }
            if (mattress != null) { mattress.gameObject.SetActive(true); }

            bool isFourLeg = bedType == BedType.FourLeg;
            bool isPlatform = bedType == BedType.Platform;

            foreach (Transform t in new[] { frontLeftLeg, frontRightLeg, backLeftLeg, backRightLeg }) { if (t != null) t.gameObject.SetActive(isFourLeg); }
            if (platformBase != null) { platformBase.gameObject.SetActive(isPlatform); }

            foreach (Transform t in GetHeadboardParts()) { if (t != null) t.gameObject.SetActive(hasHeadboard); }
            if (headboardSlatContainer != null) { headboardSlatContainer.gameObject.SetActive(hasHeadboard && headboardType == BoardType.Slat); }
            if (headboardCushion != null) { headboardCushion.gameObject.SetActive(hasHeadboard && headboardType == BoardType.Cushion); }
            if (headboardBottomRail != null) { headboardBottomRail.gameObject.SetActive(hasHeadboard && headboardType == BoardType.Slat && headboardLift > 0.01f); }

            foreach (Transform t in GetFootboardParts()) { if (t != null) t.gameObject.SetActive(hasFootboard); }
            if (footboardSlatContainer != null) { footboardSlatContainer.gameObject.SetActive(hasFootboard && footboardType == BoardType.Slat); }
            if (footboardCushion != null) { footboardCushion.gameObject.SetActive(hasFootboard && footboardType == BoardType.Cushion); }

            if (leftPillow != null) { leftPillow.gameObject.SetActive(hasPillows); }
            if (rightPillow != null) { rightPillow.gameObject.SetActive(hasPillows); }

            if (isFourLeg) { SetupLegs(); }
            if (isPlatform) { SetupPlatform(); }

            SetupFrame();
            SetupMattress();

            if (hasHeadboard) { SetupBoard(true); }
            if (hasFootboard) { SetupBoard(false); }

            if (hasPillows) { SetupPillows(); }
        }

        public override void OrganizeHierarchy()
        {
            Transform frameGroup = ProceduralUtility.GetOrCreateGroup(transform, "Frame Components");
            Transform baseGroup = ProceduralUtility.GetOrCreateGroup(transform, "Base Components");
            Transform headboardGroup = ProceduralUtility.GetOrCreateGroup(transform, "Headboard Components");
            Transform footboardGroup = ProceduralUtility.GetOrCreateGroup(transform, "Footboard Components");
            Transform beddingGroup = ProceduralUtility.GetOrCreateGroup(transform, "Bedding Components");

            ParentToGroup(GetFrameParts(), frameGroup);
            ParentToGroup(GetBaseLegParts(), baseGroup);

            ParentToGroup(GetHeadboardParts(), headboardGroup);
            if (headboardCushion != null) { ProceduralUtility.ParentToGroup(headboardCushion, headboardGroup); }
            if (headboardSlatContainer != null) { ProceduralUtility.ParentToGroup(headboardSlatContainer, headboardGroup); }

            ParentToGroup(GetFootboardParts(), footboardGroup);
            if (footboardCushion != null) { ProceduralUtility.ParentToGroup(footboardCushion, footboardGroup); }
            if (footboardSlatContainer != null) { ProceduralUtility.ParentToGroup(footboardSlatContainer, footboardGroup); }

            ParentToGroup(GetCushionParts(), beddingGroup);
        }

        public override void ApplyColors()
        {
            ApplyColorTo(GetBaseLegParts(), primaryColor);
            ApplyColorTo(GetFrameParts(), primaryColor);
            ApplyColorTo(GetHeadboardParts(), primaryColor);
            ApplyColorTo(GetFootboardParts(), primaryColor);

            ApplyColorTo(GetCushionParts(), secondaryColor);

            if (headboardSlatContainer != null) { foreach (Transform slat in headboardSlatContainer) ProceduralUtility.SetColorToPart(slat, primaryColor); }
            if (footboardSlatContainer != null) { foreach (Transform slat in footboardSlatContainer) ProceduralUtility.SetColorToPart(slat, primaryColor); }
        }

        public override void ApplyBakeMaterials(Material primaryMat, Material secondaryMat)
        {
            ApplyMaterialTo(GetBaseLegParts(), primaryMat);
            ApplyMaterialTo(GetFrameParts(), primaryMat);
            ApplyMaterialTo(GetHeadboardParts(), primaryMat);
            ApplyMaterialTo(GetFootboardParts(), primaryMat);

            ApplyMaterialTo(GetCushionParts(), secondaryMat);

            if (headboardSlatContainer != null) { foreach (Transform slat in headboardSlatContainer) ProceduralUtility.SetMaterialToPart(slat, primaryMat); }
            if (footboardSlatContainer != null) { foreach (Transform slat in footboardSlatContainer) ProceduralUtility.SetMaterialToPart(slat, primaryMat); }
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
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying) { Destroy(container.GetChild(i).gameObject); }
                else { DestroyImmediate(container.GetChild(i).gameObject); }
            }
        }
        #endregion

        #region --- GEOMETRY SETUP MATH ---
        void SetupLegs()
        {
            if (frontLeftLeg == null) { return; }

            float actualHeight = bedClearanceHeight / Mathf.Cos(legSplayAngle * Mathf.Deg2Rad);
            Vector3 legScale = new(legThickness, actualHeight, legThickness);

            float xPos = bedWidth - (legThickness / 2f) - legInset;
            float zPos = bedDepth - (legThickness / 2f) - legInset;

            Transform[] legs = { frontLeftLeg, frontRightLeg, backLeftLeg, backRightLeg };
            Vector3[] positions = {
            new(-xPos, 0, zPos),
            new(xPos, 0, zPos),
            new(-xPos, 0, -zPos),
            new(xPos, 0, -zPos)
        };
            Vector3[] rotations = {
            new(-legSplayAngle, 0, -legSplayAngle),
            new(-legSplayAngle, 0, legSplayAngle),
            new(legSplayAngle, 0, -legSplayAngle),
            new(legSplayAngle, 0, legSplayAngle)
        };

            for (int i = 0; i < 4; i++)
            {
                legs[i].localScale = legScale;
                legs[i].SetLocalPositionAndRotation(positions[i], Quaternion.Euler(rotations[i]));
            }

            if (baseLegMesh == null && frontLeftLeg.TryGetComponent(out MeshFilter mf) && !mf.sharedMesh.name.Contains("Sheared"))
            {
                baseLegMesh = mf.sharedMesh;
            }

            if (baseLegMesh != null)
            {
                float safeFrameThickness = Mathf.Max(0.001f, legThickness);
                float taperRatio = legEndThickness / safeFrameThickness;

                shearedFrontLeft = ProceduralUtility.GenerateUniversalShear(baseLegMesh, new Vector2(legSplayAngle, legSplayAngle), legScale, false, ref shearedFrontLeft, taperRatio);
                shearedFrontRight = ProceduralUtility.GenerateUniversalShear(baseLegMesh, new Vector2(legSplayAngle, -legSplayAngle), legScale, false, ref shearedFrontRight, taperRatio);
                shearedBackLeft = ProceduralUtility.GenerateUniversalShear(baseLegMesh, new Vector2(-legSplayAngle, legSplayAngle), legScale, false, ref shearedBackLeft, taperRatio);
                shearedBackRight = ProceduralUtility.GenerateUniversalShear(baseLegMesh, new Vector2(-legSplayAngle, -legSplayAngle), legScale, false, ref shearedBackRight, taperRatio);

                ProceduralUtility.SetMeshAndCollider(frontLeftLeg, shearedFrontLeft);
                ProceduralUtility.SetMeshAndCollider(frontRightLeg, shearedFrontRight);
                ProceduralUtility.SetMeshAndCollider(backLeftLeg, shearedBackLeft);
                ProceduralUtility.SetMeshAndCollider(backRightLeg, shearedBackRight);
            }
        }

        void SetupPlatform()
        {
            if (platformBase == null) { return; }

            float pWidth = bedWidth - (platformInset * 2f);
            float pLength = bedDepth - (platformInset * 2f);

            platformBase.localScale = new Vector3(pWidth, bedClearanceHeight, pLength);
            platformBase.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);

            if (basePlatformMesh != null)
            {
                ProceduralUtility.SetMeshAndCollider(platformBase, basePlatformMesh);
            }
        }

        void SetupFrame()
        {
            if (frameLeft == null || frameRight == null || frameFront == null || frameBack == null) { return; }

            float yPos = bedClearanceHeight * 2f + (frameHeight / 2f);
            float sideLength = bedDepth;
            float frontWidth = bedWidth - (frameThickness * 2f);
            float xOffset = bedWidth - frameThickness - legThickness;
            float zOffset = bedDepth - frameThickness - legThickness;

            Transform[] frames = { frameLeft, frameRight, frameFront, frameBack };
            Vector3[] scales = {
            new(frameThickness + legThickness, frameHeight, sideLength),
            new(frameThickness + legThickness, frameHeight, sideLength),
            new(frontWidth, frameHeight, frameThickness + legThickness),
            new(frontWidth, frameHeight, frameThickness + legThickness)
        };
            Vector3[] positions = {
            new(-xOffset, yPos, 0),
            new(xOffset, yPos, 0),
            new(0, yPos, zOffset),
            new(0, yPos, -zOffset)
        };

            for (int i = 0; i < 4; i++)
            {
                frames[i].localScale = scales[i];
                frames[i].SetLocalPositionAndRotation(positions[i], Quaternion.identity);

                if (baseFrameMesh != null)
                {
                    ProceduralUtility.SetMeshAndCollider(frames[i], baseFrameMesh);
                }
            }
        }

        void SetupBoard(bool isHeadboard)
        {
            Transform panel = isHeadboard ? headboardPanel : footboardPanel;
            Transform bottomRail = isHeadboard ? headboardBottomRail : null;
            if (panel == null) { return; }

            Transform slatContainer = isHeadboard ? headboardSlatContainer : footboardSlatContainer;
            if (slatContainer != null) { ClearContainer(slatContainer); }

            BoardType type = isHeadboard ? headboardType : footboardType;
            float height = isHeadboard ? headboardHeight : footboardHeight;
            float thickness = isHeadboard ? headboardThickness : footboardThickness;
            float angle = isHeadboard ? laybackAngle : footboardLaybackAngle;
            float lift = isHeadboard ? headboardLift : 0f;

            bool useBottomRail = (type == BoardType.Slat && lift > 0.01f && bottomRail != null);

            float rotAngle = isHeadboard ? -angle : angle;
            float shearAngle = isHeadboard ? angle : -angle;

            float currentTopRailHeight = isHeadboard ? headboardRailHeight : footboardTopRailHeight;
            float currentBottomRailHeight = useBottomRail ? currentTopRailHeight : 0f;
            float currentSlatSpacing = isHeadboard ? headboardSlatSpacing : footboardSlatSpacing;
            float currentSlatThickness = isHeadboard ? headboardSlatThickness : footboardSlatThickness;
            float currentCushionMargin = isHeadboard ? headboardCushionMargin : footboardCushionMargin;
            float currentCushionProjection = isHeadboard ? headboardCushionProjection : footboardCushionProjection;

            float baseZPos = isHeadboard ? (-bedDepth + frameThickness * 2f) : (bedDepth - frameThickness * 2f);
            Vector3 boardBasePos = new(0, bedClearanceHeight * 2f + frameHeight, baseZPos);

            Vector3 liftShift = Quaternion.Euler(rotAngle, 0, 0) * new Vector3(0, lift * 2f, 0);
            Vector3 panelPos = boardBasePos + liftShift / 2f;

            if (baseBoardMesh == null && panel.TryGetComponent(out MeshFilter mf) && !mf.sharedMesh.name.Contains("Sheared"))
            {
                baseBoardMesh = mf.sharedMesh;
            }

            float actualPanelHeight = type == BoardType.Slat ? currentTopRailHeight : height;

            if (type == BoardType.Slat)
            {
                Vector3 upShift = Quaternion.Euler(rotAngle, 0, 0) * new Vector3(0, (height - currentTopRailHeight) * 2f, 0);
                panelPos += upShift;
            }

            Vector3 panelScale = new(bedWidth - legThickness, actualPanelHeight, thickness);
            panel.localScale = panelScale;
            panel.SetLocalPositionAndRotation(panelPos, Quaternion.Euler(rotAngle, 0, 0));

            if (baseBoardMesh != null && type != BoardType.Slat)
            {
                if (isHeadboard)
                {
                    shearedHeadboard = ProceduralUtility.GenerateUniversalShear(baseBoardMesh, new Vector2(shearAngle, 0), panelScale, true, ref shearedHeadboard);
                    ProceduralUtility.SetMeshAndCollider(panel, shearedHeadboard);
                }
                else
                {
                    shearedFootboard = ProceduralUtility.GenerateUniversalShear(baseBoardMesh, new Vector2(shearAngle, 0), panelScale, true, ref shearedFootboard);
                    ProceduralUtility.SetMeshAndCollider(panel, shearedFootboard);
                }
            }

            if (useBottomRail)
            {
                Vector3 bottomRailScale = new(bedWidth - legThickness, currentBottomRailHeight, thickness);
                bottomRail.localScale = bottomRailScale;

                Vector3 bottomRailPos = boardBasePos + liftShift / 2f;
                bottomRail.SetLocalPositionAndRotation(bottomRailPos, Quaternion.Euler(rotAngle, 0, 0));

                if (baseBoardMesh != null)
                {
                    if (isHeadboard)
                    {
                        shearedHeadboardBottom = ProceduralUtility.GenerateUniversalShear(baseBoardMesh, new Vector2(shearAngle, 0), bottomRailScale, true, ref shearedHeadboardBottom);
                        ProceduralUtility.SetMeshAndCollider(bottomRail, shearedHeadboardBottom);
                    }
                }
            }

            GameObject slatPrefab = isHeadboard ? headboardSlatPrefab : footboardSlatPrefab;
            Transform cushion = isHeadboard ? headboardCushion : footboardCushion;

            if (type == BoardType.Slat && slatPrefab != null && slatContainer != null)
            {
                float availableWidth = (bedWidth - legThickness) * 2f - frameThickness;
                int slatCount = Mathf.Max(1, Mathf.FloorToInt(availableWidth / currentSlatSpacing));
                float startX = -(availableWidth / 2f) + (currentSlatSpacing / 2f) + frameThickness;

                float slatHeight = height - currentTopRailHeight - currentBottomRailHeight;
                Vector3 slatScale = new(currentSlatThickness, slatHeight, currentSlatThickness);

                Mesh activeSlatMesh = null;
                if (baseSlatMesh != null)
                {
                    if (isHeadboard)
                    {
                        shearedHeadboardSlat = ProceduralUtility.GenerateUniversalShear(baseSlatMesh, new Vector2(shearAngle, 0), slatScale, true, ref shearedHeadboardSlat);
                        activeSlatMesh = shearedHeadboardSlat;
                    }
                    else
                    {
                        shearedFootboardSlat = ProceduralUtility.GenerateUniversalShear(baseSlatMesh, new Vector2(shearAngle, 0), slatScale, true, ref shearedFootboardSlat);
                        activeSlatMesh = shearedFootboardSlat;
                    }
                }

                Vector3 slatBasePos = boardBasePos + liftShift / 2f;

                if (useBottomRail)
                {
                    Vector3 railShift = Quaternion.Euler(rotAngle, 0, 0) * new Vector3(0, currentBottomRailHeight * 2f, 0);
                    slatBasePos += railShift;
                }

                for (int i = 0; i < slatCount; i++)
                {
                    GameObject slat = Instantiate(slatPrefab, slatContainer);

                    slat.transform.localPosition = new Vector3(startX + (i * currentSlatSpacing), slatBasePos.y, slatBasePos.z);
                    slat.transform.localScale = slatScale;
                    slat.transform.localRotation = Quaternion.Euler(rotAngle, 0, 0);

                    if (activeSlatMesh != null) { ProceduralUtility.SetMeshAndCollider(slat.transform, activeSlatMesh); }
                }
            }
            else if (type == BoardType.Cushion && cushion != null)
            {
                Vector3 cushionScale = new(bedWidth - legThickness - currentCushionMargin, height - currentCushionMargin, (thickness + currentCushionProjection) / 2f);
                cushion.localScale = cushionScale;

                float projDir = isHeadboard ? currentCushionProjection : -currentCushionProjection;
                Vector3 cushionOffset = Quaternion.Euler(rotAngle, 0, 0) * new Vector3(0, currentCushionMargin / 2f, projDir / 2f);

                Vector3 cushionPos = boardBasePos + liftShift + cushionOffset;
                cushion.SetLocalPositionAndRotation(cushionPos, Quaternion.Euler(rotAngle, 0, 0));

                if (baseCushionMesh != null)
                {
                    Mesh shearedCushion = null;
                    shearedCushion = ProceduralUtility.GenerateUniversalShear(baseCushionMesh, new Vector2(shearAngle, 0), cushionScale, true, ref shearedCushion);
                    ProceduralUtility.SetMeshAndCollider(cushion, shearedCushion);
                }
            }

            Transform leftPost = isHeadboard ? leftHeadboardPost : leftFootboardPost;
            Transform rightPost = isHeadboard ? rightHeadboardPost : rightFootboardPost;

            if (leftPost != null && rightPost != null)
            {
                float postHeight = height + lift;
                Vector3 postScale = new(legThickness, postHeight, legThickness);
                float postX = bedWidth - legThickness;

                leftPost.localScale = postScale;
                rightPost.localScale = postScale;

                leftPost.SetLocalPositionAndRotation(new Vector3(-postX, boardBasePos.y, boardBasePos.z), Quaternion.Euler(rotAngle, 0, 0));
                rightPost.SetLocalPositionAndRotation(new Vector3(postX, boardBasePos.y, boardBasePos.z), Quaternion.Euler(rotAngle, 0, 0));

                if (basePostMesh != null)
                {
                    if (isHeadboard)
                    {
                        shearedHeadboardPost = ProceduralUtility.GenerateUniversalShear(basePostMesh, new Vector2(shearAngle, 0), postScale, true, ref shearedHeadboardPost);
                        ProceduralUtility.SetMeshAndCollider(leftPost, shearedHeadboardPost);
                        ProceduralUtility.SetMeshAndCollider(rightPost, shearedHeadboardPost);
                    }
                    else
                    {
                        shearedFootboardPost = ProceduralUtility.GenerateUniversalShear(basePostMesh, new Vector2(shearAngle, 0), postScale, true, ref shearedFootboardPost);
                        ProceduralUtility.SetMeshAndCollider(leftPost, shearedFootboardPost);
                        ProceduralUtility.SetMeshAndCollider(rightPost, shearedFootboardPost);
                    }
                }
            }
        }

        void SetupMattress()
        {
            if (mattress == null) { return; }

            float mWidth = Mathf.Max(0.01f, bedWidth - (frameThickness * 2f) - legThickness);
            float mLength = Mathf.Max(0.01f, bedDepth - (frameThickness * 2f) - legThickness);

            float yPos = bedClearanceHeight * 2f + frameHeight - mattressInset + (mattressThickness / 2f);

            mattress.localScale = new Vector3(mWidth, mattressThickness, mLength);
            mattress.SetLocalPositionAndRotation(new Vector3(0, yPos, 0), Quaternion.identity);

            if (baseMattressMesh != null)
            {
                ProceduralUtility.SetMeshAndCollider(mattress, baseMattressMesh);
            }
        }

        void SetupPillows()
        {
            if (leftPillow == null || rightPillow == null) { return; }

            float mattressTopY = bedClearanceHeight * 2f + frameHeight - mattressInset + mattressThickness;
            float pillowY = mattressTopY + pillowThickness;

            float pillowZ = -bedDepth + frameThickness + pillowDepth + pillowOffset;
            float pillowX = bedWidth / 2.5f;

            Vector3 pScale = new(pillowWidth, pillowThickness, pillowDepth);

            leftPillow.localScale = pScale;
            rightPillow.localScale = pScale;

            leftPillow.SetLocalPositionAndRotation(new Vector3(-pillowX, pillowY, pillowZ), Quaternion.Euler(laybackAngle / 2f, 0, 0));
            rightPillow.SetLocalPositionAndRotation(new Vector3(pillowX, pillowY, pillowZ), Quaternion.Euler(laybackAngle / 2f, 0, 0));

            if (basePillowMesh != null)
            {
                ProceduralUtility.SetMeshAndCollider(leftPillow, basePillowMesh);
                ProceduralUtility.SetMeshAndCollider(rightPillow, basePillowMesh);
            }
        }
        #endregion
    }
}
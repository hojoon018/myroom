using UnityEngine;
using System.Collections.Generic;

namespace OatAotOat.ProceduralDigitalAsset
{
    public enum ShelfStyle { Bookshelf, TVCabinet }
    public enum BookAlignment { Left, Center, Right, Random }

    public class ProceduralShelfGenerator : ProceduralGenerator
    {
        #region --- INSTRUCTOR REFERENCES ---
        [Header("Shelf Style")]
        public ShelfStyle shelfStyle = ShelfStyle.Bookshelf;

        [Header("Model References")]
        public Transform leftPanel;
        public Transform rightPanel, topPanel, bottomPanel, backPanel;
        public Transform outerLeftPanel, outerRightPanel, outerLeftBackPanel, outerRightBackPanel;

        [Header("TV Cabinet Specific")]
        public Transform lowerDivider;
        public Transform upperDivider, upperCabinetBottomDivider;

        [Header("Prefabs & Containers")]
        public GameObject shelfPrefab;
        public Transform shelfContainer, sideShelfContainer;
        public GameObject doorPrefab;
        public Transform doorContainer, upperDoorContainer;

        [Header("Decoration Prefabs")]
        public GameObject bookPrefab;
        public Transform bookContainer;
        public GameObject tvPrefab;
        public Transform tvContainer;

        [Header("Source Geometry")]
        public Mesh basePanelMesh;
        public Mesh baseBookMesh;
        public Mesh baseTvMesh;
        #endregion

        #region --- SETTINGS ---
        [Header("Dimensions")]
        [Range(0.5f, 4f)] public float mainShelfWidth = 1.0f;
        [Range(1f, 3f)] public float wholeShelfHeight = 2.0f;
        [Range(0.2f, 1f)] public float wholeShelfDepth = 0.4f;
        [Range(0.01f, 0.1f)] public float panelThickness = 0.03f;
        public bool hasBackPanel = true;

        [Header("Side Section Settings")]
        public bool hasSideSections = false;
        [Range(0.2f, 2f)] public float sideSectionWidth = 0.5f;
        [Range(1, 10)] public int sideShelfCount = 4;

        [Header("Bookshelf Settings")]
        [Range(1, 10)] public int shelfCount = 4;

        [Header("Book Decoration Settings")]
        public bool hasBooks = true;
        public int randomSeed = 12345;
        public BookAlignment bookAlignment = BookAlignment.Random;
        [Range(0.1f, 1f)] public float bookFillPercentage = 0.7f;

        public Vector2 bookHeightScale = new(0.5f, 0.9f);
        public Vector2 bookDepthScale = new(0.7f, 0.95f);
        public Vector2 bookThicknessScale = new(0.15f, 0.35f);

        [Range(0f, 1f)] public float bookLeanChance = 0.15f;
        [Range(0f, 25f)] public float maxLeanAngle = 15f;
        public List<Color> bookColors = new()
        {
            new Color(0.7f, 0.2f, 0.2f),
            new Color(0.2f, 0.5f, 0.7f),
            new Color(0.3f, 0.6f, 0.3f),
            new Color(0.8f, 0.6f, 0.2f),
            new Color(0.9f, 0.9f, 0.9f),
            new Color(0.2f, 0.2f, 0.2f)
        };

        [Header("TV Cabinet Settings")]
        [Range(0.2f, 1.0f)] public float lowerCabinetHeight = 0.4f;
        [Range(1, 6)] public int doorCount = 3;
        [Range(0.01f, 0.05f)] public float doorSpacing = 0.01f;
        [Range(0.75f, 1.5f)] public float tvAreaHeight = 0.8f;
        [Range(0, 5)] public int upperShelfCount = 1;

        [Header("Upper Cabinet Settings")]
        public bool hasUpperCabinet = true;
        [Range(0.2f, 1.0f)] public float upperCabinetHeight = 0.4f;
        [Range(1, 6)] public int upperDoorCount = 3;

        [Header("TV Decoration Settings")]
        public bool hasTV = true;
        [Range(0.1f, 1f)] public float tvFillScale = 0.85f;
        [Range(0.01f, 1f)] public float tvDepthScale = 0.1f;
        #endregion

        #region --- DATA STRUCTS & CACHES ---
        private IEnumerable<Transform> GetFrameParts()
        {
            return new[] {
            leftPanel, rightPanel, topPanel, bottomPanel, backPanel,
            outerLeftPanel, outerRightPanel, outerLeftBackPanel, outerRightBackPanel,
            lowerDivider, upperDivider, upperCabinetBottomDivider
        };
        }
        private struct BookData
        {
            public float width, height, depth, leanAngle, spaceTaken;
        }
        #endregion

        #region --- BASE CLASS IMPLEMENTATION ---
        protected override void GenerateGeometry()
        {
            foreach (Transform t in new[] { leftPanel, rightPanel, topPanel, bottomPanel })
            {
                if (t == null) { continue; }
                t.gameObject.SetActive(true);
            }

            if (backPanel != null) { backPanel.gameObject.SetActive(hasBackPanel); }

            bool isTVCabinet = shelfStyle == ShelfStyle.TVCabinet;

            if (lowerDivider != null) { lowerDivider.gameObject.SetActive(isTVCabinet); }
            if (upperDivider != null) { upperDivider.gameObject.SetActive(isTVCabinet); }
            if (upperCabinetBottomDivider != null) { upperCabinetBottomDivider.gameObject.SetActive(isTVCabinet && hasUpperCabinet); }

            SetupOuterFrame();

            if (isTVCabinet) { SetupTVCabinet(); }
            else { SetupBookshelf(); }

            SetupSideSections();
        }

        public override void OrganizeHierarchy()
        {
            Transform frameGroup = ProceduralUtility.GetOrCreateGroup(transform, "Frame Components");
            Transform shelfGroup = ProceduralUtility.GetOrCreateGroup(transform, "Shelf Components");
            Transform doorGroup = ProceduralUtility.GetOrCreateGroup(transform, "Door Components");
            Transform decorGroup = ProceduralUtility.GetOrCreateGroup(transform, "Decoration Components");

            ParentToGroup(GetFrameParts(), frameGroup);

            if (shelfContainer != null) { ProceduralUtility.ParentToGroup(shelfContainer, shelfGroup); }
            if (sideShelfContainer != null) { ProceduralUtility.ParentToGroup(sideShelfContainer, shelfGroup); }

            if (doorContainer != null) { ProceduralUtility.ParentToGroup(doorContainer, doorGroup); }
            if (upperDoorContainer != null) { ProceduralUtility.ParentToGroup(upperDoorContainer, doorGroup); }

            if (bookContainer != null) { ProceduralUtility.ParentToGroup(bookContainer, decorGroup); }
            if (tvContainer != null) { ProceduralUtility.ParentToGroup(tvContainer, decorGroup); }
        }

        public override void ApplyColors()
        {
            ApplyColorTo(GetFrameParts(), primaryColor);

            ApplyColorToChildren(shelfContainer, primaryColor);
            ApplyColorToChildren(sideShelfContainer, primaryColor);

            ApplyColorToChildren(doorContainer, secondaryColor);
            ApplyColorToChildren(upperDoorContainer, secondaryColor);

            if (tvContainer != null)
            {
                foreach (Transform tv in tvContainer) { ProceduralUtility.SetColorToPart(tv, Color.black); }
            }

            if (bookContainer != null)
            {
                System.Random colorRng = new(randomSeed);
                foreach (Transform book in bookContainer)
                {
                    Color bColor = secondaryColor;
                    if (bookColors != null && bookColors.Count > 0)
                    {
                        bColor = bookColors[colorRng.Next(bookColors.Count)];
                    }
                    ProceduralUtility.SetColorToPart(book, bColor);
                }
            }
        }

        public override void ApplyBakeMaterials(Material primaryMat, Material secondaryMat)
        {
            ApplyMaterialTo(GetFrameParts(), primaryMat);

            ApplyMaterialToChildren(shelfContainer, secondaryMat);
            ApplyMaterialToChildren(sideShelfContainer, secondaryMat);

            ApplyMaterialToChildren(doorContainer, secondaryMat);
            ApplyMaterialToChildren(upperDoorContainer, secondaryMat);
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

        void ApplyColorToChildren(Transform container, Color color)
        {
            if (container == null) return;
            foreach (Transform child in container) { ProceduralUtility.SetColorToPart(child, color); }
        }

        void ApplyMaterialToChildren(Transform container, Material mat)
        {
            if (container == null) return;
            foreach (Transform child in container) { ProceduralUtility.SetMaterialToPart(child, mat); }
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

        void GenerateDoors(Transform container, float containerCenterY, float availableHeight, int count)
        {
            if (doorPrefab == null || container == null || count <= 0) { return; }

            float innerWidth = mainShelfWidth - (panelThickness * 2f);

            float actualSpacing = doorSpacing;
            float requiredSpace = actualSpacing * (count + 1);
            if (requiredSpace >= innerWidth * 2f)
            {
                actualSpacing = (innerWidth * 1.5f) / (count + 1);
            }

            float doorTotalWidth = innerWidth * 2f - (actualSpacing * (count + 1));
            float singleDoorWidth = doorTotalWidth / count;
            float doorHeight = availableHeight - actualSpacing * 2f;

            float startX = -innerWidth + (singleDoorWidth / 2f) + actualSpacing;
            float doorZ = wholeShelfDepth - (panelThickness / 2f);

            for (int i = 0; i < count; i++)
            {
                GameObject newDoor = Instantiate(doorPrefab, container);
                newDoor.transform.localScale = new Vector3(singleDoorWidth, doorHeight, panelThickness);
                newDoor.transform.SetLocalPositionAndRotation(new Vector3(startX + (i * (singleDoorWidth + actualSpacing)), containerCenterY, doorZ), Quaternion.identity);
            }
        }
        #endregion

        #region --- GEOMETRY SETUP MATH ---
        void SetupOuterFrame()
        {
            float xOffset = mainShelfWidth - panelThickness;
            float yCenter = (wholeShelfHeight - panelThickness) / 2f;
            float panelHeight = (wholeShelfHeight + panelThickness) / 2f;

            if (leftPanel != null)
            {
                leftPanel.localScale = new Vector3(panelThickness, panelHeight, wholeShelfDepth);
                leftPanel.SetLocalPositionAndRotation(new Vector3(-xOffset, yCenter, 0), Quaternion.identity);
                if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(leftPanel, basePanelMesh); }
            }

            if (rightPanel != null)
            {
                rightPanel.localScale = new Vector3(panelThickness, panelHeight, wholeShelfDepth);
                rightPanel.SetLocalPositionAndRotation(new Vector3(xOffset, yCenter, 0), Quaternion.identity);
                if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(rightPanel, basePanelMesh); }
            }

            if (outerLeftPanel != null) { outerLeftPanel.gameObject.SetActive(hasSideSections); }
            if (outerRightPanel != null) { outerRightPanel.gameObject.SetActive(hasSideSections); }

            if (hasSideSections)
            {
                float outerLeftX = -xOffset - sideSectionWidth;
                float outerRightX = xOffset + sideSectionWidth;

                if (outerLeftPanel != null)
                {
                    outerLeftPanel.localScale = new Vector3(panelThickness, panelHeight, wholeShelfDepth);
                    outerLeftPanel.SetLocalPositionAndRotation(new Vector3(outerLeftX, yCenter, 0), Quaternion.identity);
                    if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(outerLeftPanel, basePanelMesh); }
                }

                if (outerRightPanel != null)
                {
                    outerRightPanel.localScale = new Vector3(panelThickness, panelHeight, wholeShelfDepth);
                    outerRightPanel.SetLocalPositionAndRotation(new Vector3(outerRightX, yCenter, 0), Quaternion.identity);
                    if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(outerRightPanel, basePanelMesh); }
                }
            }

            float innerWidth = mainShelfWidth - (panelThickness * 2f);

            if (bottomPanel != null)
            {
                bottomPanel.localScale = new Vector3(innerWidth, panelThickness, wholeShelfDepth);
                bottomPanel.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
                if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(bottomPanel, basePanelMesh); }
            }

            if (topPanel != null)
            {
                topPanel.localScale = new Vector3(innerWidth, panelThickness, wholeShelfDepth);
                topPanel.SetLocalPositionAndRotation(new Vector3(0, wholeShelfHeight - panelThickness, 0), Quaternion.identity);
                if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(topPanel, basePanelMesh); }
            }

            if (backPanel != null && hasBackPanel)
            {
                float backZ = wholeShelfDepth - panelThickness;
                backPanel.localScale = new Vector3(innerWidth, wholeShelfHeight / 2f - (panelThickness * 1.5f), panelThickness);
                backPanel.SetLocalPositionAndRotation(new Vector3(0, yCenter, -backZ), Quaternion.identity);
                if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(backPanel, basePanelMesh); }
            }
        }

        void SetupBookshelf()
        {
            ClearContainer(shelfContainer);
            ClearContainer(bookContainer);
            ClearContainer(tvContainer);
            ClearContainer(upperDoorContainer);
            ClearContainer(doorContainer);

            if (shelfPrefab == null || shelfContainer == null || shelfCount <= 0) { return; }

            float innerHeight = wholeShelfHeight - (panelThickness * 2f);
            float innerWidth = mainShelfWidth - (panelThickness * 2f);

            int safeShelfCount = shelfCount;
            while ((safeShelfCount * panelThickness) >= innerHeight && safeShelfCount > 0)
            {
                safeShelfCount--;
            }
            if (safeShelfCount <= 0) { return; }

            float spacing = (innerHeight - (safeShelfCount * panelThickness)) / (safeShelfCount + 1);

            System.Random rng = new(randomSeed);
            GenerateBooks(0f, innerWidth, spacing / 2f, rng);

            for (int i = 0; i < safeShelfCount; i++)
            {
                GameObject newShelf = Instantiate(shelfPrefab, shelfContainer);
                float yPos = panelThickness + spacing + (i * (spacing + panelThickness)) + (panelThickness / 2f);

                newShelf.transform.localScale = new Vector3(innerWidth, panelThickness, wholeShelfDepth - panelThickness);
                newShelf.transform.SetLocalPositionAndRotation(new Vector3(0, yPos, panelThickness), Quaternion.identity);

                GenerateBooks(yPos, innerWidth, spacing / 2f, rng);
            }
        }

        void GenerateBooks(float shelfYPos, float shelfWidth, float maxShelfHeight, System.Random rng)
        {
            if (!hasBooks || bookPrefab == null || bookContainer == null) { return; }

            float usableWidth = (shelfWidth * 2f) - 0.05f;
            float targetFillWidth = usableWidth * bookFillPercentage;
            float currentFill = 0f;
            float availableDepth = wholeShelfDepth - panelThickness;

            List<BookData> booksToPlace = new();
            int safetyCounter = 0;

            while (currentFill < targetFillWidth)
            {
                safetyCounter++;
                if (safetyCounter > 500) { break; }

                float bH = maxShelfHeight * ((float)rng.NextDouble() * (bookHeightScale.y - bookHeightScale.x) + bookHeightScale.x);
                float bD = availableDepth * ((float)rng.NextDouble() * (bookDepthScale.y - bookDepthScale.x) + bookDepthScale.x);
                float bW = bH * ((float)rng.NextDouble() * (bookThicknessScale.y - bookThicknessScale.x) + bookThicknessScale.x);

                bool leans = rng.NextDouble() < bookLeanChance;
                float leanAngle = 0f;
                if (leans)
                {
                    float sign = rng.NextDouble() > 0.5 ? 1f : -1f;
                    leanAngle = (float)rng.NextDouble() * maxLeanAngle * sign;
                }

                float randomGap = (float)rng.NextDouble() * 0.02f;
                float spaceTaken = bW + randomGap + (leans ? (bH * Mathf.Sin(Mathf.Abs(leanAngle) * Mathf.Deg2Rad)) : 0f);

                if (currentFill + spaceTaken > usableWidth) { break; }

                booksToPlace.Add(new BookData { width = bW, height = bH, depth = bD, leanAngle = leanAngle, spaceTaken = spaceTaken });
                currentFill += spaceTaken;
            }

            float startX = 0;
            float leftBound = -(usableWidth / 2f);
            float rightBound = (usableWidth / 2f) - currentFill;

            switch (bookAlignment)
            {
                case BookAlignment.Left: startX = leftBound; break;
                case BookAlignment.Right: startX = rightBound; break;
                case BookAlignment.Center: startX = -(currentFill / 2f); break;
                case BookAlignment.Random: startX = leftBound + ((float)rng.NextDouble() * (rightBound - leftBound)); break;
            }

            float currentX = startX;
            foreach (BookData b in booksToPlace)
            {
                GameObject newBook = Instantiate(bookPrefab, bookContainer);

                float bY = shelfYPos + (panelThickness / 2f);
                float bZ = (wholeShelfDepth / 2f) - (b.depth / 2f) - panelThickness;

                newBook.transform.localScale = new Vector3(b.width, b.height, b.depth);
                newBook.transform.SetLocalPositionAndRotation(new Vector3(currentX + (b.width / 2f), bY, bZ), Quaternion.Euler(0, 0, b.leanAngle));

                if (baseBookMesh != null) { ProceduralUtility.SetMeshAndCollider(newBook.transform, baseBookMesh); }

                currentX += b.spaceTaken;
            }
        }

        void SetupTVCabinet()
        {
            ClearContainer(shelfContainer);
            ClearContainer(bookContainer);
            ClearContainer(doorContainer);
            ClearContainer(upperDoorContainer);
            ClearContainer(tvContainer);

            float structuralThickness = (panelThickness * 2f) + (panelThickness * 2f) + (hasUpperCabinet ? panelThickness : 0f);
            float maxAvailableForSections = wholeShelfHeight - structuralThickness;

            float safeLowerHeight = lowerCabinetHeight;
            float safeTvHeight = tvAreaHeight;
            float safeUpperHeight = hasUpperCabinet ? upperCabinetHeight : 0f;

            if (maxAvailableForSections <= 0) { return; }

            float totalRequested = safeLowerHeight + safeTvHeight + safeUpperHeight;
            if (totalRequested > maxAvailableForSections)
            {
                float scale = maxAvailableForSections / totalRequested;
                safeLowerHeight *= scale;
                safeTvHeight *= scale;
                safeUpperHeight *= scale;
            }

            float innerWidth = mainShelfWidth - (panelThickness * 2f);

            float lowerDividerY = panelThickness + safeLowerHeight + (panelThickness / 2f);

            if (lowerDivider != null)
            {
                lowerDivider.localScale = new Vector3(innerWidth, panelThickness, wholeShelfDepth - panelThickness);
                lowerDivider.SetLocalPositionAndRotation(new Vector3(0, lowerDividerY, panelThickness), Quaternion.identity);
                if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(lowerDivider, basePanelMesh); }
            }

            float doorY = panelThickness + (safeLowerHeight / 2f);
            GenerateDoors(doorContainer, doorY, safeLowerHeight, doorCount);

            float upperDividerY = lowerDividerY + (panelThickness / 2f) + safeTvHeight + (panelThickness / 2f);

            if (upperDivider != null)
            {
                upperDivider.localScale = new Vector3(innerWidth, panelThickness, wholeShelfDepth - panelThickness);
                upperDivider.SetLocalPositionAndRotation(new Vector3(0, upperDividerY, panelThickness), Quaternion.identity);
                if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(upperDivider, basePanelMesh); }
            }

            SetupTVDecoration(safeLowerHeight, safeTvHeight);

            float topLimitY = wholeShelfHeight - panelThickness;

            if (hasUpperCabinet && upperCabinetBottomDivider != null)
            {
                float upperCabBottomY = topLimitY - safeUpperHeight - (panelThickness / 2f);

                upperCabinetBottomDivider.localScale = new Vector3(innerWidth, panelThickness, wholeShelfDepth - panelThickness);
                upperCabinetBottomDivider.SetLocalPositionAndRotation(new Vector3(0, upperCabBottomY, panelThickness), Quaternion.identity);
                if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(upperCabinetBottomDivider, basePanelMesh); }

                float upperDoorY = upperCabBottomY + (panelThickness / 2f) + (safeUpperHeight / 2f);
                GenerateDoors(upperDoorContainer, upperDoorY, safeUpperHeight, upperDoorCount);

                topLimitY = upperCabBottomY - (panelThickness / 2f);
            }

            if (upperShelfCount > 0 && shelfPrefab != null && shelfContainer != null)
            {
                float remainingHeight = topLimitY - upperDividerY - (panelThickness / 2f);

                if (remainingHeight > 0)
                {
                    float spacing = (remainingHeight - (upperShelfCount * panelThickness)) / (upperShelfCount + 1);

                    for (int i = 0; i < upperShelfCount; i++)
                    {
                        GameObject newShelf = Instantiate(shelfPrefab, shelfContainer);
                        float yPos = upperDividerY + (panelThickness / 2f) + spacing + (i * (spacing + panelThickness)) + (panelThickness / 2f);

                        newShelf.transform.localScale = new Vector3(innerWidth, panelThickness, wholeShelfDepth - panelThickness);
                        newShelf.transform.SetLocalPositionAndRotation(new Vector3(0, yPos, panelThickness), Quaternion.identity);
                    }
                }
            }
        }

        void SetupTVDecoration(float actualLowerHeight, float actualTvHeight)
        {
            if (!hasTV || tvPrefab == null || tvContainer == null) { return; }

            float innerWidth = mainShelfWidth - (panelThickness * 2f);
            float availableDepth = wholeShelfDepth - panelThickness;

            float targetAspect = 16f / 9f;
            float areaAspect = innerWidth / actualTvHeight;

            float maxTvWidth, maxTvHeight;
            if (areaAspect > targetAspect)
            {
                maxTvHeight = actualTvHeight;
                maxTvWidth = maxTvHeight * targetAspect;
            }
            else
            {
                maxTvWidth = innerWidth;
                maxTvHeight = maxTvWidth / targetAspect;
            }

            Vector3 meshSize = Vector3.one;
            if (baseTvMesh != null)
            {
                meshSize = baseTvMesh.bounds.size;
            }
            else if (tvPrefab.TryGetComponent(out MeshFilter mf) && mf.sharedMesh != null)
            {
                meshSize = mf.sharedMesh.bounds.size;
            }

            if (meshSize.x <= 0.001f) { meshSize.x = 1f; }
            if (meshSize.y <= 0.001f) { meshSize.y = 1f; }
            if (meshSize.z <= 0.001f) { meshSize.z = 1f; }

            float tvWidth = (maxTvWidth * tvFillScale) / meshSize.x;
            float tvHeight = (maxTvHeight * tvFillScale) / meshSize.y;
            float tvDepth = (availableDepth * tvDepthScale) / meshSize.z;

            GameObject tvObj = Instantiate(tvPrefab, tvContainer);
            tvObj.transform.localScale = new Vector3(tvWidth, tvHeight, tvDepth);

            float tvY = (panelThickness * 2f) + actualLowerHeight + (actualTvHeight / 2f);
            float tvZ = -wholeShelfDepth + (panelThickness * 2f);

            tvObj.transform.SetLocalPositionAndRotation(new Vector3(0, tvY, tvZ), Quaternion.identity);

            if (baseTvMesh != null) { ProceduralUtility.SetMeshAndCollider(tvObj.transform, baseTvMesh); }
        }

        void SetupSideSections()
        {
            ClearContainer(sideShelfContainer);

            if (outerLeftBackPanel != null) { outerLeftBackPanel.gameObject.SetActive(hasSideSections && hasBackPanel); }
            if (outerRightBackPanel != null) { outerRightBackPanel.gameObject.SetActive(hasSideSections && hasBackPanel); }

            if (!hasSideSections || shelfPrefab == null || sideShelfContainer == null || sideShelfCount <= 0) { return; }

            float innerHeight = wholeShelfHeight - (panelThickness * 2f);
            int safeSideShelfCount = sideShelfCount;
            while ((safeSideShelfCount * panelThickness) >= innerHeight && safeSideShelfCount > 0)
            {
                safeSideShelfCount--;
            }
            if (safeSideShelfCount <= 0) { return; }

            float spacing = (innerHeight - (safeSideShelfCount * panelThickness)) / (safeSideShelfCount + 1);

            float xOffset = mainShelfWidth - panelThickness;
            float sideScaleX = (sideSectionWidth - panelThickness) / 2f;
            float leftCenterX = -xOffset - (sideSectionWidth / 2f);
            float rightCenterX = xOffset + (sideSectionWidth / 2f);

            for (int i = 0; i < safeSideShelfCount + 2; i++)
            {
                float yPos = panelThickness + spacing + ((i - 1) * (spacing + panelThickness));

                float zScale = i == 0 || i == safeSideShelfCount + 1 ? wholeShelfDepth : wholeShelfDepth - panelThickness;
                float zPos = i == 0 || i == safeSideShelfCount + 1 ? 0 : panelThickness;

                GameObject leftShelf = Instantiate(shelfPrefab, sideShelfContainer);
                leftShelf.transform.localScale = new Vector3(sideScaleX, panelThickness, zScale);
                leftShelf.transform.SetLocalPositionAndRotation(new Vector3(leftCenterX, yPos, zPos), Quaternion.identity);

                GameObject rightShelf = Instantiate(shelfPrefab, sideShelfContainer);
                rightShelf.transform.localScale = new Vector3(sideScaleX, panelThickness, zScale);
                rightShelf.transform.SetLocalPositionAndRotation(new Vector3(rightCenterX, yPos, zPos), Quaternion.identity);
            }

            if (!hasSideSections || !hasBackPanel) { return; }

            if (outerLeftBackPanel != null && outerRightBackPanel != null)
            {
                float yCenter = (wholeShelfHeight - panelThickness) / 2f;
                float backZ = wholeShelfDepth - panelThickness;

                outerLeftBackPanel.localScale = new Vector3(sideScaleX, wholeShelfHeight / 2f - (panelThickness * 1.5f), panelThickness);
                outerLeftBackPanel.SetLocalPositionAndRotation(new Vector3(leftCenterX, yCenter, -backZ), Quaternion.identity);
                if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(outerLeftBackPanel, basePanelMesh); }

                outerRightBackPanel.localScale = new Vector3(sideScaleX, wholeShelfHeight / 2f - (panelThickness * 1.5f), panelThickness);
                outerRightBackPanel.SetLocalPositionAndRotation(new Vector3(rightCenterX, yCenter, -backZ), Quaternion.identity);
                if (basePanelMesh != null) { ProceduralUtility.SetMeshAndCollider(outerRightBackPanel, basePanelMesh); }
            }
        }
        #endregion
    }
}

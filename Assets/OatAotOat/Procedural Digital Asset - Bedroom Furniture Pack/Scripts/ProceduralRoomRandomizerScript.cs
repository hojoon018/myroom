using System.Collections.Generic;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    [ExecuteAlways]
    public class ProceduralRoomRandomizer : MonoBehaviour
    {
        [Header("Room Setup")]
        [Range(5f, 8f)] public float roomWidth = 5f;
        [Range(5f, 8f)] public float roomLength = 6f;

        [Header("Randomization")]
        public int layoutSeed = 12345;

        [Header("Architecture")]
        public bool generateWallsAndFloor = true;
        [Range(3f, 5f)] public float wallHeight = 4f;
        [Range(0.05f, 0.2f)] public float wallThickness = 0.1f;

        [Header("Themes")]
        public List<FurnitureTheme> availableThemes;

        [Header("Furniture Prefabs")]
        public GameObject bedPrefab;
        public GameObject tablePrefab;
        public GameObject chairPrefab;
        public GameObject shelfPrefab;
        public GameObject lampPrefab;

        [Header("Generated Content")]
        public Transform roomContainer;

        private struct RoomData
        {
            public FurnitureTheme theme;
            public int layoutType;

            public float bedW, bedL;
            public bool bedFoot;
            public float nsW, nsL, nsH;
            public int nsDrawers;
            public float lampH;
            public bool lampStylePoly;

            public float deskW, deskL;
            public int deskDrawers;
            public bool chairArm, chairRound;

            public bool shelfBookshelf, shelfSide;
            public float shelfH, shelfW, shelfD;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null || roomContainer == null) { return; }
                RestoreArchitectureColors();
            };
        }

        private void RestoreArchitectureColors()
        {
            Transform archGroup = roomContainer.Find("Architecture");
            if (archGroup == null) { return; }

            RoomData data = GenerateData();
            if (data.theme == null) { return; }

            Color floorColor = Color.Lerp(data.theme.primaryColor, Color.black, 0.4f);
            Color wallColor = Color.Lerp(data.theme.secondaryColor, Color.white, 0.4f);

            Transform floor = archGroup.Find("Procedural_Floor");
            if (floor != null) { ProceduralUtility.SetColorToPart(floor, floorColor); }

            Transform backWall = archGroup.Find("Procedural_BackWall");
            if (backWall != null) { ProceduralUtility.SetColorToPart(backWall, wallColor); }

            Transform leftWall = archGroup.Find("Procedural_LeftWall");
            if (leftWall != null) { ProceduralUtility.SetColorToPart(leftWall, wallColor); }

            Transform rightWall = archGroup.Find("Procedural_RightWall");
            if (rightWall != null) { ProceduralUtility.SetColorToPart(rightWall, wallColor); }
        }

        private RoomData GenerateData()
        {
            System.Random rng = new(layoutSeed);
            RoomData d = new();

            if (availableThemes != null && availableThemes.Count > 0)
            {
                d.theme = availableThemes[rng.Next(availableThemes.Count)];
            }

            d.layoutType = rng.Next(3);

            d.bedW = (float)rng.NextDouble() * (1.8f - 1.2f) + 1.2f;
            d.bedL = (float)rng.NextDouble() * (2.1f - 1.9f) + 1.9f;
            d.bedFoot = rng.NextDouble() > 0.5;

            d.nsW = (float)rng.NextDouble() * (0.5f - 0.4f) + 0.4f;
            d.nsL = (float)rng.NextDouble() * (0.5f - 0.4f) + 0.4f;
            d.nsH = (float)rng.NextDouble() * (0.6f - 0.45f) + 0.45f;
            d.nsDrawers = rng.Next(1, 4);

            d.lampH = (float)rng.NextDouble() * (0.45f - 0.3f) + 0.3f;
            d.lampStylePoly = rng.NextDouble() > 0.5;

            d.deskW = (float)rng.NextDouble() * (0.7f - 0.5f) + 0.5f;
            d.deskL = (float)rng.NextDouble() * (1.6f - 1.2f) + 1.2f;
            d.deskDrawers = rng.Next(1, 5);

            d.chairArm = rng.NextDouble() > 0.5;
            d.chairRound = rng.NextDouble() > 0.5;

            d.shelfBookshelf = rng.NextDouble() > 0.5;
            d.shelfH = (float)rng.NextDouble() * (2.2f - 1.8f) + 1.8f;
            d.shelfW = (float)rng.NextDouble() * (1.2f - 0.8f) + 0.8f;
            d.shelfD = (float)rng.NextDouble() * (0.5f - 0.35f) + 0.35f;
            d.shelfSide = rng.NextDouble() > 0.7;

            return d;
        }

        public void GenerateRandomBedroom()
        {
            if (roomContainer != null)
            {
                while (roomContainer.childCount > 0)
                {
                    DestroyImmediate(roomContainer.GetChild(0).gameObject);
                }
            }
            else
            {
                GameObject container = new("Generated_Bedroom_Layout");
                container.transform.SetParent(transform);
                container.transform.localPosition = Vector3.zero;
                roomContainer = container.transform;
            }

            RoomData data = GenerateData();

            if (data.theme == null)
            {
                Debug.LogWarning("ProceduralRoomRandomizer: Please assign at least one Furniture Theme!");
                return;
            }

            GenerateArchitecture(data);
            GenerateBedLayout(data);
            GenerateDeskLayout(data);
            GenerateStorageLayout(data);
        }

        private void GenerateArchitecture(RoomData data)
        {
            if (!generateWallsAndFloor) { return; }

            Transform archGroup = new GameObject("Architecture").transform;
            archGroup.SetParent(roomContainer);
            archGroup.localPosition = Vector3.zero;

            Color floorColor = Color.Lerp(data.theme.primaryColor, Color.black, 0.4f);
            Color wallColor = Color.Lerp(data.theme.secondaryColor, Color.white, 0.4f);

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Procedural_Floor";
            floor.transform.SetParent(archGroup);
            floor.transform.localScale = new Vector3(roomWidth, wallThickness, roomLength);
            floor.transform.localPosition = new Vector3(0, -wallThickness / 2f, 0);
            ProceduralUtility.SetColorToPart(floor.transform, floorColor);

            GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWall.name = "Procedural_BackWall";
            backWall.transform.SetParent(archGroup);
            backWall.transform.localScale = new Vector3(roomWidth + (wallThickness * 2f), wallHeight, wallThickness);
            backWall.transform.localPosition = new Vector3(0, (wallHeight / 2f) - wallThickness, (roomLength / 2f) + (wallThickness / 2f));
            ProceduralUtility.SetColorToPart(backWall.transform, wallColor);

            GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWall.name = "Procedural_LeftWall";
            leftWall.transform.SetParent(archGroup);
            leftWall.transform.localScale = new Vector3(wallThickness, wallHeight, roomLength);
            leftWall.transform.localPosition = new Vector3(-(roomWidth / 2f) - (wallThickness / 2f), (wallHeight / 2f) - wallThickness, 0);
            ProceduralUtility.SetColorToPart(leftWall.transform, wallColor);

            GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightWall.name = "Procedural_RightWall";
            rightWall.transform.SetParent(archGroup);
            rightWall.transform.localScale = new Vector3(wallThickness, wallHeight, roomLength);
            rightWall.transform.localPosition = new Vector3((roomWidth / 2f) + (wallThickness / 2f), (wallHeight / 2f) - wallThickness, 0);
            ProceduralUtility.SetColorToPart(rightWall.transform, wallColor);
        }

        private void GenerateBedLayout(RoomData data)
        {
            if (bedPrefab == null || tablePrefab == null || lampPrefab == null) { return; }

            GameObject bedObj = Instantiate(bedPrefab, roomContainer);
            ProceduralBedGenerator bedGen = bedObj.GetComponent<ProceduralBedGenerator>();
            bedGen.bedWidth = data.bedW;
            bedGen.bedDepth = data.bedL;
            bedGen.hasFootboard = data.bedFoot;

            GameObject nsObj = Instantiate(tablePrefab, roomContainer);
            ProceduralTableGenerator nsGen = nsObj.GetComponent<ProceduralTableGenerator>();
            nsGen.tableWidth = data.nsW;
            nsGen.tableDepth = data.nsL;
            nsGen.tableHeight = data.nsH;
            nsGen.tableShape = TableShape.Rectangular;
            nsGen.hasDrawers = true;
            nsGen.drawerCount = data.nsDrawers;
            nsGen.hasStretchers = false;

            Quaternion bedRot, nsRot;
            Vector3 bedPos, nsPos;

            if (data.layoutType == 0)
            {
                bedPos = new Vector3(0, 0, (roomLength / 2f) - data.bedL);
                bedRot = Quaternion.Euler(0, 180f, 0);
                float nsX = Mathf.Max(-data.bedW - data.nsL - 0.1f, -(roomWidth / 2f) + data.nsL);
                nsPos = new Vector3(nsX, 0, (roomLength / 2f) - data.nsW);
                nsRot = Quaternion.Euler(0, 180f, 0);
            }
            else if (data.layoutType == 1)
            {
                bedPos = new Vector3(-(roomWidth / 2f) + data.bedL, 0, 0);
                bedRot = Quaternion.Euler(0, 90f, 0);
                float nsZ = Mathf.Max(-data.bedW - data.nsL - 0.1f, -(roomLength / 2f) + data.nsW);
                nsPos = new Vector3(-(roomWidth / 2f) + data.nsW, 0, nsZ);
                nsRot = Quaternion.Euler(0, 90f, 0);
            }
            else
            {
                bedPos = new Vector3((roomWidth / 2f) - data.bedL, 0, 0);
                bedRot = Quaternion.Euler(0, -90f, 0);
                float nsZ = Mathf.Min(data.bedW + data.nsL + 0.1f, (roomLength / 2f) - data.nsW);
                nsPos = new Vector3((roomWidth / 2f) - data.nsW, 0, nsZ);
                nsRot = Quaternion.Euler(0, -90f, 0);
            }

            bedObj.transform.SetLocalPositionAndRotation(bedPos, bedRot);
            nsObj.transform.SetLocalPositionAndRotation(nsPos, nsRot);

            data.theme.ApplyThemeTo(bedGen);
            bedGen.ForceGenerate();

            data.theme.ApplyThemeTo(nsGen);
            nsGen.ForceGenerate();

            GameObject lampObj = Instantiate(lampPrefab, roomContainer);
            ProceduralLampGenerator lampGen = lampObj.GetComponent<ProceduralLampGenerator>();
            lampGen.lampHeight = data.lampH;
            lampGen.lampStyle = data.lampStylePoly ? LampStyle.Polygon : LampStyle.TaperedDrum;

            float surfaceY = (nsGen.tableHeight * 2f) + nsGen.topThickness;
            lampObj.transform.SetLocalPositionAndRotation(nsPos + new Vector3(0, surfaceY, 0), Quaternion.identity);

            data.theme.ApplyThemeTo(lampGen);
            lampGen.ForceGenerate();
        }

        private void GenerateDeskLayout(RoomData data)
        {
            if (tablePrefab == null || chairPrefab == null) { return; }

            GameObject deskObj = Instantiate(tablePrefab, roomContainer);
            ProceduralTableGenerator deskGen = deskObj.GetComponent<ProceduralTableGenerator>();
            deskGen.tableWidth = data.deskW;
            deskGen.tableDepth = data.deskL;
            deskGen.tableHeight = 0.75f;
            deskGen.hasDrawers = true;
            deskGen.drawerCount = data.deskDrawers;

            GameObject chairObj = Instantiate(chairPrefab, roomContainer);
            ProceduralChairGenerator chairGen = chairObj.GetComponent<ProceduralChairGenerator>();
            chairGen.hasArmrests = data.chairArm;
            chairGen.chairShape = data.chairRound ? ChairShape.Round : ChairShape.Rectangular;
            chairGen.seatHeight = 0.05f;
            chairGen.legHeight = 0.45f;

            Vector3 deskPos, chairPos;
            Quaternion deskRot, chairRot;

            if (data.layoutType == 0)
            {
                deskPos = new Vector3(-(roomWidth / 2f) + data.deskW, 0, 0);
                deskRot = Quaternion.Euler(0, 90f, 0);
                chairPos = deskPos + new Vector3(data.deskW + 0.3f, 0, 0);
                chairRot = Quaternion.Euler(0, -90f, 0);
                chairPos.x = Mathf.Min(chairPos.x, -data.bedW - 0.45f);
            }
            else if (data.layoutType == 1)
            {
                deskPos = new Vector3((roomWidth / 2f) - data.deskW, 0, 0);
                deskRot = Quaternion.Euler(0, -90f, 0);
                chairPos = deskPos + new Vector3(-data.deskW - 0.3f, 0, 0);
                chairRot = Quaternion.Euler(0, 90f, 0);
                chairPos.x = Mathf.Max(chairPos.x, data.bedW + 0.45f);
            }
            else
            {
                deskPos = new Vector3(0, 0, (roomLength / 2f) - data.deskW);
                deskRot = Quaternion.Euler(0, 180f, 0);
                chairPos = deskPos + new Vector3(0, 0, -data.deskW - 0.3f);
                chairRot = Quaternion.Euler(0, 0, 0);
                chairPos.z = Mathf.Min(chairPos.z, -data.bedW - 0.45f);
            }

            deskObj.transform.SetLocalPositionAndRotation(deskPos, deskRot);
            chairObj.transform.SetLocalPositionAndRotation(chairPos, chairRot);

            data.theme.ApplyThemeTo(deskGen);
            deskGen.ForceGenerate();

            data.theme.ApplyThemeTo(chairGen);
            chairGen.ForceGenerate();
        }

        private void GenerateStorageLayout(RoomData data)
        {
            if (shelfPrefab == null) { return; }

            GameObject shelfObj = Instantiate(shelfPrefab, roomContainer);
            ProceduralShelfGenerator shelfGen = shelfObj.GetComponent<ProceduralShelfGenerator>();
            shelfGen.shelfStyle = data.shelfBookshelf ? ShelfStyle.Bookshelf : ShelfStyle.TVCabinet;
            shelfGen.wholeShelfHeight = data.shelfH;
            shelfGen.mainShelfWidth = data.shelfW;
            shelfGen.wholeShelfDepth = data.shelfD;
            shelfGen.hasSideSections = data.shelfSide;

            Vector3 shelfPos;
            Quaternion shelfRot;

            if (data.layoutType == 0)
            {
                shelfPos = new Vector3((roomWidth / 2f) - data.shelfD, 0, 0); shelfRot = Quaternion.Euler(0, -90f, 0);
            }
            else if (data.layoutType == 1)
            {
                shelfPos = new Vector3(0, 0, (roomLength / 2f) - data.shelfD); shelfRot = Quaternion.Euler(0, 180f, 0);
            }
            else
            {
                shelfPos = new Vector3(-(roomWidth / 2f) + data.shelfD, 0, 0); shelfRot = Quaternion.Euler(0, 90f, 0);
            }

            shelfObj.transform.SetLocalPositionAndRotation(shelfPos, shelfRot);

            data.theme.ApplyThemeTo(shelfGen);
            shelfGen.ForceGenerate();
        }

        #region --- GIZMOS ---
        private void OnDrawGizmos()
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = new Color(0f, 1f, 1f, 0.8f);
            Gizmos.DrawWireCube(new Vector3(0, 0, 0), new Vector3(roomWidth, 0, roomLength));

            if (generateWallsAndFloor)
            {
                Gizmos.color = new Color(1f, 0.9f, 0f, 0.3f);
                Gizmos.DrawWireCube(new Vector3(0, wallHeight / 2f - 0.1f, 0), new Vector3(roomWidth, wallHeight, roomLength));
            }
            Gizmos.matrix = oldMatrix;

            DrawPreviewFootprints();
        }

        private void DrawPreviewFootprints()
        {
            RoomData data = GenerateData();

            Vector3 bedPos, nsPos;
            Quaternion bedRot, nsRot;

            if (data.layoutType == 0)
            {
                bedPos = new Vector3(0, 0, (roomLength / 2f) - data.bedL);
                bedRot = Quaternion.Euler(0, 180f, 0);
                float nsX = Mathf.Max(-data.bedW - data.nsL - 0.1f, -(roomWidth / 2f) + data.nsL);
                nsPos = new Vector3(nsX, 0, (roomLength / 2f) - data.nsW);
                nsRot = Quaternion.Euler(0, 180f, 0);
            }
            else if (data.layoutType == 1)
            {
                bedPos = new Vector3(-(roomWidth / 2f) + data.bedL, 0, 0);
                bedRot = Quaternion.Euler(0, 90f, 0);
                float nsZ = Mathf.Max(-data.bedW - data.nsL - 0.1f, -(roomLength / 2f) + data.nsW);
                nsPos = new Vector3(-(roomWidth / 2f) + data.nsW, 0, nsZ);
                nsRot = Quaternion.Euler(0, 90f, 0);
            }
            else
            {
                bedPos = new Vector3((roomWidth / 2f) - data.bedL, 0, 0);
                bedRot = Quaternion.Euler(0, -90f, 0);
                float nsZ = Mathf.Min(data.bedW + data.nsL + 0.1f, (roomLength / 2f) - data.nsW);
                nsPos = new Vector3((roomWidth / 2f) - data.nsW, 0, nsZ);
                nsRot = Quaternion.Euler(0, -90f, 0);
            }

            Gizmos.color = new Color(0.9f, 0.2f, 0.2f, 0.5f);
            DrawGizmoCube(bedPos, bedRot, new Vector3(data.bedW * 2f, 1.6f, data.bedL * 2f), "Bed");

            Gizmos.color = new Color(0.2f, 0.5f, 0.9f, 0.5f);
            DrawGizmoCube(nsPos, nsRot, new Vector3(data.nsL * 2f, data.nsH * 2f, data.nsW * 2f), "Nightstand");

            Gizmos.color = new Color(0.9f, 0.9f, 0.1f, 0.5f);
            DrawGizmoCube(nsPos + new Vector3(0, (data.nsH * 2f) + 0.03f, 0), Quaternion.identity, new Vector3(0.2f, data.lampH, 0.2f), "Lamp");

            Vector3 deskPos, chairPos;
            Quaternion deskRot, chairRot;

            if (data.layoutType == 0)
            {
                deskPos = new Vector3(-(roomWidth / 2f) + data.deskW, 0, 0);
                deskRot = Quaternion.Euler(0, 90f, 0);
                chairPos = deskPos + new Vector3(data.deskW + 0.3f, 0, 0);
                chairRot = Quaternion.Euler(0, -90f, 0);
                chairPos.x = Mathf.Min(chairPos.x, -data.bedW - 0.45f);
            }
            else if (data.layoutType == 1)
            {
                deskPos = new Vector3((roomWidth / 2f) - data.deskW, 0, 0);
                deskRot = Quaternion.Euler(0, -90f, 0);
                chairPos = deskPos + new Vector3(-data.deskW - 0.3f, 0, 0);
                chairRot = Quaternion.Euler(0, 90f, 0);
                chairPos.x = Mathf.Max(chairPos.x, data.bedW + 0.45f);
            }
            else
            {
                deskPos = new Vector3(0, 0, (roomLength / 2f) - data.deskW);
                deskRot = Quaternion.Euler(0, 180f, 0);
                chairPos = deskPos + new Vector3(0, 0, -data.deskW - 0.3f);
                chairRot = Quaternion.Euler(0, 0, 0);
                chairPos.z = Mathf.Min(chairPos.z, -data.bedW - 0.45f);
            }

            Gizmos.color = new Color(0.2f, 0.5f, 0.9f, 0.5f);
            DrawGizmoCube(deskPos, deskRot, new Vector3(data.deskL * 2f, 1.5f, data.deskW * 2f), "Desk");

            Gizmos.color = new Color(0.9f, 0.6f, 0.1f, 0.5f);
            DrawGizmoCube(chairPos, chairRot, new Vector3(0.9f, 1.45f, 0.9f), "Chair");

            Vector3 shelfPos;
            Quaternion shelfRot;

            if (data.layoutType == 0)
            {
                shelfPos = new Vector3((roomWidth / 2f) - data.shelfD, 0, 0);
                shelfRot = Quaternion.Euler(0, -90f, 0);
            }
            else if (data.layoutType == 1)
            {
                shelfPos = new Vector3(0, 0, (roomLength / 2f) - data.shelfD);
                shelfRot = Quaternion.Euler(0, 180f, 0);
            }
            else
            {
                shelfPos = new Vector3(-(roomWidth / 2f) + data.shelfD, 0, 0);
                shelfRot = Quaternion.Euler(0, 90f, 0);
            }

            Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.5f);
            float totalShelfW = (data.shelfW * 2f) + (data.shelfSide ? 1.0f : 0f);
            DrawGizmoCube(shelfPos, shelfRot, new Vector3(totalShelfW, data.shelfH, data.shelfD * 2f), "Storage");
        }

        private void DrawGizmoCube(Vector3 pos, Quaternion rot, Vector3 size, string label)
        {
            Matrix4x4 oldMat = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.TRS(pos, rot, Vector3.one);

            Gizmos.DrawWireCube(new Vector3(0, size.y / 2f, 0), size);

            Gizmos.matrix = oldMat;

            if (!string.IsNullOrEmpty(label))
            {
                GUIStyle style = new();
                style.normal.textColor = Gizmos.color;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontStyle = FontStyle.Bold;

                Vector3 labelLocalPos = pos + new Vector3(0, size.y + 0.2f, 0);
                Vector3 worldPos = transform.TransformPoint(labelLocalPos);

                UnityEditor.Handles.Label(worldPos, label, style);
            }
        }
        #endregion
#endif
    }
}

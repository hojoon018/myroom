using UnityEngine;
using System.Collections.Generic;

namespace OatAotOat.ProceduralDigitalAsset
{
    public enum LampStyle { Polygon, TaperedDrum, Desk }

    public class ProceduralLampGenerator : ProceduralGenerator
    {
        #region --- INSTRUCTOR REFERENCES ---
        [Header("Lamp Style")]
        public LampStyle lampStyle = LampStyle.Polygon;

        [Header("Model References")]
        public Transform lampBase;
        public Transform pillar;
        public Transform shade;
        public Transform bulb;

        [Header("Desk Lamp References")]
        public Transform lowerArm;
        public Transform upperArm;
        public Transform joint1;
        public Transform joint2;
        public Transform joint3;

        [Header("Source Geometry")]
        public Mesh baseRoundMesh;
        public Mesh basePillarMesh;
        public Mesh baseDrumShadeMesh;
        public Mesh baseDomeShadeMesh;
        public Mesh baseJointMesh;
        public Mesh baseHalfJointMesh;
        public Mesh baseBulbMesh;
        #endregion

        #region --- SETTINGS ---
        [Header("Polygon Settings")]
        [Range(3, 12)] public int polygonSides = 4;
        [Range(0f, 0.5f)] public float polygonRoundness = 0.0f;
        [Range(1, 6)] public int roundArcSegments = 6;
        [Range(0.01f, 0.1f)] public float polygonShadeThickness = 0.02f; // NEW: Shade thickness

        [Header("Base Settings")]
        [Range(0.05f, 0.3f)] public float baseRadius = 0.1f;
        [Range(0.01f, 0.1f)] public float baseThickness = 0.02f;

        [Header("Standard Frame Settings")]
        [Range(0.2f, 0.8f)] public float lampHeight = 0.4f;
        [Range(0.01f, 0.05f)] public float pillarThickness = 0.02f;

        [Header("Desk Lamp Frame Settings")]
        [Range(0.1f, 0.5f)] public float lowerArmLength = 0.25f;
        [Range(0.1f, 0.5f)] public float upperArmLength = 0.25f;
        [Range(0.01f, 0.04f)] public float armThickness = 0.015f;
        [Range(-45f, 45f)] public float lowerArmAngle = 15f;
        [Range(-90f, 90f)] public float upperArmAngle = -45f;
        [Range(-90f, 90f)] public float shadeAngle = -30f;

        [Header("Shade Settings")]
        [Range(0.05f, 0.3f)] public float shadeHeight = 0.15f;
        [Range(0.05f, 0.3f)] public float shadeBottomRadius = 0.12f;
        [Range(0.02f, 0.3f)] public float shadeTopRadius = 0.1f;
        [Range(0f, 0.15f)] public float shadeOffset = 0.05f;

        [Header("Bulb Settings")]
        public bool hasBulb = true;
        [Range(0.02f, 0.1f)] public float bulbSize = 0.04f;

        private Mesh shearedShadeMesh;
        private Mesh generatedPolygonBase;
        private Mesh generatedPolygonShade;
        #endregion

        #region --- CACHED DATA & PROPERTIES ---
        private IEnumerable<Transform> GetFrameParts()
        {
            return new[] { lampBase, pillar, lowerArm, upperArm, joint1, joint2, joint3 };
        }

        private IEnumerable<Transform> GetShadeParts()
        {
            return new[] { shade, bulb };
        }
        #endregion

        #region --- BASE CLASS IMPLEMENTATION ---
        protected override void GenerateGeometry()
        {
            if (lampBase == null || shade == null) { return; }

            bool isDeskLamp = lampStyle == LampStyle.Desk;

            if (pillar != null) { pillar.gameObject.SetActive(!isDeskLamp); }

            foreach (Transform t in new[] { lowerArm, upperArm, joint1, joint2, joint3 })
            {
                if (t == null) { continue; }
                t.gameObject.SetActive(isDeskLamp);
            }

            if (bulb != null) { bulb.gameObject.SetActive(hasBulb); }

            SetupBase();

            if (isDeskLamp)
            {
                SetupDeskLampFrame();
            }
            else
            {
                SetupStandardFrame();
            }
        }

        public override void OrganizeHierarchy()
        {
            Transform frameGroup = ProceduralUtility.GetOrCreateGroup(transform, "Frame Components");
            Transform shadeGroup = ProceduralUtility.GetOrCreateGroup(transform, "Shade Components");

            ParentToGroup(GetFrameParts(), frameGroup);
            ParentToGroup(GetShadeParts(), shadeGroup);
        }

        public override void ApplyColors()
        {
            ApplyColorTo(GetFrameParts(), primaryColor);
            ApplyColorTo(GetShadeParts(), secondaryColor);
        }

        public override void ApplyBakeMaterials(Material primaryMat, Material secondaryMat)
        {
            ApplyMaterialTo(GetFrameParts(), primaryMat);
            ApplyMaterialTo(GetShadeParts(), secondaryMat);
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

        Mesh CreatePrismMesh(int sides, bool isShade, float roundness, float thickness)
        {
            Mesh mesh = new() { name = "Procedural_Polygon_" + sides };
            List<Vector3> verts = new();
            List<int> tris = new();
            List<Vector2> uvs = new();

            float angleStep = 360f / sides;
            float actualRadius = 0.5f / Mathf.Cos((180f / sides) * Mathf.Deg2Rad);

            List<Vector2> profile = new();

            float maxT = actualRadius * Mathf.Sin(Mathf.PI / sides);
            float maxR = maxT * Mathf.Tan(Mathf.PI / sides);
            float r = Mathf.Clamp(roundness, 0f, maxR * 0.95f);

            if (r <= 0.001f)
            {
                for (int i = 0; i < sides; i++)
                {
                    float a = (i * angleStep + (180f / sides)) * Mathf.Deg2Rad;
                    profile.Add(new Vector2(Mathf.Sin(a) * actualRadius, Mathf.Cos(a) * actualRadius));
                }
            }
            else
            {
                float T = r / Mathf.Tan(Mathf.PI / sides);
                float distToCenter = r / Mathf.Sin(Mathf.PI / sides);

                for (int i = 0; i < sides; i++)
                {
                    float a = (i * angleStep + (180f / sides)) * Mathf.Deg2Rad;
                    Vector2 P = new(Mathf.Sin(a) * actualRadius, Mathf.Cos(a) * actualRadius);

                    float aPrev = ((i - 1 + sides) % sides * angleStep + (180f / sides)) * Mathf.Deg2Rad;
                    Vector2 P_prev = new(Mathf.Sin(aPrev) * actualRadius, Mathf.Cos(aPrev) * actualRadius);

                    float aNext = ((i + 1) % sides * angleStep + (180f / sides)) * Mathf.Deg2Rad;
                    Vector2 P_next = new(Mathf.Sin(aNext) * actualRadius, Mathf.Cos(aNext) * actualRadius);

                    Vector2 V_prev = (P_prev - P).normalized;
                    Vector2 V_next = (P_next - P).normalized;

                    Vector2 C = P + (V_prev + V_next).normalized * distToCenter;

                    Vector2 A = P + V_prev * T;
                    Vector2 B = P + V_next * T;

                    float startAngle = Mathf.Atan2(A.y - C.y, A.x - C.x);
                    float endAngle = Mathf.Atan2(B.y - C.y, B.x - C.x);
                    float angleDiff = Mathf.DeltaAngle(startAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg);

                    for (int j = 0; j <= roundArcSegments; j++)
                    {
                        float t = j / (float)roundArcSegments;
                        float currentAngle = startAngle * Mathf.Rad2Deg + angleDiff * t;
                        float rad = currentAngle * Mathf.Deg2Rad;
                        profile.Add(new Vector2(C.x + Mathf.Cos(rad) * r, C.y + Mathf.Sin(rad) * r));
                    }
                }
            }

            float minProfileRadius = float.MaxValue;
            for (int i = 0; i < profile.Count; i++)
            {
                float mag = profile[i].magnitude;
                if (mag < minProfileRadius) minProfileRadius = mag;
            }

            float safeThickness = Mathf.Min(thickness, minProfileRadius * 0.85f);

            List<Vector2> innerProfile = new();
            for (int i = 0; i < profile.Count; i++)
            {
                Vector2 p = profile[i];
                float len = p.magnitude;
                float newLen = Mathf.Max(0.001f, len - safeThickness);
                innerProfile.Add(p.normalized * newLen);
            }

            int ptCount = profile.Count;

            for (int i = 0; i < ptCount; i++)
            {
                Vector2 p1 = profile[i];
                Vector2 p2 = profile[(i + 1) % ptCount];

                Vector3 bl = new(p1.x, 0, p1.y);
                Vector3 br = new(p2.x, 0, p2.y);
                Vector3 tl = new(p1.x, 1, p1.y);
                Vector3 tr = new(p2.x, 1, p2.y);

                int v = verts.Count;
                verts.AddRange(new[] { bl, br, tl, tr });

                float u1 = (float)i / ptCount;
                float u2 = (float)(i + 1) / ptCount;
                uvs.AddRange(new[] { new Vector2(u1, 0), new Vector2(u2, 0), new Vector2(u1, 1), new Vector2(u2, 1) });

                tris.AddRange(new[] { v, v + 1, v + 2, v + 1, v + 3, v + 2 });
            }

            if (isShade)
            {
                for (int i = 0; i < ptCount; i++)
                {
                    Vector2 p1 = innerProfile[i];
                    Vector2 p2 = innerProfile[(i + 1) % ptCount];

                    Vector3 bl = new(p1.x, 0, p1.y);
                    Vector3 br = new(p2.x, 0, p2.y);
                    Vector3 tl = new(p1.x, 1, p1.y);
                    Vector3 tr = new(p2.x, 1, p2.y);

                    int vIn = verts.Count;
                    verts.AddRange(new[] { bl, br, tl, tr });

                    float u1 = (float)i / ptCount;
                    float u2 = (float)(i + 1) / ptCount;
                    uvs.AddRange(new[] { new Vector2(u1, 0), new Vector2(u2, 0), new Vector2(u1, 1), new Vector2(u2, 1) });

                    tris.AddRange(new[] { vIn, vIn + 2, vIn + 1, vIn + 1, vIn + 2, vIn + 3 });
                }

                for (int i = 0; i < ptCount; i++)
                {
                    Vector2 p1Outer = profile[i];
                    Vector2 p2Outer = profile[(i + 1) % ptCount];
                    Vector2 p1Inner = innerProfile[i];
                    Vector2 p2Inner = innerProfile[(i + 1) % ptCount];

                    Vector3 outerL = new(p1Outer.x, 0, p1Outer.y);
                    Vector3 outerR = new(p2Outer.x, 0, p2Outer.y);
                    Vector3 innerL = new(p1Inner.x, 0, p1Inner.y);
                    Vector3 innerR = new(p2Inner.x, 0, p2Inner.y);

                    int vR = verts.Count;
                    verts.AddRange(new[] { outerL, outerR, innerL, innerR });
                    uvs.AddRange(new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) });

                    tris.AddRange(new[] { vR, vR + 2, vR + 1, vR + 1, vR + 2, vR + 3 });
                }
            }

            int tC = verts.Count;
            verts.Add(new Vector3(0, 1, 0));
            uvs.Add(new Vector2(0.5f, 0.5f));
            for (int i = 0; i < ptCount; i++)
            {
                verts.Add(new Vector3(profile[i].x, 1, profile[i].y));
                uvs.Add(new Vector2(profile[i].x / (actualRadius * 2f) + 0.5f, profile[i].y / (actualRadius * 2f) + 0.5f));
            }
            for (int i = 0; i < ptCount; i++)
            {
                tris.Add(tC);
                tris.Add(tC + 1 + i);
                tris.Add(tC + 1 + ((i + 1) % ptCount));
            }

            if (isShade)
            {
                int utC = verts.Count;
                verts.Add(new Vector3(0, 1, 0));
                uvs.Add(new Vector2(0.5f, 0.5f));
                for (int i = 0; i < ptCount; i++)
                {
                    verts.Add(new Vector3(innerProfile[i].x, 1, innerProfile[i].y));
                    uvs.Add(new Vector2(innerProfile[i].x / (actualRadius * 2f) + 0.5f, innerProfile[i].y / (actualRadius * 2f) + 0.5f));
                }
                for (int i = 0; i < ptCount; i++)
                {
                    tris.Add(utC);
                    tris.Add(utC + 1 + ((i + 1) % ptCount));
                    tris.Add(utC + 1 + i);
                }
            }
            else
            {
                int bC = verts.Count;
                verts.Add(new Vector3(0, 0, 0));
                uvs.Add(new Vector2(0.5f, 0.5f));
                for (int i = 0; i < ptCount; i++)
                {
                    verts.Add(new Vector3(profile[i].x, 0, profile[i].y));
                    uvs.Add(new Vector2(profile[i].x / (actualRadius * 2f) + 0.5f, profile[i].y / (actualRadius * 2f) + 0.5f));
                }
                for (int i = 0; i < ptCount; i++)
                {
                    tris.Add(bC);
                    tris.Add(bC + 1 + ((i + 1) % ptCount));
                    tris.Add(bC + 1 + i);
                }
            }

            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
        #endregion

        #region --- GEOMETRY SETUP MATH ---
        void SetupBase()
        {
            float actualWidth = baseRadius * 2f;
            lampBase.localScale = new Vector3(actualWidth, baseThickness, actualWidth);
            lampBase.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            Mesh targetBaseMesh = baseRoundMesh;
            if (lampStyle == LampStyle.Polygon)
            {
                generatedPolygonBase = CreatePrismMesh(polygonSides, false, polygonRoundness, polygonShadeThickness);
                targetBaseMesh = generatedPolygonBase;
            }

            if (targetBaseMesh != null)
            {
                ProceduralUtility.SetMeshAndCollider(lampBase, targetBaseMesh);
            }
        }

        void SetupStandardFrame()
        {
            if (pillar != null)
            {
                float actualPillarHeight = lampHeight - baseThickness;
                pillar.localScale = new Vector3(pillarThickness, actualPillarHeight / 2f, pillarThickness);
                pillar.SetLocalPositionAndRotation(new Vector3(0, baseThickness, 0), Quaternion.identity);

                if (basePillarMesh != null)
                {
                    ProceduralUtility.SetMeshAndCollider(pillar, basePillarMesh);
                }
            }

            float shadeY = lampHeight - shadeOffset;
            float safeBottomRadius = Mathf.Max(0.001f, shadeBottomRadius);

            shade.SetLocalPositionAndRotation(new Vector3(0, shadeY, 0), Quaternion.identity);

            Mesh targetShadeMesh = baseDrumShadeMesh;
            if (lampStyle == LampStyle.Polygon)
            {
                generatedPolygonShade = CreatePrismMesh(polygonSides, true, polygonRoundness, polygonShadeThickness);
                targetShadeMesh = generatedPolygonShade;
            }

            if (targetShadeMesh != null)
            {
                float safeTopRadius = Mathf.Max(0.001f, shadeTopRadius);
                Vector3 topBasedScale = new(safeTopRadius * 2f, shadeHeight, safeTopRadius * 2f);
                float taperRatio = safeBottomRadius / safeTopRadius;

                shade.localScale = topBasedScale;

                shearedShadeMesh = ProceduralUtility.GenerateUniversalShear(targetShadeMesh, Vector2.zero, topBasedScale, false, ref shearedShadeMesh, taperRatio);
                ProceduralUtility.SetMeshAndCollider(shade, shearedShadeMesh);
            }

            if (bulb != null && hasBulb)
            {
                bulb.localScale = new Vector3(bulbSize, bulbSize, bulbSize);
                bulb.SetLocalPositionAndRotation(new Vector3(0, shadeY, 0), Quaternion.identity);

                if (baseBulbMesh != null)
                {
                    ProceduralUtility.SetMeshAndCollider(bulb, baseBulbMesh);
                }
            }
        }

        void SetupDeskLampFrame()
        {
            float safeUpperArmAngle = upperArmAngle;
            if (lowerArmAngle + safeUpperArmAngle > 80f) { safeUpperArmAngle = 80f - lowerArmAngle; }
            if (lowerArmAngle + safeUpperArmAngle < -80f) { safeUpperArmAngle = -80f - lowerArmAngle; }

            Vector3 p0 = new(0, baseThickness + (armThickness / 2f), 0);
            Vector3 dir1 = Quaternion.Euler(lowerArmAngle, 0, 0) * Vector3.up;
            Vector3 p1 = p0 + (dir1 * lowerArmLength) * 2f;

            Vector3 dir2 = Quaternion.Euler(lowerArmAngle + safeUpperArmAngle, 0, 0) * Vector3.up;
            Vector3 p2 = p1 + (dir2 * upperArmLength) * 2f;

            if (lowerArm != null)
            {
                lowerArm.localScale = new Vector3(armThickness, lowerArmLength, armThickness);
                lowerArm.SetLocalPositionAndRotation(p0, Quaternion.Euler(lowerArmAngle, 0, 0));
                if (basePillarMesh != null) { ProceduralUtility.SetMeshAndCollider(lowerArm, basePillarMesh); }
            }

            if (upperArm != null)
            {
                upperArm.localScale = new Vector3(armThickness, upperArmLength, armThickness);
                upperArm.SetLocalPositionAndRotation(p1, Quaternion.Euler(lowerArmAngle + safeUpperArmAngle, 0, 0));
                if (basePillarMesh != null) { ProceduralUtility.SetMeshAndCollider(upperArm, basePillarMesh); }
            }

            if (joint1 != null)
            {
                Vector3 jointScale = new(armThickness * 1.5f, armThickness * 1.5f, armThickness * 1.5f);
                joint1.localScale = jointScale;
                joint1.SetLocalPositionAndRotation(p0, Quaternion.identity);

                if (baseHalfJointMesh != null) { ProceduralUtility.SetMeshAndCollider(joint1, baseHalfJointMesh); }
            }

            if (joint2 != null)
            {
                joint2.localScale = new Vector3(armThickness * 1.5f, armThickness * 1.5f, armThickness * 1.5f);
                joint2.SetLocalPositionAndRotation(p1, Quaternion.identity);
                if (baseJointMesh != null) { ProceduralUtility.SetMeshAndCollider(joint2, baseJointMesh); }
            }

            if (joint3 != null)
            {
                joint3.localScale = new Vector3(armThickness * 1.5f, armThickness * 1.5f, armThickness * 1.5f);
                joint3.SetLocalPositionAndRotation(p2, Quaternion.identity);
                if (baseJointMesh != null) { ProceduralUtility.SetMeshAndCollider(joint3, baseJointMesh); }
            }

            float safeShadeAngle = shadeAngle;
            float cumulativeArmAngle = lowerArmAngle + safeUpperArmAngle;

            if (cumulativeArmAngle + safeShadeAngle > 80f) safeShadeAngle = 80f - cumulativeArmAngle;
            if (cumulativeArmAngle + safeShadeAngle < -80f) safeShadeAngle = -80f - cumulativeArmAngle;

            Vector3 shadeDir = Quaternion.Euler(cumulativeArmAngle + safeShadeAngle, 0, 0) * Vector3.up;
            Vector3 shadePos = p2 + (shadeDir * (shadeHeight + armThickness));
            Quaternion shadeRot = Quaternion.Euler(cumulativeArmAngle + safeShadeAngle, 0, 0);

            if (shade != null)
            {
                shade.localScale = new Vector3(shadeBottomRadius * 2f, shadeHeight, shadeBottomRadius * 2f);
                shade.SetLocalPositionAndRotation(shadePos, shadeRot);

                Mesh targetShadeMesh = baseDomeShadeMesh;
                if (lampStyle == LampStyle.Polygon)
                {
                    generatedPolygonShade = CreatePrismMesh(polygonSides, true, polygonRoundness, polygonShadeThickness);
                    targetShadeMesh = generatedPolygonShade;
                }

                if (targetShadeMesh != null) { ProceduralUtility.SetMeshAndCollider(shade, targetShadeMesh); }
            }

            if (bulb != null && hasBulb)
            {
                Vector3 bulbPos = shadePos - (shadeDir * (shadeHeight * 0.8f));
                bulb.localScale = new Vector3(bulbSize, bulbSize, bulbSize);
                bulb.SetLocalPositionAndRotation(bulbPos, shadeRot);
                if (baseBulbMesh != null) { ProceduralUtility.SetMeshAndCollider(bulb, baseBulbMesh); }
            }
        }
        #endregion
    }
}

using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    public static class ProceduralUtility
    {
        public static void SetMeshAndCollider(Transform part, Mesh mesh)
        {
            if (part == null) { return; }
            if (part.TryGetComponent(out MeshFilter filter)) { filter.sharedMesh = mesh; }
            if (!part.TryGetComponent(out MeshCollider collider))
            {
                collider = part.gameObject.AddComponent<MeshCollider>();
            }
            collider.sharedMesh = mesh;
        }

        public static Mesh GenerateUniversalShear(Mesh source, Vector2 tiltAngles, Vector3 targetScale,
                                                  bool stretchToFloor, ref Mesh targetMesh, float taperRatio = 1f)
        {
            if (source == null) { return null; }

            if (targetMesh == null)
            {
                targetMesh = new Mesh { name = "Universal_Sheared_Mesh" };
            }
            else
            {
                targetMesh.Clear();
            }

            Vector3[] verts = source.vertices;

            float radX = Mathf.Clamp(tiltAngles.x, -85f, 85f) * Mathf.Deg2Rad;
            float tanX = Mathf.Tan(radX);
            float cosX = Mathf.Cos(radX) == 0 ? 0.001f : Mathf.Cos(radX);

            float radZ = Mathf.Clamp(tiltAngles.y, -85f, 85f) * Mathf.Deg2Rad;
            float tanZ = Mathf.Tan(radZ);
            float cosZ = Mathf.Cos(radZ) == 0 ? 0.001f : Mathf.Cos(radZ);

            float targetScaleY = Mathf.Max(0.0001f, targetScale.y);
            float scaleRatioZ = targetScale.z / targetScaleY;
            float scaleRatioX = targetScale.x / targetScaleY;

            float minY = float.MaxValue;
            float maxY = float.MinValue;

            for (int i = 0; i < verts.Length; i++)
            {
                if (verts[i].y < minY) { minY = verts[i].y; }
                if (verts[i].y > maxY) { maxY = verts[i].y; }
            }

            float height = maxY - minY;
            if (height == 0) { height = 0.001f; }

            for (int i = 0; i < verts.Length; i++)
            {
                if (taperRatio != 1f)
                {
                    float heightPercent = (verts[i].y - minY) / height;
                    float currentTaper = Mathf.Lerp(taperRatio, 1f, heightPercent);
                    verts[i].x *= currentTaper;
                    verts[i].z *= currentTaper;
                }

                if (verts[i].y <= minY + 0.05f)
                {
                    float offsetFromBottom = verts[i].y - minY;
                    float baseStretch = stretchToFloor ? (minY / (cosX * cosZ)) : minY;
                    float shearZ = verts[i].z * scaleRatioZ * tanX;
                    float shearX = verts[i].x * scaleRatioX * tanZ;

                    verts[i].y = (baseStretch - shearZ + shearX) + offsetFromBottom;
                }
            }

            targetMesh.vertices = verts;
            targetMesh.triangles = source.triangles;
            targetMesh.normals = source.normals;
            targetMesh.uv = source.uv;
            targetMesh.RecalculateBounds();
            return targetMesh;
        }

        public static void SetColorToPart(Transform part, Color targetColor)
        {
            if (part != null && part.TryGetComponent(out Renderer rend))
            {
                MaterialPropertyBlock propBlock = new();
                rend.GetPropertyBlock(propBlock);
                propBlock.SetColor("_BaseColor", targetColor);
                propBlock.SetColor("_Color", targetColor);
                rend.SetPropertyBlock(propBlock);
            }
        }

        public static void SetMaterialToPart(Transform part, Material mat)
        {
            if (part != null && part.TryGetComponent(out Renderer rend))
            {
                rend.sharedMaterial = mat;
                rend.SetPropertyBlock(null);
            }
        }

        public static Transform GetOrCreateGroup(Transform parent, string groupName)
        {
            Transform group = parent.Find(groupName);
            if (group == null)
            {
                group = new GameObject(groupName).transform;
                group.SetParent(parent);
                group.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                group.localScale = Vector3.one;
            }
            return group;
        }

        public static void ParentToGroup(Transform child, Transform group)
        {
            if (child != null && child.parent != group) { child.SetParent(group); }
        }
    }
}

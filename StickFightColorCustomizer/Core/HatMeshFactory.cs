using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    public static class HatMeshFactory
    {
        private static Mesh _topHatMesh;
        private static Material _hatMaterial;

        public static Mesh GetTopHatMesh()
        {
            if (_topHatMesh != null)
            {
                return _topHatMesh;
            }

            var verts = new System.Collections.Generic.List<Vector3>();
            var tris = new System.Collections.Generic.List<int>();

            AddBox(verts, tris,
                new Vector3(0f, 0.42f, 0f),
                new Vector3(0.22f, 0.28f, 0.08f));

            AddDisc(verts, tris,
                new Vector3(0f, 0.22f, 0f),
                0.38f, 0.06f, 16);

            _topHatMesh = new Mesh();
            _topHatMesh.vertices = verts.ToArray();
            _topHatMesh.triangles = tris.ToArray();
            _topHatMesh.RecalculateNormals();
            _topHatMesh.RecalculateBounds();
            return _topHatMesh;
        }

        public static Material GetHatMaterial()
        {
            if (_hatMaterial != null)
            {
                return _hatMaterial;
            }

            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            _hatMaterial = new Material(shader);
            _hatMaterial.color = Color.white;
            _hatMaterial.renderQueue = 3000;
            return _hatMaterial;
        }

        private static void AddBox(
            System.Collections.Generic.List<Vector3> verts,
            System.Collections.Generic.List<int> tris,
            Vector3 center,
            Vector3 size)
        {
            Vector3 h = size * 0.5f;
            int baseIndex = verts.Count;
            verts.Add(center + new Vector3(-h.x, -h.y, -h.z));
            verts.Add(center + new Vector3(h.x, -h.y, -h.z));
            verts.Add(center + new Vector3(h.x, h.y, -h.z));
            verts.Add(center + new Vector3(-h.x, h.y, -h.z));
            verts.Add(center + new Vector3(-h.x, -h.y, h.z));
            verts.Add(center + new Vector3(h.x, -h.y, h.z));
            verts.Add(center + new Vector3(h.x, h.y, h.z));
            verts.Add(center + new Vector3(-h.x, h.y, h.z));

            AddQuad(tris, baseIndex, 0, 1, 2, 3);
            AddQuad(tris, baseIndex, 4, 7, 6, 5);
            AddQuad(tris, baseIndex, 0, 4, 5, 1);
            AddQuad(tris, baseIndex, 1, 5, 6, 2);
            AddQuad(tris, baseIndex, 2, 6, 7, 3);
            AddQuad(tris, baseIndex, 3, 7, 4, 0);
        }

        private static void AddDisc(
            System.Collections.Generic.List<Vector3> verts,
            System.Collections.Generic.List<int> tris,
            Vector3 center,
            float radius,
            float height,
            int segments)
        {
            int topCenter = verts.Count;
            verts.Add(center + new Vector3(0f, height * 0.5f, 0f));
            int bottomCenter = verts.Count;
            verts.Add(center - new Vector3(0f, height * 0.5f, 0f));

            int ringTopStart = verts.Count;
            for (int i = 0; i < segments; i++)
            {
                float a = (i / (float)segments) * Mathf.PI * 2f;
                verts.Add(center + new Vector3(Mathf.Cos(a) * radius, height * 0.5f, 0f));
            }

            int ringBottomStart = verts.Count;
            for (int i = 0; i < segments; i++)
            {
                float a = (i / (float)segments) * Mathf.PI * 2f;
                verts.Add(center + new Vector3(Mathf.Cos(a) * radius, -height * 0.5f, 0f));
            }

            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;
                int t0 = ringTopStart + i;
                int t1 = ringTopStart + next;
                int b0 = ringBottomStart + i;
                int b1 = ringBottomStart + next;

                tris.Add(topCenter);
                tris.Add(t0);
                tris.Add(t1);

                tris.Add(bottomCenter);
                tris.Add(b1);
                tris.Add(b0);

                tris.Add(t0);
                tris.Add(b0);
                tris.Add(b1);
                tris.Add(t0);
                tris.Add(b1);
                tris.Add(t1);
            }
        }

        private static void AddQuad(
            System.Collections.Generic.List<int> tris,
            int baseIndex,
            int a,
            int b,
            int c,
            int d)
        {
            tris.Add(baseIndex + a);
            tris.Add(baseIndex + b);
            tris.Add(baseIndex + c);
            tris.Add(baseIndex + a);
            tris.Add(baseIndex + c);
            tris.Add(baseIndex + d);
        }
    }
}

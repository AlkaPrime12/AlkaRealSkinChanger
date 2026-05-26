using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Utilidades LineRenderer compatibles con Unity 5.6 (Stick Fight).
    /// </summary>
    public static class StickLineUtil
    {
        public static int GetVertexCount(LineRenderer line)
        {
            if (line == null)
            {
                return 0;
            }

            return line.positionCount;
        }

        public static void SetVertexCount(LineRenderer line, int count)
        {
            if (line == null)
            {
                return;
            }

            line.positionCount = count;
        }

        public static void SetSolidColor(LineRenderer line, Color color)
        {
            if (line == null)
            {
                return;
            }

            line.startColor = color;
            line.endColor = color;
        }

        public static void ConfigureLikeGameLine(LineRenderer line, LineRenderer template, Color color, int sortingOrderOffset)
        {
            if (line == null)
            {
                return;
            }

            if (template != null)
            {
                line.useWorldSpace = template.useWorldSpace;
                line.sortingLayerID = template.sortingLayerID;
                line.sortingOrder = template.sortingOrder + sortingOrderOffset;
                if (template.widthMultiplier > 0.001f)
                {
                    line.widthMultiplier = template.widthMultiplier;
                }

                Material source = template.sharedMaterial != null ? template.sharedMaterial : template.material;
                if (source != null)
                {
                    line.material = new Material(source);
                }
            }

            if (line.material == null)
            {
                MaterialColorHelper.ApplyLineMaterial(line, color);
            }
            else
            {
                MaterialColorHelper.ApplyToMaterial(line.material, color);
            }

            SetSolidColor(line, color);
            line.enabled = true;
        }

        public static Vector3 GetVertexLocal(LineRenderer line, int index)
        {
            if (line == null || index < 0 || index >= GetVertexCount(line))
            {
                return Vector3.zero;
            }

            Vector3 p = line.GetPosition(index);
            return line.useWorldSpace ? line.transform.InverseTransformPoint(p) : p;
        }

        public static Vector3 GetVertexWorld(LineRenderer line, int index)
        {
            if (line == null || index < 0 || index >= GetVertexCount(line))
            {
                return line != null ? line.transform.position : Vector3.zero;
            }

            Vector3 p = line.GetPosition(index);
            return line.useWorldSpace ? p : line.transform.TransformPoint(p);
        }
    }
}

using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Pivot neutro bajo Renderers: pecho en mundo cada frame, rotación identidad, escala 1.
    /// Orbes y otros cosméticos se parentan aquí para seguir al stickman visual.
    /// </summary>
    public static class CosmeticFollowPivot
    {
        public const string PivotObjectName = "SFCC_CosmeticPivot";

        public static Transform EnsurePivot(Controller controller)
        {
            if (controller == null)
            {
                return null;
            }

            Transform renderersRoot = PlayerColorApplier.FindRenderersRoot(controller.transform);
            Transform parent = renderersRoot != null ? renderersRoot : controller.transform;

            Transform existing = parent.Find(PivotObjectName);
            if (existing != null)
            {
                return existing;
            }

            GameObject pivotGo = new GameObject(PivotObjectName);
            pivotGo.transform.SetParent(parent, false);
            pivotGo.transform.localPosition = Vector3.zero;
            pivotGo.transform.localRotation = Quaternion.identity;
            pivotGo.transform.localScale = Vector3.one;
            return pivotGo.transform;
        }

        public static void ApplyPivotWorldPosition(Transform pivot, Vector3 chestWorld)
        {
            if (pivot == null)
            {
                return;
            }

            pivot.position = chestWorld;
            pivot.rotation = Quaternion.identity;
            pivot.localScale = Vector3.one;
        }

        public static bool TryGetChestWorld(LineRenderer spine, Controller controller, out Vector3 chestWorld, out float torsoHeight)
        {
            chestWorld = Vector3.zero;
            torsoHeight = 0.65f;

            if (controller == null)
            {
                return false;
            }

            if (spine != null && StickLineUtil.GetVertexCount(spine) >= 1)
            {
                int low = CosmeticLineAttachUtil.FindLowestAlongDownAxis(spine);
                int high = CosmeticLineAttachUtil.FindHighestAlongDownAxis(spine);
                Vector3 worldLow = StickLineUtil.GetVertexWorld(spine, low);
                Vector3 worldHigh = StickLineUtil.GetVertexWorld(spine, high);
                chestWorld = Vector3.Lerp(worldLow, worldHigh, 0.38f);
                torsoHeight = Mathf.Max(Vector3.Distance(worldLow, worldHigh), 0.35f);
                return true;
            }

            Transform renderersRoot = PlayerColorApplier.FindRenderersRoot(controller.transform);
            Transform spineNode = renderersRoot != null
                ? PlayerColorApplier.FindRendererTransform(renderersRoot, "spineRenderer")
                : null;
            if (spineNode != null)
            {
                chestWorld = spineNode.position + Vector3.up * 0.35f;
                return true;
            }

            chestWorld = controller.transform.position + Vector3.up * 0.35f;
            return true;
        }

        public static void GetCameraOrbitAxes(Camera cam, out Vector3 orbitRight, out Vector3 orbitUp)
        {
            // Stick Fight renders in the Y-Z plane: camera looks along world +X.
            // Screen-horizontal = world Z axis; screen-vertical = world Y axis.
            // Previous code zeroed .z which collapsed the right-vector to (1,0,0),
            // causing orbits to spin in the X-Y (depth × height) plane instead of
            // the visible Y-Z plane. Hard-code the correct axes.
            orbitRight = new Vector3(0f, 0f, 1f);
            orbitUp    = new Vector3(0f, 1f, 0f);
        }

        public static Vector3 GetOrbitOffsetWorld(
            Vector3 orbitRight,
            Vector3 orbitUp,
            float phase,
            float radiusX,
            float radiusY)
        {
            return orbitRight * (Mathf.Cos(phase) * radiusX) + orbitUp * (Mathf.Sin(phase) * radiusY);
        }
    }
}

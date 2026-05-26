using System.Collections.Generic;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Core
{
    /// <summary>Logs de diagnóstico opcionales (desactivados por defecto para no laggear MelonLoader).</summary>
    public static class CosmeticAttachDiagnostics
    {
        /// <summary>Activar solo para depurar attach en desarrollo.</summary>
        public static bool VerboseAttachLogs;

        private static readonly HashSet<int> LoggedRootIds = new HashSet<int>();

        public static void LogSpriteOnce(string tag, GameObject root, LineRenderer line, Controller controller)
        {
            if (!VerboseAttachLogs || root == null)
            {
                return;
            }

            int key = root.GetInstanceID();
            if (LoggedRootIds.Contains(key))
            {
                return;
            }

            LoggedRootIds.Add(key);

            SpriteRenderer sr = root.GetComponent<SpriteRenderer>();
            string spriteInfo = "null";
            string boundsInfo = "n/a";
            if (sr != null && sr.sprite != null)
            {
                Texture2D tex = sr.sprite.texture;
                spriteInfo = (tex != null ? tex.width + "x" + tex.height : "ok");
                Bounds b = sr.sprite.bounds;
                boundsInfo = b.size.ToString();
            }

            int vertCount = line != null ? StickLineUtil.GetVertexCount(line) : 0;
            string verts = "";
            if (line != null && vertCount > 0)
            {
                for (int i = 0; i < vertCount && i < 6; i++)
                {
                    Vector3 w = StickLineUtil.GetVertexWorld(line, i);
                    verts += " v" + i + "=" + w;
                }
            }

            ModLog.Info("[SFCC-" + tag + "] DIAG root=" + root.name
                + " localPos=" + root.transform.localPosition
                + " worldPos=" + root.transform.position
                + " lossyScale=" + root.transform.lossyScale
                + " parent=" + (root.transform.parent != null ? root.transform.parent.name : "NULL")
                + (line != null
                    ? " line=" + line.gameObject.name + " useWorldSpace=" + line.useWorldSpace
                      + " widthMult=" + line.widthMultiplier + verts
                    : "")
                + (controller != null ? " ctrlId=" + controller.playerID : ""));
        }

        public static void LogOrbitSamplePeriodic(
            Controller controller,
            LineRenderer spine,
            Transform pivot,
            Vector3 chestWorld,
            Vector3 offsetWorld,
            Vector3 partLocal,
            Vector3 partWorld,
            float phase) { }

        public static void LogShoeSamplePeriodic(
            string side,
            LineRenderer footLine,
            Transform holder,
            GameObject shoeRoot,
            bool useHolder) { }

        public static void Clear()
        {
            LoggedRootIds.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace StickFightColorCustomizer.Core
{
    /// <summary>
    /// Índice de huesos por Controller (una vez al spawn). Lookups O(1), sin recorrer el árbol cada frame.
    /// </summary>
    public static class BoneIndexCache
    {
        private sealed class ControllerBones
        {
            public Dictionary<string, Transform> ByName;
        }

        private static readonly Dictionary<int, ControllerBones> ByControllerId =
            new Dictionary<int, ControllerBones>();

        public static void Clear()
        {
            ByControllerId.Clear();
        }

        public static void ClearController(Controller controller)
        {
            if (controller == null)
            {
                return;
            }

            ByControllerId.Remove(controller.GetInstanceID());
        }

        public static Transform GetBone(Controller controller, string boneName)
        {
            if (controller == null || string.IsNullOrEmpty(boneName))
            {
                return null;
            }

            ControllerBones entry = GetOrBuild(controller);
            if (entry == null || entry.ByName == null)
            {
                return null;
            }

            Transform bone;
            if (entry.ByName.TryGetValue(boneName, out bone))
            {
                return bone;
            }

            return null;
        }

        private static ControllerBones GetOrBuild(Controller controller)
        {
            int id = controller.GetInstanceID();
            ControllerBones entry;
            if (ByControllerId.TryGetValue(id, out entry) && entry != null && entry.ByName != null)
            {
                return entry;
            }

            entry = BuildIndex(controller.transform);
            ByControllerId[id] = entry;
            return entry;
        }

        private static ControllerBones BuildIndex(Transform root)
        {
            Transform rigid = FindChildByName(root, "Rigidbodies") ?? root;
            Dictionary<string, Transform> byName = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);
            IndexRecursive(rigid, byName);
            return new ControllerBones { ByName = byName };
        }

        private static void IndexRecursive(Transform node, Dictionary<string, Transform> byName)
        {
            if (node == null)
            {
                return;
            }

            if (!byName.ContainsKey(node.name))
            {
                byName[node.name] = node;
            }

            for (int i = 0; i < node.childCount; i++)
            {
                IndexRecursive(node.GetChild(i), byName);
            }
        }

        private static Transform FindChildByName(Transform parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            if (parent.name.Equals(childName, StringComparison.OrdinalIgnoreCase))
            {
                return parent;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform found = FindChildByName(parent.GetChild(i), childName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}

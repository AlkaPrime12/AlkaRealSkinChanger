using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using StickFightColorCustomizer.Models;
using StickFightColorCustomizer.Network;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Core
{
    public static class VanillaSkinBridge
    {
        private static readonly FieldInfo ColorsField = AccessTools.Field(typeof(MultiplayerManager), "m_Colors");

        public static void ApplyPlayerSlotMaterial(Controller controller, BodyColors colors, bool useUniformSkin)
        {
            if (controller == null || colors == null || !LocalPlayerResolver.IsLocalController(controller))
            {
                return;
            }

            Color slotColor = useUniformSkin ? colors.GetBodyUniformColor() : colors.Spine;
            Material slotMat = CreateSlotMaterial(controller, slotColor);
            if (slotMat == null)
            {
                return;
            }

            bool inLobby = MatchmakingHandler.Instance != null && MatchmakingHandler.Instance.IsInsideLobby;
            bool canPatchGlobalSlots = !inLobby
                && !MatchmakingHandler.IsNetworkMatch
                && !HasOtherHumanPlayers(controller);
            if (canPatchGlobalSlots)
            {
                ReplaceSlotInHandler(controller.playerID, slotMat);
                if (ColorCustomizerApp.Instance == null
                    || MultiplayerCompat.AllowNetworkManagerSlotPatch(ColorCustomizerApp.Instance.Config))
                {
                    ReplaceSlotInNetworkManager(controller.playerID, slotMat);
                }
            }

            LineRenderer[] lines = controller.GetComponentsInChildren<LineRenderer>(true);
            foreach (LineRenderer line in lines)
            {
                if (line == null || line.gameObject.name.StartsWith("SFCC_Aura"))
                {
                    continue;
                }

                line.sharedMaterial = slotMat;
                line.startColor = slotColor;
                line.endColor = slotColor;
            }

            SpriteRenderer[] sprites = controller.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sprite in sprites)
            {
                if (sprite == null || sprite.transform.tag == "DontChangeColor")
                {
                    continue;
                }

                sprite.color = slotColor;
                if (sprite.sharedMaterial != null)
                {
                    Material sm = new Material(sprite.sharedMaterial);
                    MaterialColorHelper.ApplyToMaterial(sm, slotColor);
                    sprite.sharedMaterial = sm;
                    sprite.material = sm;
                }
            }

            CharacterInformation info = controller.GetComponent<CharacterInformation>();
            if (info != null)
            {
                info.myMaterial = slotMat;
            }
        }

        private static Material CreateSlotMaterial(Controller controller, Color color)
        {
            Material source = null;
            if (ControllerHandler.Instance != null && ControllerHandler.Instance.colors != null
                && controller.playerID >= 0 && controller.playerID < ControllerHandler.Instance.colors.Length)
            {
                source = ControllerHandler.Instance.colors[controller.playerID];
            }

            if (source == null)
            {
                LineRenderer line = controller.GetComponentInChildren<LineRenderer>();
                source = line != null ? line.sharedMaterial : null;
            }

            Material mat = source != null ? new Material(source) : new Material(Shader.Find("Sprites/Default"));
            MaterialColorHelper.ApplyToMaterial(mat, color);
            return mat;
        }

        private static void ReplaceSlotInHandler(int playerId, Material mat)
        {
            if (ControllerHandler.Instance == null || ControllerHandler.Instance.colors == null)
            {
                return;
            }

            if (playerId >= 0 && playerId < ControllerHandler.Instance.colors.Length)
            {
                ControllerHandler.Instance.colors[playerId] = mat;
            }
        }

        private static bool HasOtherHumanPlayers(Controller localController)
        {
            if (ControllerHandler.Instance == null || localController == null)
            {
                return false;
            }

            List<Controller> active = ControllerHandler.Instance.ActivePlayers;
            if (active == null)
            {
                return false;
            }

            int humans = 0;
            for (int i = 0; i < active.Count; i++)
            {
                Controller c = active[i];
                if (c == null || c.isAI)
                {
                    continue;
                }

                humans++;
                if (humans > 1)
                {
                    return true;
                }
            }

            return false;
        }

        private static void ReplaceSlotInNetworkManager(int playerId, Material mat)
        {
            MultiplayerManager net = GameNetworkCache.GetMultiplayerManager();
            if (net == null || ColorsField == null)
            {
                return;
            }

            Material[] colors = ColorsField.GetValue(net) as Material[];
            if (colors != null && playerId >= 0 && playerId < colors.Length)
            {
                colors[playerId] = mat;
            }
        }
    }
}


using System.Collections.Generic;
using HarmonyLib;
using StickFightColorCustomizer.Core;
using StickFightColorCustomizer.Models;
using StickFightColorCustomizer.Network;
using UnityEngine;
using StickFightColorCustomizer.Hosting;

namespace StickFightColorCustomizer.Patches
{
    [HarmonyPatch(typeof(SetLinePositions), "Update")]
    public static class Patch_SetLinePositions_Update
    {
        private static readonly HashSet<int> LocalSetLineIds = new HashSet<int>();
        private static readonly HashSet<int> NonLocalSetLineIds = new HashSet<int>();

        public static void ClearCache()
        {
            LocalSetLineIds.Clear();
            NonLocalSetLineIds.Clear();
        }

        [HarmonyPostfix]
        public static void Postfix(SetLinePositions __instance)
        {
            if (ColorCustomizerApp.Instance == null || __instance == null)
            {
                return;
            }

            int setLineId = __instance.GetInstanceID();
            string boneName = __instance.gameObject.name;

            if (NonLocalSetLineIds.Contains(setLineId))
            {
                SyncRemoteCosmetics(__instance, boneName);
                return;
            }

            if (!LocalSetLineIds.Contains(setLineId))
            {
                Controller controller = __instance.transform.root.GetComponent<Controller>();
                if (controller == null || !LocalPlayerResolver.IsLocalPlayerFast(controller))
                {
                    NonLocalSetLineIds.Add(setLineId);
                    return;
                }

                LocalSetLineIds.Add(setLineId);
            }

            SyncLocalCosmetics(__instance, boneName);
        }

        private static void SyncRemoteCosmetics(SetLinePositions setLine, string boneName)
        {
            Controller remoteController = setLine.transform.root.GetComponent<Controller>();
            if (remoteController == null)
            {
                return;
            }

            int remoteId = remoteController.GetInstanceID();

            if (ObjectsAttachmentRenderer.HasObjectsForController(remoteId)
                && CosmeticBoneFilter.IsSpineOrTorsoBone(boneName))
            {
                ObjectsAttachmentRenderer.SyncForSetLine(setLine);
            }

            if (ShoeAttachmentRenderer.HasShoesForController(remoteId)
                && CosmeticBoneFilter.IsLegBoneForShoes(boneName))
            {
                ShoeAttachmentRenderer.SyncForSetLine(setLine);
            }

            if (TopsAttachmentRenderer.HasTopsForController(remoteId))
            {
                TopsAttachmentRenderer.SyncForSetLine(setLine);
            }

            if (WingCosmeticRenderer.HasWingsForController(remoteId)
                && CosmeticBoneFilter.IsSpineOrTorsoBone(boneName))
            {
                WingCosmeticRenderer.SyncForSetLine(setLine);
            }
        }

        private static void SyncLocalCosmetics(SetLinePositions setLine, string boneName)
        {
            ColorConfig config = ColorCustomizerApp.Instance.Config;
            if (config == null)
            {
                return;
            }

            bool lobbyMenu = LobbyPerformance.IsLobbyMenuOnly();
            GlowSettings glow = config.Glow;
            bool needGlow = glow != null && glow.Enabled && (!lobbyMenu || glow.MaintainInLobby);
            bool needShoes = ModFeatureGate.IsShoeActive(config);
            bool needTops = ModFeatureGate.IsTopsActive(config);
            bool needObjects = ModFeatureGate.IsObjectsActive(config);
            bool needWings = ModFeatureGate.IsWingActive(config);
            bool needBody = !lobbyMenu && ModFeatureGate.IsBodyActive(config) && config.Colors != null;

            if (!needGlow && !needShoes && !needTops && !needObjects && !needWings && !needBody)
            {
                return;
            }

            if (needGlow)
            {
                bool glowBone =
                    CosmeticBoneFilter.IsSpineOrTorsoBone(boneName)
                    || CosmeticBoneFilter.IsLegBoneForShoes(boneName)
                    || CosmeticBoneFilter.IsHeadBoneForGlow(boneName)
                    || CosmeticBoneFilter.IsArmBoneForGlow(boneName)
                    || string.Equals(boneName, "Wings", System.StringComparison.OrdinalIgnoreCase);

                if (glowBone)
                {
                    GlowSettings glowEff = glow.Style == GlowStyleKind.Solid
                        ? glow
                        : GlowStyleModulator.GetEffective(glow);
                    GlowAuraRenderer.SyncForSetLine(setLine, glowEff, config.Colors);
                }
            }

            if (needShoes && CosmeticBoneFilter.IsLegBoneForShoes(boneName))
            {
                ShoeAttachmentRenderer.SyncForSetLine(setLine);
            }

            if (needObjects && CosmeticBoneFilter.IsSpineOrTorsoBone(boneName))
            {
                ObjectsAttachmentRenderer.SyncForSetLine(setLine);
            }

            if (needBody)
            {
                PlayerColorApplier.MaintainSetLineColor(setLine, config.Colors, config);
            }

            if (needTops)
            {
                TopsAttachmentRenderer.SyncForSetLine(setLine);
            }

            if (needWings && CosmeticBoneFilter.IsSpineOrTorsoBone(boneName))
            {
                WingCosmeticRenderer.SyncForSetLine(setLine);
            }

            if (string.Equals(boneName, "spineRenderer", System.StringComparison.OrdinalIgnoreCase))
            {
                Controller ctrl = setLine.transform.root.GetComponent<Controller>();
                if (ctrl != null)
                {
                    HatAttachmentRenderer.RefreshFacing(ctrl);
                }
            }

            LineMaintainThrottle.ClearForceDirtyIfNeeded();
        }
    }
}

using System.Reflection;
using HarmonyLib;
using StickFightColorCustomizer.Core;
using UnityEngine;

namespace StickFightColorCustomizer.Network
{
    public static class GameNetworkCache
    {
        private static MultiplayerManager _manager;
        private static float _lastLookup;
        private static readonly FieldInfo ConnectedClientsField =
            AccessTools.Field(typeof(MultiplayerManager), "mConnectedClients");
        private static ConnectedClientData[] _cachedClients;
        private static MultiplayerManager _cachedClientsManager;

        public static MultiplayerManager GetMultiplayerManager()
        {
            if (_manager != null && Time.realtimeSinceStartup - _lastLookup < 2f)
            {
                return _manager;
            }

            _lastLookup = Time.realtimeSinceStartup;
            _manager = Object.FindObjectOfType<MultiplayerManager>();
            _cachedClients = null;
            _cachedClientsManager = null;
            return _manager;
        }

        public static ConnectedClientData[] GetConnectedClients(MultiplayerManager manager)
        {
            if (manager == null || ConnectedClientsField == null)
            {
                return null;
            }

            if (_cachedClients != null && _cachedClientsManager == manager)
            {
                return _cachedClients;
            }

            _cachedClients = ConnectedClientsField.GetValue(manager) as ConnectedClientData[];
            _cachedClientsManager = manager;
            return _cachedClients;
        }

        public static void Invalidate()
        {
            _manager = null;
            _cachedClients = null;
            _cachedClientsManager = null;
            LocalPlayerResolver.InvalidateCache();
            PlayerSlotCache.Invalidate();
        }
    }
}

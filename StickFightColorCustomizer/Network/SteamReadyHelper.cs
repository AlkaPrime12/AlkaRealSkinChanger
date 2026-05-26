using System.Reflection;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace StickFightColorCustomizer.Network
{
    /// <summary>
    /// No usar SteamManager.Initialized directamente: su getter crea un SteamManager
    /// nuevo si aún no existe y puede inicializar/cerrar Steam antes que el del juego.
    /// Todas las llamadas a SteamUser/SteamMatchmaking deben pasar por IsReady() o TryGetLocalSteamId().
    /// </summary>
    public static class SteamReadyHelper
    {
        private static FieldInfo _initializedField;

        public static bool IsReady()
        {
            SteamManager manager = Object.FindObjectOfType<SteamManager>();
            if (manager == null)
            {
                return false;
            }

            if (_initializedField == null)
            {
                _initializedField = AccessTools.Field(typeof(SteamManager), "m_bInitialized");
            }

            if (_initializedField == null)
            {
                return false;
            }

            return (bool)_initializedField.GetValue(manager);
        }

        /// <summary>
        /// Obtiene el Steam ID local sin lanzar "Steam is not initialized".
        /// </summary>
        public static bool TryGetLocalSteamId(out ulong steamId)
        {
            steamId = 0;
            if (!IsReady())
            {
                return false;
            }

            try
            {
                steamId = SteamUser.GetSteamID().m_SteamID;
                return steamId != 0;
            }
            catch
            {
                steamId = 0;
                return false;
            }
        }

        public static bool TryGetLocalSteamId(out CSteamID steamId)
        {
            steamId = CSteamID.Nil;
            ulong raw;
            if (!TryGetLocalSteamId(out raw))
            {
                return false;
            }

            steamId = new CSteamID(raw);
            return steamId.IsValid();
        }
    }
}

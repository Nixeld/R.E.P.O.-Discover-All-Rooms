using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DiscoverAllRooms;

[BepInPlugin("Nixeld.DiscoverAllRooms", "DiscoverAllRooms", "1.0")]
public class DiscoverAllRooms : BaseUnityPlugin
{
    internal static DiscoverAllRooms Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    internal static ConfigEntry<bool> ConfigDiscoverAllRooms { get; private set; } = null!;

    private void Awake()
    {
        Instance = this;

        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        ConfigDiscoverAllRooms = Config.Bind(
            "General",
            "DiscoverAllRooms",
            true,
            "When enabled, all rooms are revealed on the map at the start of each level.");

        Patch();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }
}

[HarmonyPatch(typeof(LevelGenerator), nameof(LevelGenerator.GenerateDone))]
internal static class LevelGeneratorDiscoverAllRoomsPatch
{
    private static void Postfix()
    {
        if (!DiscoverAllRooms.ConfigDiscoverAllRooms.Value)
        {
            return;
        }

        RoomVolume[] rooms = Object.FindObjectsOfType<RoomVolume>();
        for (int i = 0; i < rooms.Length; i++)
        {
            rooms[i].SetExplored();
        }
    }
}

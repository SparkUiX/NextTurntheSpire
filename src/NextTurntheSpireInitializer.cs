// using System.Collections.Generic;
// using System.Reflection;
// using HarmonyLib;
// using MegaCrit.Sts2.Core.Localization;
// using MegaCrit.Sts2.Core.Logging;
// using MegaCrit.Sts2.Core.Modding;

// namespace NextTurntheSpire;

// [ModInitializer(nameof(Initialize))]
// public static class NextTurntheSpireInitializer
// {
//     private const string HarmonyId = "NextTurntheSpire.Mod";

//     private static bool _isInitialized;

//     public static void Initialize()
//     {
//         if (_isInitialized)
//         {
//             return;
//         }

//         _isInitialized = true;

//         InstallLocalizationEntries();

//         Harmony harmony = new Harmony(HarmonyId);
//         harmony.PatchAll(Assembly.GetExecutingAssembly());

//         Log.Info("[NextTurntheSpire] Initialized");
//     }

//     private static void InstallLocalizationEntries()
//     {
//         // Runtime-injected text used by the deferred power to display captured card effect text.
//         LocManager.Instance.GetTable("powers").MergeWith(new Dictionary<string, string>
//         {
//             { "NEXT_TURN_DEFERRED_CARD.description", "{CardEffect}" }
//         });
//     }
// }

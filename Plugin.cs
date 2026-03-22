using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
namespace leaderboardPatch
{
    [BepInPlugin("leaderboardpatch", "Leaderboard Patch", "1.0.1.0")]
    [BepInProcess("Neon White.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            Instance = this;
            Logger = base.Logger;

            Logger.LogInfo($"Plugin loaded!");
            
            var harmony = new Harmony("leaderboardpatch");
            harmony.PatchAll(typeof(Patch_UpdateButtons));
            harmony.PatchAll(typeof(Patch_LeftButtonPressed));
            harmony.PatchAll(typeof(Patch_RightButtonPressed));
            harmony.PatchAll(typeof(Patch_LastPageButton));
            harmony.PatchAll(typeof(Patch_FirstPageButton));
            harmony.PatchAll(typeof(Patch_RetrieveLevelName));
            harmony.PatchAll(typeof(Patch_DisplayScores_AsyncRecieve));
        }
        public static class DataTransfer
        {
            public static string levelName = "Movement";
            public static string oldLevelName;
            public static int start = 0;
            public static int difference = 0; 
            public static bool isLastPageButtonPressed = false;
            public static bool isNextPageButtonPressed = false;
            public static bool isPreviousPageButtonPressed = false;
        }
        [HarmonyPatch(typeof(Leaderboards), "DisplayScores_AsyncRecieve")]
        public class Patch_DisplayScores_AsyncRecieve
        {
            static void Prefix(ref ScoreData[] scoreDatas, ref bool atleastOneEntry)
            {
                atleastOneEntry = true;
                DataTransfer.difference = 0;
                
                string filePath = "Data/Leaderboards/" + DataTransfer.levelName + ".txt";
                string[] lines = File.ReadAllLines(filePath);
                if (DataTransfer.isNextPageButtonPressed == true)
                {
                    DataTransfer.start += 10;
                    if (DataTransfer.start >= lines.Length)
                    {
                        DataTransfer.difference = DataTransfer.start - lines.Length;
                        DataTransfer.start -= 10; 
                    }
                    DataTransfer.isNextPageButtonPressed = false;
                }
                if (DataTransfer.isPreviousPageButtonPressed == true)
                {
                    DataTransfer.start -= 10;
                    if (DataTransfer.start < 0)
                    {
                        DataTransfer.start = 0; 
                    }
                    DataTransfer.isPreviousPageButtonPressed = false;
                }
                if (DataTransfer.isLastPageButtonPressed == true)
                {
                    if (lines.Length / 10 * 10 == lines.Length) //Checks if lines.Length is 10, 20, 30...
                    {
                        DataTransfer.start = lines.Length - 10;
                    }
                    else
                    {
                        DataTransfer.start = (lines.Length / 10) * 10;
                        DataTransfer.difference = 10 - (lines.Length - DataTransfer.start); 
                    }
                    DataTransfer.isLastPageButtonPressed = false;
                }
                if (DataTransfer.oldLevelName != DataTransfer.levelName)
                {
                    DataTransfer.start = 0;
                    DataTransfer.difference = 0;
                }
                DataTransfer.oldLevelName = DataTransfer.levelName;
                ScoreData[] injectedLeaderboard = new ScoreData[10 - DataTransfer.difference];
                for (int i = 0; i < injectedLeaderboard.Length; i++)
                {
                    if (i + DataTransfer.start < lines.Length)  //Index out of range check
                    {
                        string line = lines[i + DataTransfer.start];
                        string[] userData = line.Split(',');
                        double timeInSeconds = double.Parse(userData[1]);
                        string username = userData[0];
                        double timeInMs = timeInSeconds * 1000;
                        int ranking = i + 1 + DataTransfer.start;

                        injectedLeaderboard[i] = new ScoreData(
                            ranking: ranking,
                            oldRanking: ranking,
                            profilePicture: null,  //TODO: Add user's country flag as image
                            username: username,
                            scoreValueInMilliseconds: (int)timeInMs,
                            medalValue: 0,  //Medal score(1=bronze, 2=silver, 3=gold...). 0 means no medal
                            userScore: false,
                            numLevelsCompleted: -1,
                            found: true
                        );
                        
                    }
                }
                //Inject new leaderboard
                scoreDatas = injectedLeaderboard; 
            }
        }
        [HarmonyPatch(typeof(LeaderboardIntegrationSteam), "OnLoadComplete2")]
        public class Patch_RetrieveLevelName
        {
            static void Postfix(bool result, bool offline, bool cheater)
            {
                
                var field = AccessTools.Field(typeof(LeaderboardIntegrationSteam), "currentLevelData");

                if (field == null)
                    return;
                var levelData = field.GetValue(null); 

                if (levelData == null)
                    return;

                var nameField = AccessTools.Field(levelData.GetType(), "levelDisplayName");

                if (nameField == null)
                    return;

                DataTransfer.levelName = (string)nameField.GetValue(levelData);
                Logger.LogInfo("DataTransfer.LevelName: " + DataTransfer.levelName);
                Logger.LogInfo("Level Name from namefield: " + (string)nameField.GetValue(levelData));
            }
        }
        [HarmonyPatch(typeof(Leaderboards), "UpdateButtons")]
        public class Patch_UpdateButtons
        {   
            static void Postfix(Leaderboards __instance)
            {
                __instance.endButton.interactable = true;
		        __instance.startButton.interactable = true;
		        __instance.leftArrowButton.interactable = true;
		        __instance.rightArrowButton.interactable = true;
            }
        }
        [HarmonyPatch(typeof(Leaderboards), "OnLeftArrowPressed")]
        public class Patch_LeftButtonPressed
        {
            static void Postfix()
            {
                DataTransfer.isPreviousPageButtonPressed = true;
                Logger.LogInfo("Previous button pressed!");
            }
        }
        [HarmonyPatch(typeof(Leaderboards), "OnRightArrowPressed")]
        public class Patch_RightButtonPressed
        {
            static void Postfix()
            {
                DataTransfer.isNextPageButtonPressed = true;
                Logger.LogInfo("Next button pressed!");
            }
            
        }
        [HarmonyPatch(typeof(Leaderboards), "OnLastPageButtonPressed")]
        public class Patch_LastPageButton
        {
            static void Postfix()
            {
                DataTransfer.isLastPageButtonPressed = true;
                Logger.LogInfo("Last page button pressed!");
            }
            
        }
        [HarmonyPatch(typeof(Leaderboards), "OnFirstPageButtonPressed")]
        public class Patch_FirstPageButton
        {
            static void Postfix()
            {
                DataTransfer.start = 0;
                Logger.LogInfo("First page button pressed!");
            }
            
        }
    }
}

using System.Diagnostics;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.UIElements;
namespace leaderboardPatch
{
    [BepInPlugin("leaderboardpatch", "Leaderboard Patch", "0.2.1.0")]
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
            harmony.PatchAll(typeof(Patch_RetrieveLevelName));
            harmony.PatchAll(typeof(Patch_DisplayScores_AsyncRecieve));
        }
        public static class DataTransfer
        {
            public static string LevelName;
            public static string oldLevelName = "";
            public static int start = 0;
            public static int difference = 0; 
        }
        [HarmonyPatch(typeof(Leaderboards), "DisplayScores_AsyncRecieve")]
        public class Patch_DisplayScores_AsyncRecieve
        {
            static void Prefix(ref ScoreData[] scoreDatas, ref bool atleastOneEntry)
            {
                atleastOneEntry = true;
                DataTransfer.difference = 0;
                string filePath = "Data/Leaderboards/" + DataTransfer.LevelName + ".txt";
                string[] lines = File.ReadAllLines(filePath);
                if (DataTransfer.LevelName == DataTransfer.oldLevelName)
                {
                    if (DataTransfer.start + 10 > lines.Length)
                    {
                        DataTransfer.difference = 10 - (lines.Length - DataTransfer.start);  
                    }
                    else
                        DataTransfer.start = DataTransfer.start + 10;
                    Logger.LogInfo(DataTransfer.start);
                    Logger.LogInfo("Same Name!");
                }
                else
                {
                    DataTransfer.start = 0;
                    Logger.LogInfo(DataTransfer.start);
                }
                DataTransfer.oldLevelName = DataTransfer.LevelName;
                ScoreData[] injectedLeaderboard = new ScoreData[(10 - DataTransfer.difference)];
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
                        // Pass injected leaderboard to overwrite the old one
                        scoreDatas = injectedLeaderboard; 
                    }
                }
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

                DataTransfer.LevelName = (string)nameField.GetValue(levelData);
            }
        }
    }
}
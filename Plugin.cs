using System.IO;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
namespace leaderboardPatch
{
    [BepInPlugin("leaderboardpatch", "Leaderboard Patch", "1.2.0.0")]
    [BepInProcess("Neon White.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        internal static new ManualLogSource Logger;
        private ConfigEntry<string> configUsername;
        private ConfigEntry<string> configCountry;

        private void Awake()
        {
            configUsername = Config.Bind("General", "Username", "Placeholder", "Username of the player"); 
            configCountry = Config.Bind("General", "Country", "US", "Flag of the player(Format: US, GB, CA)");
            configUsername.Value = configUsername.Value.Replace(" ", "");
            configCountry.Value = configCountry.Value.Replace(" ", "");
            Instance = this;
            Logger = base.Logger;

            Logger.LogInfo($"Plugin loaded!");
            
            var harmony = new Harmony("leaderboardpatch");
            harmony.PatchAll(typeof(Patch_GetTimeBestMicroseconds));
            harmony.PatchAll(typeof(Patch_GetLevelDisplayName));
            harmony.PatchAll(typeof(Patch_UpdateButtons));
            harmony.PatchAll(typeof(Patch_UserButton));
            harmony.PatchAll(typeof(Patch_LeftButtonPressed));
            harmony.PatchAll(typeof(Patch_RightButtonPressed));
            harmony.PatchAll(typeof(Patch_LastPageButton));
            harmony.PatchAll(typeof(Patch_FirstPageButton));
            harmony.PatchAll(typeof(Patch_DisplayScores_AsyncRecieve));
        }
        public static class DataTransfer
        {
            public static string levelName = "TUT_MOVEMENT";
            public static string oldLevelName;
            public static int start = 0;
            public static int difference = 0; 
            public static bool isLastPageButtonPressed = false;
            public static bool isNextPageButtonPressed = false;
            public static bool isPreviousPageButtonPressed = false;
            public static bool isUserButtonPressed = false;
            public static double userTime = 0;
            public static bool isMyOwnScore = false;
            public static bool isMyScoreInserted = false;
        }
        [HarmonyPatch(typeof(Leaderboards), "DisplayScores_AsyncRecieve")]
        public class Patch_DisplayScores_AsyncRecieve
        {
            static void Prefix(ref ScoreData[] scoreDatas, ref bool atleastOneEntry)
            {
                atleastOneEntry = true;
                int j = 0;
                DataTransfer.difference = 0;
                
                string leaderboardPath = Path.Combine("Data/Leaderboards/" + DataTransfer.levelName + ".txt");
                if (File.Exists(leaderboardPath) == false)
                {
                    leaderboardPath = "Data/Leaderboards/fallback.txt";
                    Logger.LogInfo("Leaderboard not found, falling back");
                }
                string[] lines = File.ReadAllLines(leaderboardPath);
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
                        DataTransfer.start = lines.Length / 10 * 10;
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
                if (DataTransfer.isUserButtonPressed == true)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] userData = lines[i].Split(',');
                        if (DataTransfer.userTime / 1000000 <= double.Parse(userData[1])) 
                        {
                            DataTransfer.start = lines.Length / 10 * 10;
                            if (DataTransfer.start == lines.Length) //Checks if lines.Length is 10, 20, 30...
                            {
                                DataTransfer.start -= 10;
                            }
                            else
                            {
                                if (DataTransfer.start + 10 > lines.Length)
                                {
                                    DataTransfer.difference = 10 - (lines.Length - DataTransfer.start);
                                
                                }
                            }
                        }
                    }
                    DataTransfer.isUserButtonPressed = false;
                }
                DataTransfer.isMyScoreInserted = false;
                ScoreData[] injectedLeaderboard = new ScoreData[10 - DataTransfer.difference];
                for (int i = 0; i < injectedLeaderboard.Length; i++)
                {
                    if (i + DataTransfer.start < lines.Length)  //Index out of range check
                    {
                        string line = lines[j + DataTransfer.start];
                        string[] userData = line.Split(',');
                        double timeInSeconds = double.Parse(userData[1]);
                        string username = userData[0];
                        string countryPath = "Data/Flags/" + userData[2] + ".png";
                        Logger.LogInfo(countryPath);
                        
                        double timeInMs = timeInSeconds * 1000;
                        
                        if ((DataTransfer.userTime / 1000 <= timeInMs) && (DataTransfer.isMyScoreInserted == false))
                        {
                            username = Instance.configUsername.Value;
                            timeInMs = DataTransfer.userTime / 1000;
                            countryPath = "Data/Flags/" + Instance.configCountry.Value + ".png";
                            DataTransfer.isMyOwnScore = true;
                            DataTransfer.isMyScoreInserted = true;
                            i--;
                            j--;
                        }
                        int ranking = i + 1 + DataTransfer.start;
                        Texture2D profilePicture = new Texture2D(64, 64);
                        if (File.Exists(countryPath) == false)
                        {
                            profilePicture = null;
                            Logger.LogWarning("Flag does not exist, falling back");
                        }
                        byte[] profilePictureBytes = File.ReadAllBytes(countryPath);
                        if (profilePicture.LoadImage(profilePictureBytes) == false)
                        {
                            profilePicture = null;
                            Logger.LogError("No texture image");
                        }
                        injectedLeaderboard[i] = new ScoreData(
                            ranking: ranking,
                            oldRanking: ranking,
                            profilePicture: profilePicture,  
                            username: username,
                            scoreValueInMilliseconds: (int)timeInMs,
                            medalValue: 0,  //Medal score(1=bronze, 2=silver, 3=gold...). 0 means no medal. TODO: Add medal
                            userScore: DataTransfer.isMyOwnScore,
                            numLevelsCompleted: -1,
                            found: true
                        );
                        DataTransfer.isMyOwnScore = false;
                        j++;
                    }
                }
                //Inject new leaderboard
                scoreDatas = injectedLeaderboard; 
            }
        }
        [HarmonyPatch(typeof(Leaderboards), "UpdateButtons")]
        public class Patch_UpdateButtons
        {   
            static void Postfix(Leaderboards __instance)
            {
                __instance.endButton.interactable = true;
                __instance.userButton.interactable = true;
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
        [HarmonyPatch(typeof(Leaderboards), "OnUserButtonPressed")]
        public class Patch_UserButton
        {
            static void Postfix()
            {
                DataTransfer.isUserButtonPressed = true;
                Logger.LogInfo("My score button pressed!");
            }
            
        }
        [HarmonyPatch(typeof(LevelData), "GetLevelDisplayName")]
        public class Patch_GetLevelDisplayName
        {
            static void Postfix(LevelData __instance)
            {
                Logger.LogInfo("Level id:" + __instance.levelID.Replace(" ", ""));
                DataTransfer.levelName = __instance.levelID.Replace(" ", "");
            }
        }
        
        /*[HarmonyPatch(typeof(Leaderboards), "SetUserRanking")]
        public class Patch_SetUserRanking
        {
            static void Prefix(ref int newRanking)
            {
                newRanking = 1;
                Logger.LogInfo("User ranking:" + newRanking);
            }
        }
        */ 
        
        [HarmonyPatch(typeof(LevelStats), "GetTimeBestMicroseconds")]
        public class Patch_GetTimeBestMicroseconds
        {
            static void Postfix(LevelStats __instance)
            {
                DataTransfer.userTime = __instance._timeBestMicroseconds;
            }
        }
    }
}
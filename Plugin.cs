using System.IO;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System;
using Steamworks;

namespace leaderboardPatch
{
    [BepInPlugin("leaderboardpatch", "Leaderboard Patch", "1.3.0.0")]
    [BepInProcess("Neon White.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin pluginInstance { get; private set; }
        public static MenuScreenResults menuScreenResultsInstance { get; private set; }
        internal static new ManualLogSource Logger;
        private ConfigEntry<string> configUsername;
        private ConfigEntry<string> configCountry;

        private void Awake()
        {
            configUsername = Config.Bind("General", "Username", "Placeholder", "Username of the player"); 
            configCountry = Config.Bind("General", "Country", "US", "Flag of the player(Format: US, GB, CA)");
            configUsername.Value = configUsername.Value.Replace(" ", "");
            configCountry.Value = configCountry.Value.Replace(" ", "");
            pluginInstance = this;

            Logger = base.Logger;

            Logger.LogInfo($"Plugin loaded!");
            
            var harmony = new Harmony("leaderboardpatch");
            harmony.PatchAll(typeof(Patch_UpdateButtons));
            harmony.PatchAll(typeof(Patch_LeftButtonPressed));
            harmony.PatchAll(typeof(Patch_RightButtonPressed));
            harmony.PatchAll(typeof(Patch_LastPageButton));
            harmony.PatchAll(typeof(Patch_FirstPageButton));
            harmony.PatchAll(typeof(Patch_UserButton));
            harmony.PatchAll(typeof(Patch_GetTimeBestMicroseconds));
            harmony.PatchAll(typeof(Patch_GetLevelDisplayName));
            harmony.PatchAll(typeof(Patch_GetMedalsTimes));
            harmony.PatchAll(typeof(Patch_SetScore));
            harmony.PatchAll(typeof(Patch_SkipSteamFrontend));
            harmony.PatchAll(typeof(Patch_SkipLeaderboardDownload));   
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
            public static double userTime = 0.0;
            public static bool isMyOwnScore = false;
            public static bool isMyScoreInserted = false;
            public static double timeSilver = 0;
            public static double timeGold = 0;
            public static double timeAce = 0;
            public static double timeDev = 0;
            public static bool isDevMedal = false;

            
        }
        [HarmonyPatch(typeof(Leaderboards), "DisplayScores_AsyncRecieve")]
        public class Patch_DisplayScores_AsyncRecieve
        {
            
            static void Prefix(ref ScoreData[] scoreDatas, ref bool atleastOneEntry)
            {
                atleastOneEntry = true;
                int j = 0;
                int medal;
                DataTransfer.difference = 0;
                
                string leaderboardPath = Path.Combine("Data/Leaderboards/" + DataTransfer.levelName + ".txt");
                if (File.Exists(leaderboardPath) == false)
                {
                    leaderboardPath = "Data/Leaderboards/fallback.txt";
                    Logger.LogWarning("Leaderboard not found, falling back");
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
                    if (lines.Length / 10 * 10 == lines.Length) 
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
                    DataTransfer.start = lines.Length / 10 * 10;
                    if (DataTransfer.start == lines.Length)
                    {
                        DataTransfer.start -= 10;
                    }
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] userData = lines[i].Split(',');
                        if (DataTransfer.userTime / 1000000.0 <= double.Parse(userData[1])) 
                        {
                            if (DataTransfer.start + 10 > lines.Length)
                            {
                                DataTransfer.difference = 10 - (lines.Length - DataTransfer.start);
                            }
                        }
                    }
                    DataTransfer.isUserButtonPressed = false;
                }
                DataTransfer.isMyScoreInserted = false;
                ScoreData[] injectedLeaderboard = new ScoreData[10 - DataTransfer.difference];
                
                for (int i = 0; i < injectedLeaderboard.Length; i++)
                {
                    if (i + DataTransfer.start < lines.Length)
                    {
                        Logger.LogInfo("[DEBUG] " + DataTransfer.userTime / 1000000.0);
                        string line = lines[j + DataTransfer.start];
                        string[] userData = line.Split(',');
                        double timeInSeconds = double.Parse(userData[1]);
                        string username = userData[0];
                        double timeInMs = timeInSeconds * 1000;
                        Texture2D profilePicture = new Texture2D(64, 64);
                        Texture2D profilePictureUser = new Texture2D(64, 64);
                        string countryPath = "Data/Flags/" + userData[2] + ".png";
                        DataTransfer.isDevMedal = timeInSeconds <= DataTransfer.timeDev;

                        medal = CalcMedal(DataTransfer.timeSilver, DataTransfer.timeGold, DataTransfer.timeAce, timeInSeconds);
                        
                        if ((DataTransfer.userTime / 1000.0 <= timeInMs) && (DataTransfer.isMyScoreInserted == false))
                        {
                            medal = CalcMedal(DataTransfer.timeSilver, DataTransfer.timeGold, DataTransfer.timeAce, DataTransfer.userTime / 1000000.0);
                            username = pluginInstance.configUsername.Value;
                            timeInMs = DataTransfer.userTime / 1000.0;
                            countryPath = "Data/Flags/" + pluginInstance.configCountry.Value + ".png";
                            DataTransfer.isMyOwnScore = true;
                            DataTransfer.isMyScoreInserted = true;
                            i--;
                            j--;
                        }
                        
                        int ranking = i + 1 + DataTransfer.start;
                        if (File.Exists(countryPath) == false)
                        {
                            countryPath = null;
                            Logger.LogWarning("Flag does not exist, falling back");
                        }
                        byte[] profilePictureBytes = File.ReadAllBytes(countryPath);
                        if (profilePicture.LoadImage(profilePictureBytes) == false)
                        {
                            profilePicture = null;
                            Logger.LogWarning("No texture image");
                        }
                        injectedLeaderboard[i] = new ScoreData(
                            ranking: ranking,
                            oldRanking: ranking,
                            profilePicture: profilePicture,  
                            username: username,
                            scoreValueInMilliseconds: (int)timeInMs,
                            medalValue: medal,  
                            userScore: DataTransfer.isMyOwnScore,
                            numLevelsCompleted: -1,
                            found: true
                        );
                        DataTransfer.isMyOwnScore = false;
                        j++;
                        if ((i + DataTransfer.start + 1 == lines.Length) && (DataTransfer.isMyScoreInserted == false))
                        {
                            Logger.LogInfo("Last entry, inserting user score");
                            if (DataTransfer.start / 10 * 10 < lines.Length)
                            {
                                if (lines.Length + 1 < DataTransfer.start + 10)
                                {
                                    medal = CalcMedal(DataTransfer.timeSilver, DataTransfer.timeGold, DataTransfer.timeAce, DataTransfer.userTime / 1000000.0);
                                    Array.Resize(ref injectedLeaderboard, injectedLeaderboard.Length + 1);
                                    profilePictureUser.LoadImage(File.ReadAllBytes("Data/Flags/" + pluginInstance.configCountry.Value + ".png"));
                                    injectedLeaderboard[i + 1] = new ScoreData(
                                        ranking: i + DataTransfer.start + 2,
                                        oldRanking: i + DataTransfer.start + 2,
                                        profilePicture: profilePictureUser,  
                                        username: pluginInstance.configUsername.Value,
                                        scoreValueInMilliseconds: (int)(DataTransfer.userTime / 1000.0),
                                        medalValue: medal,  //* Medal score(1=bronze, 2=silver, 3=gold...). 0 means no medal. Dev medal is not here
                                        userScore: true,
                                        numLevelsCompleted: -1, //* Percentage of completed levels, -1 sets this to null
                                        found: true
                                    ); 
                                }
                                
                            }
                        }
                    }
                    /*menuScreenResultsInstance = FindObjectOfType<MenuScreenResults>(true);
                    if (menuScreenResultsInstance != null)
                    {
                        Debug.Log("[DEBUG] Calling MenuScreenResultsInstance");
                        menuScreenResultsInstance.SetMedal(1,1,1,1,1,6,false);
                    }
                    else
                    {
                        Debug.LogError("[DEBUG] MenuScreenResultsInstance is null!");
                    }*/ //!Enters correctly but the function is not right

                }
                //* Inject new leaderboard
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
                ReloadLeaderboardUI();
                
            }
        }
        [HarmonyPatch(typeof(Leaderboards), "OnRightArrowPressed")]
        public class Patch_RightButtonPressed
        {
            static void Postfix()
            {
                DataTransfer.isNextPageButtonPressed = true;
                Logger.LogInfo("Next button pressed!");
                ReloadLeaderboardUI();
            }
            
        }
        [HarmonyPatch(typeof(Leaderboards), "OnLastPageButtonPressed")]
        public class Patch_LastPageButton
        {
            static void Postfix()
            {
                DataTransfer.isLastPageButtonPressed = true;
                Logger.LogInfo("Last page button pressed!");
                ReloadLeaderboardUI();
            }
            
        }
        [HarmonyPatch(typeof(Leaderboards), "OnFirstPageButtonPressed")]
        public class Patch_FirstPageButton
        {
            static void Postfix()
            {
                DataTransfer.start = 0;
                Logger.LogInfo("First page button pressed!");
                ReloadLeaderboardUI();
            }
            
        }
        [HarmonyPatch(typeof(Leaderboards), "OnUserButtonPressed")]
        public class Patch_UserButton
        {
            static void Postfix()
            {
                DataTransfer.isUserButtonPressed = true;
                Logger.LogInfo("My score button pressed!");
                ReloadLeaderboardUI();
            }
            
        }
        [HarmonyPatch(typeof(LevelData), "GetLevelDisplayName")]
        public class Patch_GetLevelDisplayName
        {
            static void Postfix(LevelData __instance)
            {
                DataTransfer.levelName = __instance.levelID.Replace(" ", "");
                Logger.LogInfo("Level id: " + DataTransfer.levelName);
            }
        }

        [HarmonyPatch(typeof(LevelData), "GetTimeSilver")]
        public class Patch_GetMedalsTimes
        {
            static void Postfix(LevelData __instance)
            {
                Logger.LogInfo("[DEBUG] Silver time:" + __instance.timeSilver);
                DataTransfer.timeSilver = __instance.timeSilver;
                Logger.LogInfo("[DEBUG] Gold time:" + __instance.timeGold);
                DataTransfer.timeGold = __instance.timeGold;
                Logger.LogInfo("[DEBUG] Ace time:" + __instance.timeAce);
                DataTransfer.timeAce = __instance.timeAce;
                Logger.LogInfo("[DEBUG] Dev time:" + __instance.timeDev);
                DataTransfer.timeDev = __instance.timeDev; //*(int)Mathf.Max(__instance.timeDev, 1.5f)
            }
        }

        [HarmonyPatch(typeof(LevelStats), "GetTimeBestMicroseconds")]
        public class Patch_GetTimeBestMicroseconds
        {
            static void Postfix(LevelStats __instance)
            {
                DataTransfer.userTime = __instance._timeBestMicroseconds;
                Logger.LogInfo("[DEBUG] Usertime: " + DataTransfer.userTime / 1000000.0);
            }
        }

        
        [HarmonyPatch(typeof(LeaderboardIntegrationSteam), "OnFoundLevelLeaderboardForInitalLevelSetup")]
        public class Patch_SkipSteamFrontend //* Needed to skip the steam frontend, which throws an error when online
        {
            static bool Prefix()
            {
               
                if (AccessTools.Field(typeof(LeaderboardIntegrationSteam), "loadingLeaderboard") != null)
                    AccessTools.Field(typeof(LeaderboardIntegrationSteam), "loadingLeaderboard").SetValue(null, false);

                AccessTools.Method(typeof(LeaderboardIntegrationSteam), "OnLoadComplete2").Invoke(null, [true, true, false]);
                return false;
            }
        }
        // TODO: Fix the bug with delay 5-10 seconds when loading for the first time.
        
        [HarmonyPatch(typeof(LeaderboardIntegrationSteam), "OnLeaderboardScoresFriendsDownloaded")]
        public class Patch_SkipLeaderboardDownload //* Needed to skip the steam frontend, which throws an error when offline
        { 
            static bool Prefix()
            {

                return false; //Skipping function
            }
        }
        static void ReloadLeaderboardUI() //* Fix to correct the behaviour of the buttons by forcing a refresh
        {
            LeaderboardScoresDownloaded_t pCallback = new LeaderboardScoresDownloaded_t();
            LeaderboardIntegrationSteam.OnLeaderboardScoreDownloadGlobalResult2(pCallback, false);
        }

        static int CalcMedal(double timeSilver, double timeGold, double timeAce, double userTime)
        {
            //Logger.LogInfo($"Checking medal for userTime: {userTime}, with times - Ace: {timeAce}, Gold: {timeGold}, Silver: {timeSilver}");
            if (userTime <= timeAce)
            {
                return 4;
            }
            else if (userTime <= timeGold)
            {
                return 3;
            }
            else if (userTime <= timeSilver)
            {
                return 2;
            }
            else
            {
                return 1;
            }
                                    
        }

        [HarmonyPatch(typeof(LeaderboardScore), "SetScore")]
        public class Patch_SetScore
        {
            static void Postfix(LeaderboardScore __instance)
            {
                if (DataTransfer.isDevMedal)
                {
                    __instance._medal.sprite = Singleton<Game>.Instance.GetGameData().medalSprite_Dev;
                }
                
            }
        }
    
    }
}
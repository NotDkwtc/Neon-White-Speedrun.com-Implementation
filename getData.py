import requests
import time
import os.path
REBIRTH_IDS = ["nwle8ood", "dlo6xzml", "21g3poxq", "jqzm3y4l", "klrr260l", "gq76xzrl", "21g3pooq", "jqzm3ykl", "klrr26wl", "21dmeog1", "5q8m9r6l", "4qy6w8dl"]  #Ids for each level of chapter Rebirth
REBIRTH_NAMES = ["TUT_MOVEMENT", "TUT_SHOOTINGRANGE", "SLUGGER", "TUT_FROG", "TUT_JUMP", "GRID_TUT_BALLOON", "TUT_BOMB2", "TUT_BOMBJUMP", "TUT_FASTTRACK", "GRID_PORT"]  #Names of a given chapter
KILLER_INSIDE_IDS = ["ywepo5ld", "jlzx0j58", "mln2oknl", "810rd9p1", "9qj58eo1", "jq6jego1", "5lm9mw01", "81wk0r61", "zqo7v241", "013y64yl", "rqvnyzy1", "5lewxd6l"]
KILLER_INSIDE_NAMES = ["GRID_PAGODA", "TUT_RIFLE", "TUT_RIFLEJOCK", "TUT_DASHENEMY", "GRID_JUMPDASH", "GRID_SMACKDOWN", "GRID_MEATY_BALLOONS", "GRID_FAST_BALLOON", "GRID_DRAGON2", "GRID_DASHDANCE"]
ONLY_SHALLOW_IDS = ["69z012xd", "9l75m478", "0q503kv1", "4lxj63gq", "814nw3kl", "z19wp74q", "p1280321", "81pg58n1", "xqkxn741", "gq76xkrl", "21g3pyoq", "jqzm3kkl"]
ONLY_SHALLOW_NAMES = ["TUT_GUARDIAN", "TUT_UZI", "TUT_JUMPER", "TUT_BOMB", "GRID_DESCEND", "GRID_STAMPEROUT", "GRID_CRUISE", "GRID_SPRINT", "GRID_MOUNTAIN", "GRID_SUPERKINETIC"]
THE_OLD_CITY_IDS = ["r9g8p4jd" , "yn2mk9j8", "klrr2nwl", "21dmerg1", "5q8m956l"]
THE_OLD_CITY_NAMES = ["GRID_ARRIVAL", "FLOATING", "GRID_BOSS_YELLOW"]
THE_BURN_THAT_CURES_IDS = ["o9x0j739", "6njkx75l", "4qy6w3dl", "mln2ornl", "810rd3p1", "9qj58jo1", "jq6je3o1", "5lm9mz01", "81wk0361", "zqo7vd41", "013y63yl", "rqvny3y1"]
THE_BURN_THAT_CURES_NAMES = ["GRID_HOPHOP", "GRID_RINGER_TUTORIAL", "GRID_RINGER_EXPLOARATION", "GRID_HOPSCOTCH", "GRID_BOOM", "GRID_SNAKE_IN_MY_BOOT", "GRID_FLOCK", "GRID_BOMBS_AHOY", "GRID_ARCS", "GRID_APARTMENT"]
COVENANT_IDS = ["69zvqmlw", "kn003p0n", "5lewxg6l", "0q503ev1", "4lxj6wgq", "814nwekl", "z19wpg4q", "p1280e21", "81pg5wn1", "xqkxnk41", "gq76xerl", "21g3proq"]
COVENANT_NAMES = ["TUT_TRIPWIRE", "GRID_TANGLED", "GRID_HUNT", "GRID_CANNONS", "GRID_FALLING", "TUT_SHOCKER2", "TUT_SHOCKER", "GRID_PREPARE", "GRID_TRIPMAZE", "GRID_RACE"]
RECKONING_IDS = ["495ne5m9", "ql6rmkwl", "jqzm37kl", "klrr2wwl", "21dme7g1", "5q8m976l", "4qy6wedl", "mln2o6nl", "810rdep1", "9qj58yo1", "jq6jeko1", "5lm9mj01"]
RECKONING_NAMES = ["TUT_FORCEFIELD2", "GRID_SHIELD", "SALVAGE2", "GRID_VERTICAL", "GRID_MINEFIELD", "TUT_MIMIC", "GRID_MIMICPOP", "GRID_SWARM", "GRID_SWITCH", "GRID_TRAPS2"]
BENEDICTION_IDS = ["rdqqe7od", "onv4xz5n", "81wk0w61", "zqo7vw41", "013y6eyl", "rqvnywy1", "5lewxz6l", "0q503zv1", "4lxj6ogq", "814nwgkl", "z19wpy4q", "p1280m21"]
BENEDICTION_NAMES = ["TUT_ROCKETJUMP", "TUT_ZIPLINE", "GRID_CLIMBANG", "GRID_ROCKETUZI", "GRID_CRASHLAND", "GRID_ESCALATE", "GRID_SPIDERCLAUS", "GRID_FIRECRACKER_2", "GRID_SPIDERMAN", "GRID_DESTRUCTION"]
APOCRYPHA_IDS = ["5d7rg4q9", "ylpqxzk8", "81pg5on1", "xqkxn041", "gq76xdrl", "21g3pdoq", "jqzm3ekl", "klrr24wl", "21dmegg1", "5q8m986l", "4qy6wodl", "mln2omnl"]
APOCRYPHA_NAMES = ["GRID_HEAT", "GRID_BOLT", "GRID_PON", "GRID_CHARGE", "GRID_MIMICFINALE", "GRID_BARRAGE", "GRID_1GUN", "GRID_HECK", "GRID_ANTIFARM", "GRID_FORTRESS"]
THE_THIRD_TEMPLE_IDS = ["kwjr2z0d", "0nw0djdl", "810rdjp1", "9qj58no1"]
THE_THIRD_TEMPLE_NAMES = ["GRID_GODTEMPLE_ENTRY", "GRID_BOSS_GODSDEATHTEMPLE"]
THOUSAND_POUND_BUTTEFLY_IDS = ["owoxr6jw", "789dm5qn", "jq6jero1", "5lm9m601", "81wk0o61", "zqo7vo41", "013y67yl", "rqvnyoy1", "5lewxr6l", "0q503nv1", "4lxj6pgq", "814nwykl"]
THOUSAND_POUND_BUTTEFLY_NAMES = ["GRID_EXTERMINATOR", "GRID_FEVER", "GRID_SKIPSLIDE", "GRID_CLOSER", "GRID_HIKE", "GRID_SKIP", "GRID_CEILING", "GRID_BOOP", "GRID_TRIPRAP", "GRID_ZIPRAP"]
HAND_OF_GOD_IDS = ["xd11n2zd", "2lg1x2ql", "z19wpm4q", "p1280d21"]
HAND_OF_GOD_NAMES = ["TUT_ORIGIN", "GRID_BOSS_RAPTURE"]
RED_SIDEQUESTS_IDS = ["xd4oog29", "wl3d4go8", "81pg5pn1", "xqkxnj41", "gq76xjrl", "21g3p8oq", "jqzm3pkl", "klrr2pwl", "gq76xjdl", "21g3p88q"]
RED_SIDEQUESTS_NAMES = ["SIDEQUEST_OBSTACLE_PISTOL", "SIDEQUEST_OBSTACLE_PISTOL_SHOOT", "SIDEQUEST_OBSTACLE_MACHINEGUN", "SIDEQUEST_OBSTACLE_RIFLE_2", "SIDEQUEST_OBSTACLE_UZI2", "SIDEQUEST_OBSTACLE_SHOTGUN", "SIDEQUEST_OBSTACLE_ROCKETLAUNCHER", "SIDEQUEST_RAPTURE_QUEST"]
VIOLET_SIDEQUESTS_IDS = ["xd0556x9", "wlek47el", "jqzm3p8l", "klrr2pml", "21dmez51", "5q8m963l", "4qy6wp2l", "mln2owjl", "810rdz21", "9qj58031"]
VIOLET_SIDEQUESTS_NAMES = ["SIDEQUEST_DODGER", "GRID_GLASSPATH", "GRID_GLASSPATH2", "GRID_HELLVATOR", "GRID_GLASSPATH3", "SIDEQUEST_ALL_SEEING_EYE", "SIDEQUEST_RESIDENTSAWB", "SIDEQUEST_RESIDENTSAW"]
YELLOW_SIDEQUESTS_IDS = ["rw6665rw", "68k7xzzl", "jq6je0j1", "5lm9mkm1", "81wk0zv1", "zqo7vkx1", "013y6dxl", "rqvnyg61", "5lewx9kl", "0q503y21"]
YELLOW_SIDEQUESTS_NAMES = ["SIDEQUEST_SUNSET_FLIP_POWERBOMB", "GRID_BALLOONLAIR", "SIDEQUEST_BARREL_CLIMB", "SIDEQUEST_FISHERMAN_SUPLEX", "SIDEQUEST_STF", "SIDEQUEST_ARENASIXNINE", "SIDEQUEST_ATTITUDE_ADJUSTMENT", "SIDEQUEST_ROCKETGODZ"]
SPEEDRUN_API_BASE = "https://www.speedrun.com/api/v1/"  #Speedrun.com base api url
FLAGS_API_BASE = "https://flagsapi.com"  #Flagsapi.com base api url
CATEGORY_ID = "5dwq1gld"  #PC Steam category, we don't want other platforms records
GAME_ID = "pdv27nv6"  #Neon White game id 

def fetch_json(url):
    response = requests.get(url)
    if response.status_code == 420:
        print("Rate Limited! Waiting a minute")
        time.sleep(60)
        fetch_json(url)
    if response.status_code != 200:
        time.sleep(10)
        raise Exception(f"API request failed for {url} with status {response.status_code}")
    return response.json()

def retrieve_leaderboard(array_id, array_names):
    try:
        level_id = array_id[0]  
        for i in range(len(array_id)):
            if (i + 2 > len(array_id)):
                return
            file_path = f"Data/Leaderboards/{array_names[i]}.txt"
            sublevel_id = array_id[(i + 2)]
            url = f"{SPEEDRUN_API_BASE}/leaderboards/{GAME_ID}/level/{level_id}/{CATEGORY_ID}/?var-{array_id[1]}={sublevel_id}"

            records_data = fetch_json(url)  
            if os.path.isfile(file_path):
                print("Files for chapter exist, skipping")
                return           
            for run_entry in records_data.get("data", {}).get("runs", []):
                time_seconds = run_entry["run"]["times"]["primary_t"]
                for player in run_entry["run"]["players"]:
                    user_data = fetch_json(player["uri"])
                    try:
                        username = user_data["data"]["names"]["international"]
                        country_raw = user_data["data"]["location"]["country"]["code"]
                        country = country_raw.split("/")
                        with open(file_path, "a", encoding="utf-8") as f:
                            f.write(f"{username},{time_seconds},{country[0].upper()}\n")
                        flags_url = f"{FLAGS_API_BASE}/{country[0].upper()}/flat/64.png"
                        response = requests.get(flags_url)
                        if response.status_code != 200:
                            raise Exception(f"API request failed for {flags_url} with status {response.status_code}")
                        with open(f"Data/Flags/{country[0].upper()}.png", "wb") as f:
                            f.write(response.content)
                    except Exception as e:
                        print(f"Exception encountered: {e}")
    except Exception as e:
        print(f"Exception encountered: {e}")
def main():
    retrieve_leaderboard(REBIRTH_IDS, REBIRTH_NAMES)
    retrieve_leaderboard(KILLER_INSIDE_IDS, KILLER_INSIDE_NAMES)           
    retrieve_leaderboard(ONLY_SHALLOW_IDS, ONLY_SHALLOW_NAMES)          
    retrieve_leaderboard(THE_OLD_CITY_IDS, THE_OLD_CITY_NAMES)
    retrieve_leaderboard(THE_BURN_THAT_CURES_IDS, THE_BURN_THAT_CURES_NAMES)           
    retrieve_leaderboard(COVENANT_IDS, COVENANT_NAMES) 
    retrieve_leaderboard(RECKONING_IDS, RECKONING_NAMES)
    retrieve_leaderboard(BENEDICTION_IDS, BENEDICTION_NAMES)           
    retrieve_leaderboard(APOCRYPHA_IDS, APOCRYPHA_NAMES) 
    retrieve_leaderboard(THE_THIRD_TEMPLE_IDS, THE_THIRD_TEMPLE_NAMES)
    retrieve_leaderboard(THOUSAND_POUND_BUTTEFLY_IDS, THOUSAND_POUND_BUTTEFLY_NAMES)
    retrieve_leaderboard(HAND_OF_GOD_IDS, HAND_OF_GOD_NAMES)           
    retrieve_leaderboard(RED_SIDEQUESTS_IDS, RED_SIDEQUESTS_NAMES) 
    retrieve_leaderboard(VIOLET_SIDEQUESTS_IDS, VIOLET_SIDEQUESTS_NAMES) 
    retrieve_leaderboard(YELLOW_SIDEQUESTS_IDS, YELLOW_SIDEQUESTS_NAMES)      
    with open("Data/Leaderboards/fallback.txt", "w", encoding="utf-8") as f:
        f.write("NoEntry,1.337,US") 
if __name__ == "__main__":
    main()
import requests
import time
import os.path
REBIRTH_IDS = ["nwle8ood", "dlo6xzml", "21g3poxq", "jqzm3y4l", "klrr260l", "gq76xzrl", "21g3pooq", "jqzm3ykl", "klrr26wl", "21dmeog1", "5q8m9r6l", "4qy6w8dl"]  #Ids for each level of chapter Rebirth
REBIRTH_NAMES = ["Movement", "Pummel", "Gunner", "Cascade", "Elevate", "Bounce", "Purify", "Climb", "Fasttrack", "Glass Port"]  #Names of a given chapter
KILLER_INSIDE_IDS = ["ywepo5ld", "jlzx0j58", "mln2oknl", "810rd9p1", "9qj58eo1", "jq6jego1", "5lm9mw01", "81wk0r61", "zqo7v241", "013y64yl", "rqvnyzy1", "5lewxd6l"]
KILLER_INSIDE_NAMES = ["Take Flight", "Godspeed", "Dasher", "Thrasher", "Outstretched", "Smackdown", "Catwalk", "Fastlane", "Distinguish", "Dancer"]
ONLY_SHALLOW_IDS = ["69z012xd", "9l75m478", "0q503kv1", "4lxj63gq", "814nw3kl", "z19wp74q", "p1280321", "81pg58n1", "xqkxn741", "gq76xkrl", "21g3pyoq", "jqzm3kkl"]
ONLY_SHALLOW_NAMES = ["Guardian", "Stomp", "Jumper", "Dash Tower", "Descent", "Driller", "Canals", "Sprint", "Mountain", "Superkinetic"]
THE_OLD_CITY_IDS = ["r9g8p4jd" , "yn2mk9j8", "klrr2nwl", "21dmerg1", "5q8m956l"]
THE_OLD_CITY_NAMES = ["Arrival", "Forgotten City", "The Clocktower"]
THE_BURN_THAT_CURES_IDS = ["o9x0j739", "6njkx75l", "4qy6w3dl", "mln2ornl", "810rd3p1", "9qj58jo1", "jq6je3o1", "5lm9mz01", "81wk0361", "zqo7vd41", "013y63yl", "rqvny3y1"]
THE_BURN_THAT_CURES_NAMES = ["Fireball", "Ringer", "Cleaner", "Warehouse", "Boom", "Streets", "Steps", "Demolition", "Arcs", "Apartment"]
COVENANT_IDS = ["69zvqmlw", "kn003p0n", "5lewxg6l", "0q503ev1", "4lxj6wgq", "814nwekl", "z19wpg4q", "p1280e21", "81pg5wn1", "xqkxnk41", "gq76xerl", "21g3proq"]
COVENANT_NAMES = ["Hanging Gardens", "Tangled", "Waterworks", "Killswitch", "Falling", "Shocker", "Bouquet", "Prepare", "Triptrack", "Race"]
RECKONING_IDS = ["495ne5m9", "ql6rmkwl", "jqzm37kl", "klrr2wwl", "21dme7g1", "5q8m976l", "4qy6wedl", "mln2o6nl", "810rdep1", "9qj58yo1", "jq6jeko1", "5lm9mj01"]
RECKONING_NAMES = ["Bubble", "Shield", "Overlook", "Pop", "Minefield", "Mimic", "Trigger", "Greenhouse", "Sweep", "Fuse"]
BENEDICTION_IDS = ["rdqqe7od", "onv4xz5n", "81wk0w61", "zqo7vw41", "013y6eyl", "rqvnywy1", "5lewxz6l", "0q503zv1", "4lxj6ogq", "814nwgkl", "z19wpy4q", "p1280m21"]
BENEDICTION_NAMES = ["Heaven's Edge", "Zipline", "Swing", "Chute", 	"Crash", "Ascent", "Straightaway", "Firecracker", "Streak", "Mirror"]
APOCRYPHA_IDS = ["5d7rg4q9", "ylpqxzk8", "81pg5on1", "xqkxn041", "gq76xdrl", "21g3pdoq", "jqzm3ekl", "klrr24wl", "21dmegg1", "5q8m986l", "4qy6wodl", "mln2omnl"]
APOCRYPHA_NAMES = ["Escalation", "Bolt", "Godstreak", "Plunge", "Mayhem", "Barrage", "Estate", "Trapwire", "Ricochet", "Fortress"]
THE_THIRD_TEMPLE_IDS = ["kwjr2z0d", "0nw0djdl", "810rdjp1", "9qj58no1"]
THE_THIRD_TEMPLE_NAMES = ["Holy Ground", "The Third Temple"]
THOUSAND_POUND_BUTTEFLY_IDS = ["owoxr6jw", "789dm5qn", "jq6jero1", "5lm9m601", "81wk0o61", "zqo7vo41", "013y67yl", "rqvnyoy1", "5lewxr6l", "0q503nv1", "4lxj6pgq", "814nwykl"]
THOUSAND_POUND_BUTTEFLY_NAMES = ["Spree", "Breakthrough", "Glide", "Closer", "Hike", "Switch", "Access", "Congregation", "Sequence", "Marathon"]
HAND_OF_GOD_IDS = ["xd11n2zd", "2lg1x2ql", "z19wpm4q", "p1280d21"]
HAND_OF_GOD_NAMES = ["Sacrifice", "Absolution"]
RED_SIDEQUESTS_IDS = ["xd4oog29", "wl3d4go8", "81pg5pn1", "xqkxnj41", "gq76xjrl", "21g3p8oq", "jqzm3pkl", "klrr2pwl", "gq76xjdl", "21g3p88q"]
RED_SIDEQUESTS_NAMES = ["Elevate Traversal I", "Elevate Traversal II", "Purify Traversal", 	"Godspeed Traversal", "Stomp Traversal", "Fireball Traversal", "Dominion Traversal", "Book of Life Traversal"]
VIOLET_SIDEQUESTS_IDS = ["xd0556x9", "wlek47el", "jqzm3p8l", "klrr2pml", "21dmez51", "5q8m963l", "4qy6wp2l", "mln2owjl", "810rdz21", "9qj58031"]
VIOLET_SIDEQUESTS_NAMES = ["Doghouse", "Choker", "Chain", "Hellevator", "Razor", "All Seeing Eye", "Resident Saw I", "Resident Saw II"]
YELLOW_SIDEQUESTS_IDS = ["rw6665rw", "68k7xzzl", "jq6je0j1", "5lm9mkm1", "81wk0zv1", "zqo7vkx1", "013y6dxl", "rqvnyg61", "5lewx9kl", "0q503y21"]
YELLOW_SIDEQUESTS_NAMES = ["Sunset Flip Powerbomb", "Balloon Mountain", "Climbing Gym", "Fisherman Suplex", "STF", "Arena", "Attitude Adjustment", "Rocket"]
API_BASE = "https://www.speedrun.com/api/v1/"  #Base api url
CATEGORY_ID = "5dwq1gld"  #PC Steam category, we don't want other platforms records
GAME_ID = "pdv27nv6"  #Neon White game id 


def fetch_json(url):
    response = requests.get(url)
    if response.status_code == 420:
        print("Rate Limited! Waiting a minute")
        time.sleep(60)
        fetch_json(url)
    if response.status_code != 200:
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
            url = f"{API_BASE}/leaderboards/{GAME_ID}/level/{level_id}/{CATEGORY_ID}/?var-{array_id[1]}={sublevel_id}"

            records_data = fetch_json(url)  
            if os.path.isfile(file_path):
                print("Files for chapter exist, skipping")
                return           
            for run_entry in records_data.get("data", {}).get("runs", []):
                time_seconds = run_entry["run"]["times"]["primary_t"]
                for player in run_entry["run"]["players"]:
                    username_data = fetch_json(player["uri"])
                    try:
                        username = username_data["data"]["names"]["international"]
                        with open(file_path, "a", encoding="utf-8") as f:
                            f.write(f"{username},{time_seconds}\n")
                    except:
                        print("No username, skipping")
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
if __name__ == "__main__":
    main()
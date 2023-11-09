using System.Collections.Generic;

public class LevelDetailedInfo
{
    public class BossPrefabInfo
    {
        public string bundle_name;
        public List<string> resources;
        public string prefab_name;
        public int load_at_start;
        public List<string> boss_abilities;
        public List<double> start_position;
    }
    public string name;
    public string scene_prefab_path;
    public string bgm_path;
    
    public int total_boss_num;
    public List<BossPrefabInfo> boss_prefab;
    public List<double> player_position;
    public int revive_limit;
    public int time_limit;
    public int crown_time_limit;
    public int crown_revive_limit;
    public int clear_condition = 0;


}

public class StoryLevelDetailedInfo
{
    public string name;
    public string scene_prefab_path;
    public string bgm_path;
    public List<string> boss_bundles = new();
    public List<string> resources = new();
    public int clear_condition = 0;
    public int fixed_chara_id = 0;
    public float crown_time_limit = 0;
    public int crown_revive_limit = 0;
    public int revive_limit = 0;
    public float time_limit = 0;
    
}

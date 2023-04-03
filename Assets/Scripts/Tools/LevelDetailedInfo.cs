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
    public string overall_camera_path;
    public int total_boss_num;
    public List<BossPrefabInfo> boss_prefab;
    public List<double> player_position;
    public int revive_limit;
    public int time_limit;
    public int crown_time_limit;
    public int crown_revive_limit;


}

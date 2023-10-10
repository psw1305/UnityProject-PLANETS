namespace MasterTablePlayer
{
    public class MasterTablePlayer : MasterTableBase<PlayerMaster>
    {
        private static readonly string FilePath = "Game_DB - Player";
        public void Load() { Load(FilePath); }
    }

    public class PlayerMaster : MasterBase
    {
        public string Race { get; private set; }
        public string Type { get; private set; }
        public string Name { get; private set; }
        public string Plus { get; private set; }
        public int Level { get; private set; }
        public float Damage { get; private set; }
        public float Health { get; private set; }
        public float Shield { get; private set; }
        public int Maintain { get; private set; }
        public string Skill_Lv { get; private set; }
        public string Skill_Name { get; private set; }
        public float Cooltime { get; private set; }
        public float DUR { get; private set; }
        public float ATK { get; private set; }
        public float RAN { get; private set; }
        public float NUM { get; private set; }
        public string Skill_Info { get; private set; }
        public int Ship_Build_Credit { get; private set; }
        public int Ship_Build_Core { get; private set; }
        public int Ship_Research_Credit { get; private set; }
        public int Ship_Research_Core { get; private set; }
        public int Ship_Research_DarkMatter { get; private set; }
    }

    public class MasterTableCaptain : MasterTableBase<CaptainMaster>
    {
        private static readonly string FilePath = "Game_DB - Captain";
        public void Load() { Load(FilePath); }
    }

    public class CaptainMaster : MasterBase
    {
        public string Captain_Type { get; private set; }
        public string Captain_Race { get; private set; }
        public string Captain_Name { get; private set; }
        public string Skill_Type { get; private set; }
        public string Skill_Name { get; private set; }
        public string Skill_Info { get; private set; }
    }
}

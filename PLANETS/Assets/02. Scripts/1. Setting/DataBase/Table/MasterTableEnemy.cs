namespace MasterTableEnemy
{
    public class MasterTableEnemy : MasterTableBase<EnemyMaster>
    {
        private static readonly string FilePath = "Game_DB - Enemy";
        public void Load() { Load(FilePath); }
    }

    public class EnemyMaster : MasterBase
    {
        public string Race { get; private set; }
        public string Type { get; private set; }
        public string Plus { get; private set; }
        public string Level { get; private set; }   
        public float Damage { get; private set; }
        public float Shield { get; private set; }
        public float Health { get; private set; }
        public float Cooltime { get; private set; }
        public float DUR { get; private set; }
        public float ATK { get; private set; }
        public float RAN { get; private set; }
        public float NUM { get; private set; }
        public string Information { get; private set; }
    }
}

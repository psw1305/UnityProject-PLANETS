namespace MasterTableStage
{
    public class MasterTableFormula : MasterTableBase<FormulaMaster>
    {
        private static readonly string FilePath = "Game_DB - Formula";
        public void Load() { Load(FilePath); }
    }

    public class FormulaMaster : MasterBase
    {
        public string Type { get; private set; }
        public float Value1 { get; private set; }
        public float Value2 { get; private set; }
        public float Value3 { get; private set; }
        public float Value4 { get; private set; }
    }

    public class MasterTableShip : MasterTableBase<ShipMaster>
    {
        private static readonly string FilePath = "Game_DB - Ship Base";
        public void Load() { Load(FilePath); }
    }

    public class ShipMaster : MasterBase
    {
        public string Race { get; private set; }
        public string Type { get; private set; }
        public float Firerate { get; private set; }
        public float ShieldTime { get; private set; }
        public float Moving { get; private set; }
        public float TurnMoving { get; private set; }
        public float Crew { get; private set; }
    }

    public class MasterTableFighter : MasterTableBase<FighterMaster>
    {
        private static readonly string FilePath = "Game_DB - Fighter";
        public void Load() { Load(FilePath); }
    }

    public class FighterMaster : MasterBase
    {
        public string Race { get; private set; }
        public int Player_Level { get; private set; }
        public float Player_Stat { get; private set; }
        public string Enemy_Level { get; private set; }
        public float Enemy_Stat { get; private set; }
    }

    public class MasterTableStageInfo : MasterTableBase<StageMaster>
    {
        private static readonly string FilePath = "Game_DB - Stage";
        public void Load() { Load(FilePath); }
    }

    public class StageMaster : MasterBase
    {
        public string UID { get; private set; }
        public string Difficulty { get; private set; }
        public string Bonus_Name { get; private set; }
        public int Bonus_Value { get; private set; }
    }

    public class MasterTableBattle : MasterTableBase<BattleMaster>
	{
		private static readonly string FilePath = "Game_DB - Battle";
		public void Load () { Load (FilePath); }
	}

	public class BattleMaster : MasterBase 
	{
		public string UID { get; private set; }
		public int Destroyer { get; private set; }
		public int Battleship { get; private set; }
		public int Auxiliary { get; private set; }
		public int Cruiser { get; private set; }
		public int Carrier { get; private set; }
    }
}
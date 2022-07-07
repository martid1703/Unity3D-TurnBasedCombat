namespace UnfrozenTestWork
{
    public class UnitSpawnerResult
    {
        public UnitModel[] Player1Units { get; private set; }
        public UnitModel[] Player2Units { get; private set; }
        public UnitModel[] NeutralUnits { get; private set; }

        public UnitSpawnerResult(UnitModel[] player1Units, UnitModel[] player2Units, UnitModel[] neutralUnits)
        {
            Player1Units = player1Units ?? throw new System.ArgumentNullException(nameof(player1Units));
            Player2Units = player2Units ?? throw new System.ArgumentNullException(nameof(player2Units));
            NeutralUnits = neutralUnits ?? throw new System.ArgumentNullException(nameof(neutralUnits));
        }
    }
}

namespace Andoco.Unity.Framework.Units.Signals
{
    using Andoco.Core.Signals;

    public class UnitHealthSignal : Signal<UnitHealthSignal.Data>
    {
        public struct Data
        {
            public Data(Unit unit, UnitHealthState state)
				: this()
            {
                this.Unit = unit;
                this.State = state;
            }

            public Unit Unit { get; private set; }

            public UnitHealthState State { get; private set; }
        }
    }
}

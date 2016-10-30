namespace Andoco.Unity.Framework.Units.Signals
{
    using Andoco.Core.Signals;

    public class UnitStrengthSignal : Signal<UnitStrengthSignal.Data>
    {
        public struct Data
        {
            public Data(Unit unit, UnitStrengthState state)
				: this()
            {
                this.Unit = unit;
                this.State = state;
            }

            public Unit Unit { get; private set; }

            public UnitStrengthState State { get; private set; }
        }
    }
}

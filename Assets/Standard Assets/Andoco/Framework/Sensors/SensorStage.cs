namespace Andoco.Unity.Framework.Sensors
{
    using System;

    [Flags]
    public enum SensorStage
    {
        Enter = (1 << 0),
        Continue = (1 << 1),
        Exit = (1 << 2)
    }
}

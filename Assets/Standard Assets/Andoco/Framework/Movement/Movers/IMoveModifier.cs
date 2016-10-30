namespace Andoco.Unity.Framework.Movement.Movers
{
    using UnityEngine;

    public interface IMoveModifier
    {
        void StartModifying(IMoveDriver moveDriver);

        void StopModifying(IMoveDriver moveDriver);
    }
}
namespace Andoco.Unity.Framework.Units
{
    public interface IDamageSystem
    {
        void ApplyDamage(Unit inflictor, string profileKey, Unit victim);

        void CancelDamage(Unit inflictor, string profileKey);
    }
}

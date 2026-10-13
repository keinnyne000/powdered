namespace Game.Core.Guns
{
    /// <summary>
    /// Anything that can take damage from a gun (enemies, drones, breakable props)
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}

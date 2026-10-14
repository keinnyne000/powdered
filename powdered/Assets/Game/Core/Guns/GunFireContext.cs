using UnityEngine;

namespace Game.Core.Guns
{
    /// <summary>
    /// Where a shot comes from and which way it's aimed
    /// </summary>
    public readonly struct GunFireContext
    {
        public readonly Vector3 Origin;
        public readonly Vector3 Direction;

        public GunFireContext(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction.normalized;
        }
    }
}

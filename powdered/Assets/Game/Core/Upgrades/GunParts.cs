using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Guns
{
    /// <summary>
    /// A gun part the player picks up during a run. Collecting one adds its Modifier to GameState.
    /// Picking up the same part twice stacks it.
    /// </summary>
    public sealed class GunPart
    {
        public readonly string Name;
        public readonly string Description;
        private readonly Func<Modifier> _createModifier;

        public GunPart(string name, string description, Func<Modifier> createModifier)
        {
            Name = name;
            Description = description;
            _createModifier = createModifier;
        }

        public Modifier CreateModifier() => _createModifier();
    }

    /// <summary>
    /// Every gun part in the game. Add new parts here and to All.
    /// </summary>
    public static class GunParts
    {
        // Combo Power stops growing after this many hits in a row
        public const int MaxComboStacks = 10;

        public static readonly GunPart FireRate = new(
            "Fire Rate",
            "Shoot 25% faster",
            () => new Modifier("GunPart-FireRate")
                .Register<ShootGameAction>((shot, state) => shot.FireRateMultiplier += 0.25f));

        public static readonly GunPart ComboPower = new(
            "Combo Power",
            $"+1 damage for each hit in a row without missing (up to +{MaxComboStacks})",
            () => new Modifier("GunPart-ComboPower")
                .Register<ShootGameAction>((shot, state) =>
                    shot.BonusDamage += Mathf.Min(state.ComboHits, MaxComboStacks)));

        public static readonly GunPart HeavyRounds = new(
            "Heavy Rounds",
            "+5 damage",
            () => new Modifier("GunPart-HeavyRounds")
                .Register<ShootGameAction>((shot, state) => shot.BonusDamage += 5f));

        public static readonly IReadOnlyList<GunPart> All = new[] { FireRate, ComboPower, HeavyRounds };
    }
}

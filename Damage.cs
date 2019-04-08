using System;
using System.Text;
using System.Diagnostics;

namespace PhysicalDamage.Core
{
    internal abstract class Damage
    {
        protected Damage(DamageTypes damageType, Target currentTarget)
        {
            this.DamageType = damageType;
            this.CurrentTarget = currentTarget;
        }

        /// <Summary>
        ///  A struct containing the data of the target of the damage
        /// </Summary>
        public Target CurrentTarget { get; private set; }

        /// <Summary>
        ///  The type of the damage, indicated by a enum
        /// </Summary>
        public DamageTypes DamageType { get; private set; }

        /// <Summary>
        ///     Use this method to calculate damage.
        ///     This method returns a Target struct
        ///     containing updated values of Health, Armor and ERA stats.
        /// </Summary>
        public virtual Target CalculateDamage()
        {
            // Override this function to specify damage algorithm
            return this.CurrentTarget; // No damage dealt thus return the original state of the target
        }

        public struct Era
        {
            public float Value;
            public float KEFractionMultiplier;
            public float HeatFractionMultiplier;
        }

        public struct Target
        {
            public float Armor;
            public Era EraData;
            public float Health;
        }
    }

    public enum DamageTypes
    {
        KE,
        HEAT,
        HE,
        Fire,
        Laser
    }
}
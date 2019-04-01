using System;
using System.Text;
using System.Diagnostics;

namespace PhysicalDamage.Core
{
    abstract class Damage
    {
        protected EDamageTypes damageType;

        public EDamageTypes DamageType
        {
            // Once a damage has gained its type (in the constructor), its damage type should remain unchanged
            get
            {
                return damageType;
            }
        }

        public struct Target
        {
            public float Armor;

            public ERA ERAData;

            public float Health;
        }

        public struct ERA
        {
            public float CurrentValue;

            public float KEFractionMultiplier;

            public float HeatFractionMultiplier;
        }

        protected Target _target;

        public Damage(EDamageTypes dt, Target t)
        {
            // In base class constructor, pass in the type of damage
            damageType = dt;
            // And the target
            _target = t;
        }

        /// <Summary>
        /// Use this method to deal damage.
        /// This method returns a Target struct
        /// containing updated values of Health, Armor and ERA stats.
        /// </Summary>
        virtual public Target CalculateDamage()
        {
            // Override this function to specify damage algorithm
            Target finalState;
            return finalState;
        }
    }

    public enum EDamageTypes
    {
        KE,
        HEAT,
        Fire,
        Laser,
        HE
    }
}
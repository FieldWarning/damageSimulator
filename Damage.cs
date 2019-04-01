using System;
using System.Text;
using System.Diagnostics;

namespace PhysicalDamage.Core
{
    abstract class Damage
    {
        #region DamageType
        protected EDamageTypes damageType;

        public EDamageTypes DamageType
        {
            // Once a damage has gained its type (in the constructor), its damage type should remain unchanged
            get
            {
                return damageType;
            }
        }
        #endregion

        #region TargetAttribute
        public struct Target
        {
            public float Armor;

            public ERA ERAData;
        }

        public struct ERA
        {
            public float CurrentValue;

            public float KEFractionMultiplier;

            public float HEATFractionMultiplier;
        }
        #endregion

        protected Target target;

        public Damage(EDamageTypes dt, Target t)
        {
            // In base class constructor, pass in the type of damage
            damageType = dt;
            // And the target
            target = t;
        }

        /// <Summary>
        /// Use this method to deal damage
        /// This method only calculates HP damage, deal with armour degradation in separate method
        /// </Summary>
        virtual public float DealDamage()
        {
            // Override this function to specify damage algorithm
            return 0.0;
        }
    }

    public enum EDamageTypes
    {
        KE,
        HEAT,
        FIRE,
        LASER,
        HE
    }
}
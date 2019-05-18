using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PhysicalDamage.Core
{
    class HEDamage : Damage
    {
        public struct HEData
        {
            /// <summary>
            /// The power of the shot
            /// </summary>
            public float Power;
            /// <summary>
            /// The radius of the explosion
            /// Beyond the effective radius, the value {Remaining damage}/{Initial damage} is less than CUTOF_FFRACTION
            /// </summary>
            public float EffectiveRadius;
        }

        private HEData _heData;
        private float _distanceToCentre;

        /// <summary>
        /// The cutoff fraction for HE damage splash
        /// Read documentation {4.2 HE dropoff within AOE} for details
        /// </summary>
        private const float CUTOFF_FRACTION = 0.01f; // Relocate this constant elsewhere when integrating

        public HEDamage(HEData data, Target target, float distanceToCentre) : base(DamageTypes.HE, target)
        {
            _heData = data;
            _distanceToCentre = distanceToCentre;
        }

        public override Target CalculateDamage()
        {
            Target finalState = this.CurrentTarget;
            throw new NotImplementedException();
        }

        private float CalculatedPowerDropoff(float power, float radius, float distance)
        {
            if (distance > radius)
            {
                return 0.0f;
            }
            else
            {
                float finalPower = (float)(power / (4 * Math.PI * distance * distance));
                return finalPower;
            }
        }
    }
}

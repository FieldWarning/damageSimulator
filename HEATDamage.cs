using System;
using System.Text;
using System.Diagnostics;

namespace PhysicalDamage.Core
{
    class HeatDamage:Damage
    {
        public struct HeatData
        {
            /// <summary>
            /// The pierce of the shot
            /// </summary>
            public float Pierce;
            /// <summary>
            /// Multiplier for armor degradation
            /// </summary>
            public float Degradation;
            /// <summary>
            /// Multiplier for health damage
            /// </summary>
            public float HealthDamageFactor;
        }

        private HeatData _heatData;

        public HeatDamage(HeatData data, Target target)
            : base(DamageTypes.HEAT, target)
        {
            this._heatData = data;
        }

        public override Target CalculateDamage()
        {
            Target finalState = this.CurrentTarget;
            HeatData he = _heatData;

            // No air friction attenuation as HEAT round detonates on surface of the armor

            if (finalState.EraData.Value > 0.0f) {
                // Calculate effects of ERA
                float finalEra = Math.Max(
                    0.0f,
                    finalState.EraData.Value - he.Pierce * finalState.EraData.HeatFractionMultiplier
                );
                finalState.EraData.Value = finalEra;
                
                he.Pierce = CalculatePostEraPierce(
                    he.Pierce,
                    finalState.EraData.HeatFractionMultiplier
                );
            }

            // Armor degradation
            float finalArmor = Math.Max(
                0.0f,
                finalState.Armor - (he.Pierce / finalState.Armor) * he.Degradation
            );
            finalState.Armor = finalArmor;

            // Calculate final damage
            float finalDamage = Math.Max(
                0.0f,
                (he.Pierce - finalState.Armor) * he.HealthDamageFactor
            );
            float finalHealth = Math.Max(
                0.0f,
                finalState.Health - finalDamage
            );
            finalState.Health = finalHealth;
            
            return finalState;
        }

        private static float CalculatePostEraPierce(float pierce, float eraFractionMultiplier)
        {
            return pierce * (1 - eraFractionMultiplier);
        }
    }
}
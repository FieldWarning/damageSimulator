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

        public HeatDamage(HeatData data,Damage.Target target)
            : base(DamageTypes.HEAT, target)
        {
            this._heatData = data;
        }

        public override Target CalculateDamage()
        {
            Target finalState = this.CurrentTarget;
            HeatData he = _heatData;

            // No air friction attenuation as HEAT round detonates on surface of the armor

            if (this.CurrentTarget.EraData.Value > 0.0f) {
                // Calculate effects of ERA
                float finalEra = Math.Max(
                    0.0f,
                    this.CurrentTarget.EraData.Value - he.Pierce * this.CurrentTarget.EraData.HeatFractionMultiplier
                );
                finalState.EraData.Value = finalEra;
                
                he.Pierce = CalculatePostERAPierce(
                    he.Pierce,
                    this.CurrentTarget.EraData.HeatFractionMultiplier
                );
            }

            // Armor degradation
            float finalArmor = Math.Max(
                0.0f,
                this.CurrentTarget.Armor - (he.Pierce / this.CurrentTarget.Armor) * he.Degradation
            );
            finalState.Armor = finalArmor;

            // Calculate final damage
            float finalDamage = Math.Max(
                0.0f,
                (he.Pierce - this.CurrentTarget.Armor) * he.HealthDamageFactor
            );
            float finalHealth = Math.Max(
                0.0f,
                this.CurrentTarget.Health - finalDamage
            );
            finalState.Health = finalHealth;
            
            return finalState;
        }

        private static float CalculatePostERAPierce(float pierce, float eraFractionMultiplier)
        {
            float finalPierce = pierce * (1 - eraFractionMultiplier);
            return finalPierce;
        }
    }
}
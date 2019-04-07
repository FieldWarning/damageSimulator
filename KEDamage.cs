using System;
using System.Text;
using System.Diagnostics;

namespace PhysicalDamage.Core
{
    class KEDamage:Damage
    {
        public struct KineticData
        {
            /// <summary>
            /// The pierce of the shot
            /// </summary>
            public float Pierce;
            /// <summary>
            /// Distance between the firing unit and target unit
            /// </summary>
            public float Distance;
            /// <summary>
            /// Multiplier for armor degradation
            /// </summary>
            public float Degradation;
            /// <summary>
            /// Multiplier for health damage
            /// </summary>
            public float HealthDamageFactor;
            /// <summary>
            /// Air friction constant used in calculation of attenuation
            /// </summary>
            public float FrictionFactor;
        }

        private KineticData _keData;

        public KEDamage(KineticData data,Damage.Target target)
            : base(DamageTypes.KE, target)
        {
            this._keData = data;
        }

        public override Target CalculateDamage()
        {
            Target finalState = this.CurrentTarget;
            KineticData ke = _keData;

            // Calculate attenuation of air friction
            ke.Pierce = CalculateKEAttenuationSimple(
                ke.Pierce,
                ke.Distance,
                ke.FrictionFactor
            );

            if (this.CurrentTarget.EraData.Value > 0.0f) {
                // Calculate effects of ERA
                float finalEra = Math.Max(
                    0.0f,
                    this.CurrentTarget.EraData.Value - _kineticData.Pierce * this.CurrentTarget.EraData.KEFractionMultiplier
                );
                finalState.EraData.Value = finalEra;

                ke.Pierce = CalculatePostERAPierce(
                    ke.Pierce,
                    this.CurrentTarget.EraData.KEFractionMultiplier
                );
            }
            
            

            // Armor degradation
            float finalArmor = Math.Max(
                0.0f,
                this.CurrentTarget.Armor - (ke.Pierce / this.CurrentTarget.Armor) * ke.Degradation
            );
            finalState.Armor = finalArmor;

            // Calculate final damage
            float finalDamage = Math.Max(
                0.0f,
                (ke.Pierce - this.CurrentTarget.Armor) * ke.HealthDamageFactor
            );
            float finalHealth = Math.Max(
                0.0f,
                this.CurrentTarget.Health - finalDamage
            );
            finalState.Health = finalHealth;
            
            return finalState;
        }


        private static float CalculateKEAttenuationSimple(float pierce, float distance, float frictionFactor)
        {
            return  Math.Exp(-frictionFactor * distance) * pierce;
        }
        
        private static float CalculatePostERAPierce(float pierce, float eraFractionMultiplier)
        {
            return pierce * (1 - eraFractionMultiplier);
        }
    }
}
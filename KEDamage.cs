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
            /// Distance between the units
            /// </summary>
            public float Distance;
            /// <summary>
            /// Multiplier for armor degradation
            /// </summary>
            public float ArmorDegradationFactor;
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

        public KEDamage(KineticData d,Damage.Target t):base(EDamageTypes.KE, t)
        {
            _keData = d;
        }

        public override Target CalculateDamage()
        {
            Target finalState = _target;

            // Calculate attenuation of air friction
            _keData.Pierce = CalculateKEAttenuationSimple(_keData.Pierce,_keData.Distance,_keData.FrictionFactor);

            // Calculate effects of ERA
            float finalERA = Math.Max(0.0f, _target.ERAData.CurrentValue
             - _keData.Pierce*_target.ERAData.KEFractionMultiplier);
            finalState.ERAData.CurrentValue = finalERA;
            
            _keData.Pierce = CalculatePostERAPierce(_keData.Pierce,_target.ERAData.KEFractionMultiplier);

            // Armor degradation
            float finalArmor = Math.Max(0.0f,
             _target.Armor - (_keData.Pierce / _target.Armor)*_keData.ArmorDegradationFactor);
            finalState.Armor = finalArmor;

            // Calculate final damage
            float finalDamage = Math.Max(0.0f, _keData.Pierce - _target.Armor)*_keData.HealthDamageFactor;
            float finalHealth = Math.Max(0.0f, _target.Health - finalDamage);
            finalState.Health = finalHealth;
            
            return finalState;
        }


        private static float CalculateKEAttenuationSimple(float pierce, float distance, float frictionFactor)
        {
            float finalPierce =  Math.Exp(-frictionFactor*distance)*pierce;
            return finalPierce;
        }
        
        private static float CalculatePostERAPierce(float pierce, float eraFractionMultiplier)
        {
            float finalPierce = pierce*(1-eraFractionMultiplier);
            return finalPierce;
        }
    }
}
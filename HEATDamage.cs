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
            public float ArmorDegradationFactor;
            /// <summary>
            /// Multiplier for health damage
            /// </summary>
            public float HealthDamageFactor;
        }

        private HeatData _heatData;

        public HeatDamage(HeatData d,Damage.Target t):base(EDamageTypes.HEAT, t)
        {
            _heatData = d;
        }

        public override Target CalculateDamage()
        {
            Target finalState = _target;

            // No air friction attenuation as HEAT round detonates on surface of the armor

            // Calculate effects of ERA
            float finalERA = Math.Max(0.0f, _target.ERAData.CurrentValue
             - _heatData.Pierce*_target.ERAData.HeatFractionMultiplier);
            finalState.ERAData.CurrentValue = finalERA;
            
            _heatData.Pierce = CalculatePostERAPierce(_heatData.Pierce,_target.ERAData.HeatFractionMultiplier);

            // Armor degradation
            float finalArmor = Math.Max(0.0f,
             _target.Armor - (_heatData.Pierce / _target.Armor)*_heatData.ArmorDegradationFactor);
            finalState.Armor = finalArmor;

            // Calculate final damage
            float finalDamage = Math.Max(0.0f, _heatData.Pierce - _target.Armor)*_heatData.HealthDamageFactor;
            float finalHealth = Math.Max(0.0f, _target.Health - finalDamage);
            finalState.Health = finalHealth;
            
            return finalState;
        }

        private static float CalculatePostERAPierce(float pierce, float eraFractionMultiplier)
        {
            float finalPierce = pierce*(1-eraFractionMultiplier);
            return finalPierce;
        }
    }
}
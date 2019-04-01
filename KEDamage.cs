using System;
using System.Text;
using System.Diagnostics;

namespace PhysicalDamage.Core
{
    class KEDamage:Damage
    {
        public struct KineticData
        {
            public float Pierce;

            public float Distance;

            public float ArmorKEFactor;

            public float HealthKEFactor;

            public float FrictionFactor;
        }

        private KineticData data;

        public KEDamage(KineticData d,Damage.Target t):base(EDamageTypes.KE, t)
        {
            data = d;
        }

        override public float DealDamage()
        {
            // Calculate attenuation of air friction
            data.Pierce = CalculateKEAttenuationSimple(data.Pierce,data.Distance,data.FrictionFactor);

            // Calculate effects of ERA
            UpdateTargetERA(data.Pierce);
            data.Pierce = CalculatePostERAPierce(data.Pierce,target.ERAData.KEFractionMultiplier);

            // Armor degradation
            UpdateTargetArmor(data.Pierce);

            // Calculate final damage
            float damage2HP = Math.Max(0.0f, data.Pierce - target.Armor)*data.HealthKEFactor;
            return damage2HP;
        }

        #region DamageCalculation

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
        #endregion

        #region UpdateValues

        private void UpdateTargetERA(float piercePostAttenuation)
        {
            float ERAAfterHit = Math.Max(0.0f, target.ERAData.CurrentValue
             - piercePostAttenuation*target.ERAData.KEFractionMultiplier);
            // Use delegate function to update this value to target UnitData class
        }

        private void UpdateTargetArmor(float piercePostERA)
        {
            float ArmorAfterHit = Math.Max(0.0f, target.Armor - piercePostERA*data.ArmorKEFactor);
            // Use delegate function to update this value to target UnitData class
        }
        #endregion
    }
}
using System;
using System.Text;
using System.Diagnostics;

namespace PhysicalDamage.Core
{
    class HEATDamage:Damage
    {
        public struct HEATData
        {
            public float Pierce;

            public float ArmorHEATFactor;

            public float HealthHEATFactor;
        }

        private HEATData data;

        public HEATDamage(HEATData d,Damage.Target t):base(EDamageTypes.HEAT, t)
        {
            data = d;
        }

        override public float DealDamage()
        {
            // No air friction for HEAT round as it detonate on surface of armor

            // Calculate effects of ERA
            UpdateTargetERA(data.Pierce);
            data.Pierce = CalculatePostERAPierce(data.Pierce,target.ERAData.HEATFractionMultiplier);

            // Armor degradation
            UpdateTargetArmor(data.Pierce);

            // Calculate final damage
            float damage2HP = Math.Max(0.0f, data.Pierce - target.Armor)*data.HealthHEATFactor;
            return damage2HP;
        }

        private static float CalculatePostERAPierce(float pierce, float eraFrictionMultiplier)
        {
            float finalPierce = pierce*(1-eraFrictionMultiplier);
            return finalPierce;
        }
        
        #region UpdateValues

        private void UpdateTargetERA(float piercePostAttenuation)
        {
            float ERAAfterHit = Math.Max(0.0f, target.ERAData.CurrentValue
             - piercePostAttenuation*target.ERAData.HEATFractionMultiplier);
            // Use delegate function to update this value to target UnitData class
        }

        private void UpdateTargetArmor(float piercePostERA)
        {
            float ArmorAfterHit = Math.Max(0.0f, target.Armor - piercePostERA*data.ArmorHEATFactor);
            // Use delegate function to update this value to target UnitData class
        }
        #endregion
    }
}
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
        private Damage.Target target;

        public KEDamage(KineticData d,Damage.Target t):base(EDamageTypes.KE)
        {
            data = d;
            target = t;
        }

        override public float DealDamage()
        {
            // Calculate attenuation of air friction
            data.Pierce = CalculateKEAttenuationSimple(data.Pierce,data.Distance,data.FrictionFactor);

            // Calculate effects of ERA
            data.Pierce = CalculatePostERAPierce(data.Pierce,target.ERAData.KEFractionMultiplier);

            // Calculate final damage
            float finalDamage = Math.Max(0.0f, target.Armor - data.Pierce)*data.HealthKEFactor;
            return finalDamage;
        }

        // Then we implement a separate function for armour degradation,
        // use callback function to update target armour value in its data model class

        #region DamageCalculation

        private static float CalculateKEAttenuationSimple(float pierce, float distance, float frictionFactor)
        {
            float finalPierce =  Math.Exp(-frictionFactor*distance)*pierce;
            return finalPierce;
        }
        
        private static float CalculatePostERAPierce(float pierce, float eraFrictionMultiplier)
        {
            float finalPierce = pierce*(1-eraFrictionMultiplier);

            // Use some callback function to update target's era value here

            return finalPierce;
        }
        #endregion
    }
}
using System;
using System.Text;
using System.Diagnostics;

namespace PFW.Damage
{
    public static class PhysicalProjectile
    {
        public float FrictionFactor = 1.0f;
        public float Energy2Pierce = 1.0f;

        public struct Kinetic
        {
            public float Pierce;

            public float ArmorKineticFactor;

            public float HealthKineticFactor;
        }
        public struct Heat
        {
            public float Pierce;

            public float ArmorHeatFactor;

            public float HealthHeatFactor;
        }

        public struct Vehicle
        {
            public float Armor;

            public float Health;
        }

        public struct ERA
        {
            public int Plates;

            public float Bonus;

            public float PiercingFactor;

            public float ExplosiveFactor;
        }

        /// <summary>
        /// Returns how much armor and health is left, possibly if the target has died.
        /// Returns are the following: 0 is Remaining AV, 1 is Remaining HP
        /// </summary>
        public static float[] GetTargetDamage(Kinetic kinetic, Heat heat, Vehicle vehicle, ERA era, float distance)
        {
            float armorPiercingDamage = 0f;
            float heatPiercingDamage = 0f;
            //bool ERABlocksShot = false;

            // Apply KE attenuation
            kinetic.Pierce = CalculateKEAttenuationSimple(kinetic.Pierce,distance, FrictionFactor);

            // Kinetic Armor Degradation
            if (era.Plates < 1 && vehicle.Armor > 0)
            {
                armorPiercingDamage = (kinetic.Pierce / vehicle.Armor) * kinetic.ArmorKineticFactor;
                Debug.WriteLine($"Armor Piercing Damage: {armorPiercingDamage}");
            }

            if (era.Plates > 0)
            {
                armorPiercingDamage = (kinetic.Pierce / era.Bonus) * kinetic.ArmorKineticFactor;
                float eraArmor = era.Bonus - (armorPiercingDamage * era.PiercingFactor);

                if (eraArmor > 0)
                    armorPiercingDamage = 0;
                else if (vehicle.Armor > 0)
                    armorPiercingDamage = ((eraArmor * -1) / vehicle.Armor) * kinetic.ArmorKineticFactor;

            }

            Debug.WriteLine($"Armor Piercing Damage: {armorPiercingDamage}");

            // Heat Armor Degradation

            /*heatPiercingDamage = (era.Plates < 1 && vehicle.Armor > 0) ? 
                                     (heat.Piercing / vehicle.Armor) * heat.ArmorHeatFactor :
                                      heatPiercingDamage;*/

            if (era.Plates < 1 && vehicle.Armor > 0)
            {
                heatPiercingDamage = (heat.Pierce / vehicle.Armor) * heat.ArmorHeatFactor;
                Debug.WriteLine($"Heat Piercing Damage: {heatPiercingDamage}");
            }

            if (era.Plates > 0)
            {
                heatPiercingDamage = (heat.Pierce / era.Bonus) * heat.HealthHeatFactor;
                float eraArmor = era.Bonus - (heatPiercingDamage * era.ExplosiveFactor);

                if (era.Bonus > 0)
                {
                    heatPiercingDamage = 0;
                }
                else if (vehicle.Armor > 0)
                {
                    heatPiercingDamage = ((eraArmor * -1) / vehicle.Armor) * heat.ArmorHeatFactor;
                }
            }

            Debug.WriteLine($"Heat Piercing Damage: {heatPiercingDamage}");



            // Total armor degradation
            var totalDegradation = armorPiercingDamage + heatPiercingDamage;
            vehicle.Armor -= totalDegradation;
            if (vehicle.Armor < 0) vehicle.Armor = 0;
            Debug.WriteLine($"Total Degradation: {totalDegradation}");
            Debug.WriteLine($"Vehicle Armor: {vehicle.Armor}");

            // AP -> HP damage
            float kineticHealthDamage = (kinetic.Pierce - vehicle.Armor) * kinetic.HealthKineticFactor;
            if (kineticHealthDamage < 0) kineticHealthDamage = 0;


            Debug.WriteLine($"Kinetic Health Damage: {kineticHealthDamage}");

            // HE -> HP damage
            float heatHealthDamage = (heat.Pierce - vehicle.Armor) * heat.HealthHeatFactor;
            if (heatHealthDamage < 0) kineticHealthDamage = 0;


            if (vehicle.Armor == 0 && era.Plates > 0)
            {
                kineticHealthDamage -= era.Bonus;
                heatHealthDamage -= era.Bonus;
            }

            era.Plates = era.Plates - 1;

            Debug.WriteLine($"ERA Plates: {era.Plates}");

            // Total health reduction
            vehicle.Health = vehicle.Health - (kineticHealthDamage + heatHealthDamage);

            // just to prevent any bugs, i'm resetting any negative values

            if (era.Plates < 0)
                era.Plates = 0;

            if (vehicle.Armor < 0)
                vehicle.Armor = 0;

            if (vehicle.Health < 0)
                vehicle.Health = 0;

            Math.Round(vehicle.Armor, 2);
            Math.Round(vehicle.Health, 2);

            return new float[] { vehicle.Armor, vehicle.Health, era.Plates };
        }

        public static float CalculateKEAttenuationSimple(float pierce, float distance, float frictionFactor)
        {
            float finalEnergy =  Math.Exp(-frictionFactor*distance)*pierce;
            return finalEnergy;
        }

        public static float CalculateKEAttenuationRealistic(float pierce, float distance, float frictionK, float mass, float initialVelocity)
        {
            float finalEnergy = Math.Exp(-2*frictionK*distance/mass)*pierce;
            return finalEnergy;
        }

        /// <summary>
        /// Given the mass and initial velocity of a particular projectile,
        /// calculate its initial kinetic energy then convert to in-game piercing power value
        /// </summary>
        public static float CalculateInitialEnergy(float mass, float initialVelocity)
        {
            float energy =  0.5*mass*initialVelocity*initialVelocity; // KE = 1/2 mv^2
            float pierce = energy*Energy2Pierce;
            return pierce;
        }
    }


}
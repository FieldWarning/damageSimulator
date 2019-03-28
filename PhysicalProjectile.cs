using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Damage_Simulation_Algo.DamageClasses
{
    static class PhysicalProjectile
    {
        public struct Kinetic
        {
            public float Piercing;
            public float ArmorKineticMultiplier;
            public float HealthKineticMultiplier;
        }

        public struct Heat
        {
            public float Piercing;
            public float ArmorHeatMultiplier;
            public float HealthHeatMultiplier;
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
            public float PiercingMultiplier;
            public float ExplosiveMultiplier;
        }

        /// <summary>
        /// Returns how much armor and health is left, possibly if the target has died.
        /// Returns are the following: 0 is Remaining AV, 1 is Remaining HP
        /// </summary>
        public static float[] GetTargetDamage(Kinetic kinetic, Heat heat, Vehicle vehicle, ERA era)
        {
            float armorPiercingDamage = 0f;
            float heatPiercingDamage = 0f;
            bool ERABlocksShot = false;

            // Kinetic Armor Degradation
            if (era.Plates < 1 && vehicle.Armor > 0)
            {
                armorPiercingDamage = (kinetic.Piercing / vehicle.Armor) * kinetic.ArmorKineticMultiplier;
                Debug.WriteLine($"Armor Piercing Damage: {armorPiercingDamage}");
            }

            if (era.Plates > 0)
            {
                float eraArmor = era.Bonus;
                armorPiercingDamage = (kinetic.Piercing / era.Bonus) * kinetic.ArmorKineticMultiplier;
                eraArmor = era.Bonus - (armorPiercingDamage * era.PiercingMultiplier);

                if (eraArmor > 0)
                {
                    armorPiercingDamage = 0;
                }
                else if (vehicle.Armor > 0)
                {
                    armorPiercingDamage = ((eraArmor * -1) / vehicle.Armor) * kinetic.ArmorKineticMultiplier;
                }

            }

            Debug.WriteLine($"Armor Piercing Damage: {armorPiercingDamage}");

            // Heat Armor Degradation

            /*heatPiercingDamage = (era.Plates < 1 && vehicle.Armor > 0) ? 
                                     (heat.Piercing / vehicle.Armor) * heat.ArmorHeatMultiplier :
                                      heatPiercingDamage;*/

            if (era.Plates < 1 && vehicle.Armor > 0)
            {
                heatPiercingDamage = (heat.Piercing / vehicle.Armor) * heat.ArmorHeatMultiplier;
                Debug.WriteLine($"Heat Piercing Damage: {heatPiercingDamage}");
            }

            if (era.Plates > 0)
            {
                float eraArmor = era.Bonus;
                heatPiercingDamage = (heat.Piercing / era.Bonus) * heat.HealthHeatMultiplier;
                eraArmor = era.Bonus - (heatPiercingDamage * era.ExplosiveMultiplier);

                if (era.Bonus > 0)
                {
                    heatPiercingDamage = 0;
                }
                else if (vehicle.Armor > 0)
                {
                    heatPiercingDamage = ((eraArmor * -1) / vehicle.Armor) * heat.ArmorHeatMultiplier;
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
            float kineticHealthDamage = (kinetic.Piercing - vehicle.Armor) * kinetic.HealthKineticMultiplier;
            if (kineticHealthDamage < 0) kineticHealthDamage = 0;
            

            Debug.WriteLine($"Kinetic Health Damage: {kineticHealthDamage}");
            
            // HE -> HP damage
            float heatHealthDamage = (heat.Piercing - vehicle.Armor) * heat.HealthHeatMultiplier;
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
            {
                era.Plates = 0;
            }
            if (vehicle.Armor < 0)
            {
                vehicle.Armor = 0;
            }
            if (vehicle.Health < 0)
            {
                vehicle.Health = 0;
            }

            Math.Round(vehicle.Armor, 2);
            Math.Round(vehicle.Health, 2);

            return new float[] {vehicle.Armor, vehicle.Health, era.Plates};
        }
    }
}


using System;
using System.Text;
using System.Diagnostics;

namespace PhysicalDamage.Core
{
    public static class PhysicalProjectile
    {
        public struct Kinetic
        {
            public float Piercing;

            public float ArmorKineticFactor;

            public float HealthKineticFactor;
        }

        public struct Heat
        {
            public float Piercing;

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
        /// Calculates the amount of damage for *blank*
        /// </summary>
        /// <param name="kinetic">Kinetic variables of equation.</param>
        /// <param name="heat">Heat variables.</param>
        /// <param name="vehicle">Vehicle variables.</param>
        /// <param name="era">ERA variables.</param>
        /// <returns><see cref="System.Single">float[3]</see></returns>
        public static float[] GetTargetDamage(Kinetic kinetic, Heat heat, Vehicle vehicle, ERA era)
        {
            float armorPiercingDamage = 0f;
            float heatPiercingDamage = 0f;
            //bool ERABlocksShot = false;

            // Kinetic Armor Degradation
            if (era.Plates < 1 && vehicle.Armor > 0)
            {
                armorPiercingDamage = (kinetic.Piercing / vehicle.Armor) * kinetic.ArmorKineticFactor;
                Debug.WriteLine($"Armor Piercing Damage: {armorPiercingDamage}");
                Console.WriteLine($"Armor Piercing Damage: {armorPiercingDamage}");
            }

            if (era.Plates > 0)
            {
                armorPiercingDamage = (kinetic.Piercing / era.Bonus) * era.PiercingFactor;
                float eraArmor = era.Bonus - (armorPiercingDamage * era.PiercingFactor);




                if (eraArmor > 0)
                    armorPiercingDamage -= eraArmor;
                if (eraArmor < 0)
                    eraArmor = 0;

                if (vehicle.Armor > 0)
                    armorPiercingDamage = ( kinetic.Piercing / vehicle.Armor) * kinetic.ArmorKineticFactor;

            }

            Debug.WriteLine($"Armor Piercing Damage: {armorPiercingDamage}");

            // Heat Armor Degradation

            if (era.Plates < 1 && vehicle.Armor > 0)
            {
                heatPiercingDamage = (heat.Piercing / vehicle.Armor) * heat.ArmorHeatFactor;
                
            }

            if (era.Plates > 0)
            {
                heatPiercingDamage = (heat.Piercing / era.Bonus) * heat.HealthHeatFactor;
                float eraArmor = era.Bonus - (heatPiercingDamage * era.ExplosiveFactor);



                if (eraArmor > 0)
                    heatPiercingDamage -= eraArmor;
                if (eraArmor < 0)
                    eraArmor = 0;
                else if (vehicle.Armor > 0)
                    heatPiercingDamage = (heat.Piercing / vehicle.Armor) * heat.ArmorHeatFactor;
            }

            Debug.WriteLine($"Heat Piercing Damage: {heatPiercingDamage}");
            Console.WriteLine($"Heat Piercing Damage: {heatPiercingDamage}");

            if (armorPiercingDamage < 0) armorPiercingDamage = 0;
            if (heatPiercingDamage < 0) heatPiercingDamage = 0;

            // Total armor degradation
            var totalDegradation = armorPiercingDamage + heatPiercingDamage;
            vehicle.Armor -= totalDegradation;
            if (vehicle.Armor < 0) vehicle.Armor = 0;
            Debug.WriteLine($"Total Degradation: {totalDegradation}");
            Debug.WriteLine($"Vehicle Armor: {vehicle.Armor}");
            Console.WriteLine($"Total Degradation: {totalDegradation}");
            Console.WriteLine($"Vehicle Armor: {vehicle.Armor}");

            // HP damage

            // AP -> HP damage
            float kineticHealthDamage = (kinetic.Piercing - vehicle.Armor) * kinetic.HealthKineticFactor;
            


            Debug.WriteLine($"Kinetic Health Damage: {kineticHealthDamage}");
            Console.WriteLine($"Kinetic Health Damage: {kineticHealthDamage}");

            // HE -> HP damage
            float heatHealthDamage = (heat.Piercing - vehicle.Armor) * heat.HealthHeatFactor;
            


            if (vehicle.Armor == 0 && era.Plates > 0)
            {
                kineticHealthDamage -= era.Bonus;
                heatHealthDamage -= era.Bonus;
            }

            era.Plates = era.Plates - 1;

            Debug.WriteLine($"ERA Plates: {era.Plates}");
            Console.WriteLine($"ERA Plates: {era.Plates}");

            // Total health reduction
            if (kineticHealthDamage < 0) kineticHealthDamage = 0;
            if (heatHealthDamage < 0) heatHealthDamage = 0;
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

            Console.WriteLine($"After shot: {vehicle.Armor} AV, {vehicle.Health} HP, {era.Plates} ERA Plates");

            return new float[] { vehicle.Armor, vehicle.Health, era.Plates };
        }
    }
}
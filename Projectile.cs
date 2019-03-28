using System;
using System.Diagnostics;

using static ProjectileVariables;

public class Program
{
    public static void Main()
    {
        var era = new ERA() { Bonus = 15.0f, ExplosiveFactor = 93.0f, PiercingFactor = 12.203f, Plates = 2 };
        var heat = new Heat { Piercing = 314f, ArmorHeatFactor = 122f, HealthHeatFactor = 213f };
        var kinetic = new Kinetic { Piercing = 32f, ArmorKineticFactor = 92f, HealthKenticFactor = 29f };
        var vehicle = new Vehicle { VehicleArmor = 600f, VehicleHealth = 1000f };
        var projectile = PhysicalProjectile.GetTargetDamage(kinetic, heat, vehicle, era);

        Console.WriteLine("Projectile: {0}{1}{2}", projectile[0], projectile[1], projectile[2]);
        Console.ReadKey();
    }
}

/// <summary>
/// 
/// </summary>
public static class PhysicalProjectile
{
    /// <summary>
    ///     Returns how much armor and health is left, possibly if the target has died.
    ///     Returns are the following: 0 is Remaining AV, 1 is Remaining HP
    /// </summary>
    public static float[] GetTargetDamage(Kinetic kinetic, Heat heat, Vehicle vehicle, ERA era)
    {
        var armorPiercingDamage = 0f;
        var heatPiercingDamage = 0f;

        // Kinetic VehicleArmor Degradation
        if (era.Plates < 1 && vehicle.VehicleArmor > 0)
        {
            armorPiercingDamage = kinetic.Piercing / vehicle.VehicleArmor * kinetic.ArmorKineticFactor;
            Debug.WriteLine($"VehicleArmor Piercing Damage: {armorPiercingDamage}");
        }

        if (era.Plates > 0)
        {
            armorPiercingDamage = kinetic.Piercing / era.Bonus * kinetic.ArmorKineticFactor;
            var eraArmor = era.Bonus - armorPiercingDamage * era.PiercingFactor;

            if (eraArmor > 0)
                armorPiercingDamage = 0;
            else if (vehicle.VehicleArmor > 0)
                armorPiercingDamage = eraArmor * -1 / vehicle.VehicleArmor * kinetic.ArmorKineticFactor;
        }

        Debug.WriteLine($"VehicleArmor Piercing Damage: {armorPiercingDamage}");

        // Heat VehicleArmor Degradation

        /*heatPiercingDamage = (era.Plates < 1 && vehicle.VehicleArmor > 0) ? 
                                 (heat.Piercing / vehicle.VehicleArmor) * heat.ArmorHeatFactor :
                                  heatPiercingDamage;*/
        if (era.Plates < 1 && vehicle.VehicleArmor > 0)
        {
            heatPiercingDamage = heat.Piercing / vehicle.VehicleArmor * heat.ArmorHeatFactor;
            Debug.WriteLine($"Heat Piercing Damage: {heatPiercingDamage}");
        }

        if (era.Plates > 0)
        {
            heatPiercingDamage = heat.Piercing / era.Bonus * heat.HealthHeatFactor;
            var eraArmor = era.Bonus - heatPiercingDamage * era.ExplosiveFactor;

            if (era.Bonus > 0)
                heatPiercingDamage = 0;
            else if (vehicle.VehicleArmor > 0)
                heatPiercingDamage = eraArmor * -1 / vehicle.VehicleArmor * heat.ArmorHeatFactor;
        }

        Debug.WriteLine($"Heat Piercing Damage: {heatPiercingDamage}");

        // Total VehicleArmor Degradation
        var totalDegradation = armorPiercingDamage + heatPiercingDamage;
        if ((vehicle.VehicleArmor -= totalDegradation) < 0)
            vehicle.VehicleArmor = 0;
        Debug.WriteLine($"Total Degradation: {totalDegradation}");
        Debug.WriteLine($"Vehicle VehicleArmor: {vehicle.VehicleArmor}");

        // AP -> HP damage
        var kineticHealthDamage = (kinetic.Piercing - vehicle.VehicleArmor) * kinetic.HealthKenticFactor;
        kineticHealthDamage = kineticHealthDamage < 0 ? 0 : kineticHealthDamage;

        Debug.WriteLine($"Kinetic VehicleHealth Damage: {kineticHealthDamage}");

        // HE -> HP damage
        var heatHealthDamage = (heat.Piercing - vehicle.VehicleArmor) * heat.HealthHeatFactor;
        heatHealthDamage = heatHealthDamage < 0 ? 0 : heatHealthDamage;

        kineticHealthDamage -= vehicle.VehicleArmor < 1 && era.Plates > 0 ? era.Bonus : 0;

        if (vehicle.VehicleArmor < 1 && era.Plates > 0)
            kineticHealthDamage -= era.Bonus = heatHealthDamage -= era.Bonus;

        Debug.WriteLine($"ERA Plates: {era.Plates}");

        // Total VehicleHealth Reduction
        vehicle.VehicleHealth = vehicle.VehicleHealth - (kineticHealthDamage + heatHealthDamage);

        // just to prevent any bugs, i'm resetting any negative values
        era.Plates = (era.Plates -= 1) < 0 ? 0 : era.Plates;
        vehicle.VehicleArmor = (vehicle.VehicleArmor < 0) ? 0 : vehicle.VehicleArmor;
        vehicle.VehicleHealth = (vehicle.VehicleHealth < 0) ? 0 : vehicle.VehicleHealth;

        var roundedVehicleArmor = (float)Math.Round((decimal)vehicle.VehicleArmor, 2);
        var roundedVehicleHealth = (float)Math.Round((decimal)vehicle.VehicleHealth, 2);

        return new[] { roundedVehicleArmor, roundedVehicleHealth, era.Plates };

    }
}

public static class ProjectileVariables
{
    public struct ERA
    {
        public int Plates { get; set; }

        public float Bonus { get; set; }

        public float PiercingFactor { get; set; }

        public float ExplosiveFactor { get; set; }
    }

    public struct Heat
    {
        public float Piercing { get; set; }
        public float ArmorHeatFactor { get; set; }
        public float HealthHeatFactor { get; set; }
    }

    public struct Kinetic
    {
        public float Piercing { get; set; }
        public float ArmorKineticFactor { get; set; }
        public float HealthKenticFactor { get; set; }
    }

    public struct Vehicle
    {
        public float VehicleArmor { get; set; }
        public float VehicleHealth { get; set; }
    }
}
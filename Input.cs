using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using PhysicalDamage.Core;
using static PhysicalDamage.Core.PhysicalProjectile;

namespace PhysicalDamage
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var era = new ERA { Bonus = 2.0f, ExplosiveFactor = 1.0f, PiercingFactor = 1.0f, Plates = 1 };
            var heat = new Heat { Piercing = 0f, ArmorHeatFactor = 0.5f, HealthHeatFactor = 1f };
            var kinetic = new Kinetic { Piercing = 20f, ArmorKineticFactor = 1f, HealthKineticFactor = 0.5f };
            var vehicle = new Vehicle { Armor = 20f, Health = 10f };


            var dmg = PhysicalProjectile.GetTargetDamage(kinetic, heat, vehicle, era);
            var armor = dmg[0];
            var health = dmg[1];
            var plates = dmg[2];


            /* long startingTime = Stopwatch.GetTimestamp();

            for (int i = 0; i < 5000; i++)
            {
                
            }

            long endingTime = Stopwatch.GetTimestamp();
            long elapsedTime = endingTime - startingTime; 

            double elapsedSeconds = elapsedTime * (1.0 / Stopwatch.Frequency); 
            
             Console.WriteLine($"5000 Cycles -> {elapsedSeconds}ms"); 
            */

            Console.WriteLine($"Armor: {armor} - HP: {health} - ERA Plates: {plates}");

           


            Console.ReadKey();
            
        }
    }
}
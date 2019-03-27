using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Damage_Simulation_Algo
{
    public partial class Checker : Form
    {
        
        public Checker()
        {
            InitializeComponent();
        }

        private void Checker_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Made by flkXI, this is open source material and belongs in the Project: Field Warning project, this program falls within it's license");

            // defaults

            

            // AP Weapon Value
            APWVTB.Text = "10";

            // AP -> AV Damage Multiplier
            APAVDMTB.Text = "1";

            // AP -> HP Damage Multiplier
            APHPDMTB.Text = "0.5";

            // HE Weapon Value
            HEWVTB.Text = "10";

            // HE -> AV Damage Multiplier
            HEAVTB.Text = "0.5";

            // HE -> HP Damage Multiplier
            HEHPDMTB.Text = "1";

            // Vehicle AV
            VAVTB.Text = "15";

            // Vehicle Explosive Reactive Armor (add's AV to vehicle AV, can only tank 1 shot)
            VERATB.Text = "2";
            ERA_Count.Text = "0";
            ERA_AP_MultiplierTB.Text = "1";
            ERA_HE_MultiplierTB.Text = "1";

            // Vehicle HP 
            VHPTB.Text = "10";

            // DPS Variables
            AccuracyTB.Text = "50";
            ReloadTB.Text = "0";

        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            if (DetailsClearTick.Checked)
            {
                DetailsBox.Items.Clear();
            }
            if (LaserTick.Checked)
            {
                Algorithm_LAS_CLD();
            }
            else if (SimpleAlgoTick.Checked)
            {
                AlgorithmSimple();
            }
            else if (AccuracyTick.Checked)
            {
                AlgorithmComplex_AP_HE_ERA();
            }
            else
            {
                Algorithm();
            }

        }

        private void Algorithm() 
        {
            try
            {
                // Base Variables

                decimal AP = Convert.ToInt32(APWVTB.Text);
                decimal HE = Convert.ToInt32(HEWVTB.Text);
                decimal Base_AV = Convert.ToInt32(VAVTB.Text);
                decimal Base_HP = Convert.ToInt32(VHPTB.Text);
                
                int Shots_Taken = 0;

                // ERA variables

                int ERA = Convert.ToInt32(ERA_Count.Text);
                decimal ERA_Bonus = Convert.ToDecimal(VERATB.Text);
                decimal ERA_AP_Multiplier = Convert.ToDecimal(ERA_AP_MultiplierTB.Text);
                decimal ERA_HE_Multiplier = Convert.ToDecimal(ERA_HE_MultiplierTB.Text);

                if (ERA > 1)
                {
                    Console.WriteLine("ERAAAA");
                    
                }

                // Process Variables

                decimal APAV_Multiplier = Convert.ToDecimal(APAVDMTB.Text);
                decimal APHP_Multiplier = Convert.ToDecimal(APHPDMTB.Text);

                decimal HEAV_Multiplier = Convert.ToDecimal(HEAVTB.Text);
                decimal HEHP_Multiplier = Convert.ToDecimal(HEHPDMTB.Text);

                decimal Current_AV = Base_AV;
                decimal Current_HP = Base_HP;

                decimal AP_AV_Damage = 0;
                decimal HE_AV_Damage = 0;

                // Base stats to list

                DetailsBox.Items.Add($"Shots taken: {Shots_Taken} - AV: {Math.Round(Current_AV, 2)} - HP: {Math.Round(Current_HP, 2)} - ERA Plates: {ERA} - ERA Bonus: {ERA_Bonus}");

                while (Current_HP > 0)
                {
                    var watch = Stopwatch.StartNew();

                    // add the shot to data
                    Shots_Taken++;


                    // using try because of ERA mechanics
                    try
                    {
                        // AP integrity reduction
                        AP_AV_Damage = 0;

                        if (ERA >= 1)
                        {
                            ERA_Bonus = Convert.ToDecimal(VERATB.Text);
                            if (Current_AV > 0)
                            {
                                if (ERABlocksAPTick.Checked)
                                {
                                    decimal Current_ERA_Armor = ERA_Bonus;
                                    AP_AV_Damage = (AP / ERA_Bonus) * APAV_Multiplier;
                                    Current_ERA_Armor = ERA_Bonus - (AP_AV_Damage * ERA_AP_Multiplier);
                                    if (Current_ERA_Armor < 0)
                                    {
                                        decimal Post_ERA_AP = Current_ERA_Armor * (-1);

                                        AP_AV_Damage = (Post_ERA_AP / Current_AV) * APAV_Multiplier;
                                    }
                                    else if (Current_ERA_Armor > 0)
                                    {
                                        AP_AV_Damage = 0;
                                    }
                                }
                                else
                                {
                                    AP_AV_Damage = (AP / Current_AV) * APAV_Multiplier;
                                }
                            }
                            else
                            {
                                AP_AV_Damage = AP * APAV_Multiplier;
                            }

                        }
                        else
                        {

                            ERA_Bonus = 0;
                            if (Current_AV > 0)
                            {
                                AP_AV_Damage = (AP / Current_AV) * APAV_Multiplier;
                            }
                            else
                            {
                                AP_AV_Damage = AP * APAV_Multiplier;
                            } 

                        } 

                        // HE integrity reduction
                        HE_AV_Damage = 0;

                        if (ERA >= 1)
                        {
                            ERA_Bonus = Convert.ToDecimal(VERATB.Text);

                            
                            if (Current_AV > 0)
                            {
                                if (ERABlocksHETick.Checked)
                                {
                                    decimal Current_ERA_Armor = ERA_Bonus;
                                    HE_AV_Damage = (HE / ERA_Bonus) * HEAV_Multiplier;
                                    Current_ERA_Armor = ERA_Bonus - (HE_AV_Damage * ERA_HE_Multiplier);
                                    if (Current_ERA_Armor < 0)
                                    {
                                        decimal Post_ERA_HE = Current_ERA_Armor * (-1);

                                        HE_AV_Damage = (Post_ERA_HE / Current_AV) * HEAV_Multiplier;
                                    }
                                    else if (Current_ERA_Armor > 0)
                                    {
                                        HE_AV_Damage = 0;
                                    }
                                }
                                else
                                {
                                    HE_AV_Damage = (HE / Current_AV) * HEAV_Multiplier;
                                }
                            }
                            else
                            {
                                HE_AV_Damage = HE * HEAV_Multiplier;
                            }



                        }
                        else if (ERA <= 0)
                        {

                            ERA_Bonus = 0;
                            if (Current_AV > 0)
                            {
                                HE_AV_Damage = (HE / Current_AV) * HEAV_Multiplier;
                            }
                            else
                            {
                                HE_AV_Damage = HE * HEAV_Multiplier;
                            }

                        }

                        ERA = ERA - 1;
                    }
                    catch
                    {
                                         
                    }

                    

                    // Total Integrity reduction
                    decimal FULL_AV_Damage = AP_AV_Damage + HE_AV_Damage;
                    Current_AV = Current_AV - FULL_AV_Damage;
                    if (Current_AV < 0)
                    {
                        Current_AV = 0;
                    }

                    // AP -> HP Damage
                    decimal AP_HP_Damage = (AP - Current_AV) * APHP_Multiplier;
                    if (AP_HP_Damage < 0)
                    {
                        AP_HP_Damage = 0;
                    }

                    // HE -> HP Damage
                    decimal HE_HP_Damage = (HE - Current_AV) * HEHP_Multiplier;
                    if (HE_HP_Damage < 0)
                    {
                        HE_HP_Damage = 0;
                    }

                    // Total HP Damage
                    decimal FULL_HP_Damage = AP_HP_Damage + HE_HP_Damage;
                    if (FULL_HP_Damage < 0)
                    {
                        FULL_HP_Damage = 0;
                    }
                    else
                    {
                        Current_HP = Current_HP - FULL_HP_Damage;
                    }

                    watch.Stop();
                    var executiontime = watch.ElapsedMilliseconds;

                    if (executiontime == 0)
                    {
                        WatchLabel.Text = "Execution time: <1 ms";
                    }
                    else
                    {
                        WatchLabel.Text = "Execution time: " + executiontime.ToString() + " ms";
                    }

                    

                    // Shows the details for each shot

                    if (ERA < 0)
                    {
                        ERA = 0;
                    }
                    if (Current_HP < 0)
                    {
                        Current_HP = 0;
                    }

                    DetailsBox.Items.Add($"Shots taken: {Shots_Taken} - AV: {Math.Round(Current_AV, 2)} - HP: {Math.Round(Current_HP, 2)} - ERA Plates: {ERA} - ERA Bonus at shot {Shots_Taken}: {ERA_Bonus}");
                }

                if (Current_HP < 0)
                {
                    // make unit die in actual game

                    Current_HP = 0;
                    DetailsBox.Items.Add($"ITS DEAD! Shots taken: {Shots_Taken} - AV: {Math.Round(Current_AV, 2)} - HP: {Current_HP}");


                }
                
                ShotCount.Text = $"Shots needed to bring enemy to 0 HP: {Shots_Taken}";

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } 



        }

        private void Algorithm_LAS_CLD()
        {
            // load variables

            decimal laser_damage = Convert.ToDecimal(LaserDAMTB.Text);
            decimal laser_AV_Multiplier = Convert.ToDecimal(LASAVTB.Text);
            decimal laser_HP_Multiplier = Convert.ToDecimal(LASHPTB.Text);

            decimal Base_AV = Convert.ToInt32(VAVTB.Text);
            decimal Base_HP = Convert.ToInt32(VHPTB.Text);

            // ERA variables

            int ERA = Convert.ToInt32(ERA_Count.Text);
            decimal ERA_Bonus = Convert.ToDecimal(VERATB.Text);
            decimal curr_era_bonus = ERA_Bonus;
            decimal ERA_AP_Multiplier = Convert.ToDecimal(ERA_AP_MultiplierTB.Text);
            decimal ERA_HE_Multiplier = Convert.ToDecimal(ERA_HE_MultiplierTB.Text);
            decimal ERA_LAS_Multiplier = Convert.ToDecimal(ERALASTB.Text);

            // process variables

            decimal Current_AV = Base_AV;
            decimal Current_HP = Base_HP;
            decimal AV_Damage = 0;
            decimal HP_Damage = 0;

            int Overheat_ms = Convert.ToInt32(HeaterTB.Text) * 1000;
            int Cooldown_ms = Convert.ToInt32(las_cooldownTB.Text) * 1000;
            int Current_Heat = 0;
            // bool isFiring = true;

            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                while (Current_HP > 0)
                {
                    while (/* isFiring && */ Current_Heat < Overheat_ms && Current_HP > 0)
                    {
                        Current_Heat = Current_Heat + 1000;
                        // Thread.Sleep(1000);

                        // era plate pop
                        if (ERA > 1)
                        {
                            decimal era_dmg = (laser_damage / curr_era_bonus) * ERA_LAS_Multiplier;
                            curr_era_bonus = curr_era_bonus - era_dmg;
                            if (curr_era_bonus <= curr_era_bonus - (curr_era_bonus * (10 / 100)))
                            {
                                ERA = ERA - 1;
                                if (ERA < 0)
                                {
                                    ERA = 0;
                                }

                                DetailsBox.Items.Add($"Seconds Fired: {Current_Heat / 1000} - ERA value: {Math.Round(ERA_Bonus, 2)} - ERA plates: {ERA}");
                            }
                        }
                        else
                        {

                            // AV degradation
                            if (Current_AV > 0)
                            {
                                AV_Damage = (laser_damage / Current_AV) * laser_AV_Multiplier;
                                Current_AV = Current_AV - AV_Damage;


                            }
                            
                            if (Current_AV < 0)
                            {
                                Current_AV = 0;
                            }

                            // HP damage
                            if (laser_damage > Current_AV)
                            {
                                decimal pen_damage = laser_damage - Current_AV;
                                HP_Damage = HP_Damage + pen_damage;

                                Current_HP = Current_HP - (pen_damage * laser_HP_Multiplier);
                            }
                            if (Current_HP <= 0)
                            {
                                Current_HP = 0;
                                DetailsBox.Items.Add("ITS DEAD!");
                            }

                            DetailsBox.Items.Add($"Seconds Fired: {Current_Heat / 1000} - AV value: {Math.Round(Current_AV, 2)} - HP value: {Math.Round(Current_HP, 2)}");
                        }

                    }

                    if (Current_Heat >= Overheat_ms)
                    {
                        DetailsBox.Items.Add($"WEAPON OVERHEAT, Cooldown: {Cooldown_ms / 1000}");
                        
                        // execute cooldown
                        Current_Heat = 0;
                    }
                    
                    
                }

                stopwatch.Stop();
                string elapsedms = stopwatch.ElapsedMilliseconds.ToString();
                WatchLabel.Text = elapsedms + " ms";


            }
            catch
            {

            }

            

        }

        private void AlgorithmComplex_AP_HE_ERA()
        {
            ShotCount.Text = "CALCULATING, Please wait";

            try
            {
                // Base Variables

                decimal AP = Convert.ToInt32(APWVTB.Text);
                decimal HE = Convert.ToInt32(HEWVTB.Text);
                decimal Base_AV = Convert.ToInt32(VAVTB.Text);
                decimal Base_HP = Convert.ToInt32(VHPTB.Text);

                int Shots_Taken = 0;

                // ERA variables

                int ERA = Convert.ToInt32(ERA_Count.Text);
                decimal ERA_Bonus = Convert.ToDecimal(VERATB.Text);
                decimal ERA_AP_Multiplier = Convert.ToDecimal(ERA_AP_MultiplierTB.Text);
                decimal ERA_HE_Multiplier = Convert.ToDecimal(ERA_HE_MultiplierTB.Text);

                if (ERA > 1)
                {
                    Console.WriteLine("ERAAAA");

                }

                // Process Variables

                decimal APAV_Multiplier = Convert.ToDecimal(APAVDMTB.Text);
                decimal APHP_Multiplier = Convert.ToDecimal(APHPDMTB.Text);

                decimal HEAV_Multiplier = Convert.ToDecimal(HEAVTB.Text);
                decimal HEHP_Multiplier = Convert.ToDecimal(HEHPDMTB.Text);

                decimal Current_AV = Base_AV;
                decimal Current_HP = Base_HP;

                decimal AP_AV_Damage = 0;
                decimal HE_AV_Damage = 0;


                //accuracy vairables
                int Hit_Chance = Convert.ToInt32(AccuracyTB.Text);
                int Reload_Time = Convert.ToInt32(ReloadTB.Text);


                // Base stats to list

                DetailsBox.Items.Add($"Shots taken: {Shots_Taken} - AV: {Math.Round(Current_AV, 2)} - HP: {Math.Round(Current_HP, 2)} - ERA Plates: {ERA} - ERA Bonus: {ERA_Bonus}");

                while (Current_HP > 0)
                {
                    var watch = Stopwatch.StartNew();
                    watch.Start();

                    // Accuracy Roll

                    // The random number provider.
                    RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();

                    // Return a random integer between a min and max value.
                    int RandomN(int min, int max)
                    {
                        uint scale = uint.MaxValue;
                        while (scale == uint.MaxValue)
                        {
                            // Get four random bytes.
                            byte[] four_bytes = new byte[4];
                            Rand.GetBytes(four_bytes);

                            // Convert that into an uint.
                            scale = BitConverter.ToUInt32(four_bytes, 0);
                        }

                        // Add min to the scaled difference between max and min.
                        return (int)(min + (max - min) * (scale / (double)uint.MaxValue));
                    }

                    int finalrn = RandomN(1, 100);

                    // reload time
                    Thread.Sleep(Reload_Time);

                    bool isHit = false;

                    if (finalrn <= Hit_Chance)
                    {
                        isHit = true;

                        // add the shot to data
                        Shots_Taken++;


                        // using try because of ERA mechanics
                        try
                        {
                            // AP integrity reduction
                            AP_AV_Damage = 0;

                            if (ERA >= 1)
                            {
                                ERA_Bonus = Convert.ToDecimal(VERATB.Text);
                                if (Current_AV > 0)
                                {
                                    if (ERABlocksAPTick.Checked)
                                    {
                                        decimal Current_ERA_Armor = ERA_Bonus;
                                        AP_AV_Damage = (AP / ERA_Bonus) * APAV_Multiplier;
                                        Current_ERA_Armor = ERA_Bonus - (AP_AV_Damage * ERA_AP_Multiplier);
                                        if (Current_ERA_Armor < 0)
                                        {
                                            decimal Post_ERA_AP = Current_ERA_Armor * (-1);

                                            AP_AV_Damage = (Post_ERA_AP / Current_AV) * APAV_Multiplier;
                                        }
                                        else if (Current_ERA_Armor > 0)
                                        {
                                            AP_AV_Damage = 0;
                                        }
                                    }
                                    else
                                    {
                                        AP_AV_Damage = (AP / Current_AV) * APAV_Multiplier;
                                    }
                                }
                                else
                                {
                                    AP_AV_Damage = AP * APAV_Multiplier;
                                }

                            }
                            else
                            {

                                ERA_Bonus = 0;
                                if (Current_AV > 0)
                                {
                                    AP_AV_Damage = (AP / Current_AV) * APAV_Multiplier;
                                }
                                else
                                {
                                    AP_AV_Damage = AP * APAV_Multiplier;
                                }

                            }

                            // HE integrity reduction
                            HE_AV_Damage = 0;

                            if (ERA >= 1)
                            {
                                ERA_Bonus = Convert.ToDecimal(VERATB.Text);


                                if (Current_AV > 0)
                                {
                                    if (ERABlocksHETick.Checked)
                                    {
                                        decimal Current_ERA_Armor = ERA_Bonus;
                                        HE_AV_Damage = (HE / ERA_Bonus) * HEAV_Multiplier;
                                        Current_ERA_Armor = ERA_Bonus - (HE_AV_Damage * ERA_HE_Multiplier);
                                        if (Current_ERA_Armor < 0)
                                        {
                                            decimal Post_ERA_HE = Current_ERA_Armor * (-1);

                                            HE_AV_Damage = (Post_ERA_HE / Current_AV) * HEAV_Multiplier;
                                        }
                                        else if (Current_ERA_Armor > 0)
                                        {
                                            HE_AV_Damage = 0;
                                        }
                                    }
                                    else
                                    {
                                        HE_AV_Damage = (HE / Current_AV) * HEAV_Multiplier;
                                    }
                                }
                                else
                                {
                                    HE_AV_Damage = HE * HEAV_Multiplier;
                                }



                            }
                            else if (ERA <= 0)
                            {

                                ERA_Bonus = 0;
                                if (Current_AV > 0)
                                {
                                    HE_AV_Damage = (HE / Current_AV) * HEAV_Multiplier;
                                }
                                else
                                {
                                    HE_AV_Damage = HE * HEAV_Multiplier;
                                }

                            }

                            ERA = ERA - 1;
                        }
                        catch
                        {

                        }



                        // Total Integrity reduction
                        decimal FULL_AV_Damage = AP_AV_Damage + HE_AV_Damage;
                        Current_AV = Current_AV - FULL_AV_Damage;
                        if (Current_AV < 0)
                        {
                            Current_AV = 0;
                        }

                        // AP -> HP Damage
                        decimal AP_HP_Damage = (AP - Current_AV) * APHP_Multiplier;
                        if (AP_HP_Damage < 0)
                        {
                            AP_HP_Damage = 0;
                        }

                        // HE -> HP Damage
                        decimal HE_HP_Damage = (HE - Current_AV) * HEHP_Multiplier;
                        if (HE_HP_Damage < 0)
                        {
                            HE_HP_Damage = 0;
                        }

                        // Total HP Damage
                        decimal FULL_HP_Damage = AP_HP_Damage + HE_HP_Damage;
                        if (FULL_HP_Damage < 0)
                        {
                            FULL_HP_Damage = 0;
                        }
                        else
                        {
                            Current_HP = Current_HP - FULL_HP_Damage;
                        }

                        watch.Stop();
                        var executiontime0 = watch.ElapsedMilliseconds;

                        if (executiontime0 == 0)
                        {
                            WatchLabel.Text = "Execution time: <1 ms";
                        }
                        else
                        {
                            WatchLabel.Text = "Execution time: " + executiontime0.ToString() + " ms";
                        }



                        // Shows the details for each shot

                        if (ERA < 0)
                        {
                            ERA = 0;
                        }

                        DetailsBox.Items.Add($"Shots taken: {Shots_Taken} - AV: {Math.Round(Current_AV, 2)} - HP: {Math.Round(Current_HP, 2)} - ERA Plates: {ERA} - ERA Bonus at shot {Shots_Taken}: {ERA_Bonus}");

                    }

                    if (finalrn > Hit_Chance)
                    {
                        Shots_Taken++;
                        DetailsBox.Items.Add($"Shot taken: {Shots_Taken} - MISS!");
                    }
                }

                if (Current_HP < 0)
                {
                    // make unit die in actual game

                    Current_HP = 0;
                    DetailsBox.Items.Add($"ITS DEAD! Shots taken: {Shots_Taken} - AV: {Math.Round(Current_AV, 2)} - HP: {Current_HP}");


                }

                ShotCount.Text = $"Shots needed to bring enemy to 0 HP: {Shots_Taken}";


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }



        }



        // RNG

        private void AccuracyRoll(int Hit_Chance, int shotstotake)
        {
            int shot_count = 0;
            var timer = new Stopwatch();
            while (shotstotake > shot_count)
            {

                // The random number provider.
                RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();

                // Return a random integer between a min and max value.
                int RandomN(int min, int max)
                {
                    uint scale = uint.MaxValue;
                    while (scale == uint.MaxValue)
                    {
                        // Get four random bytes.
                        byte[] four_bytes = new byte[4];
                        Rand.GetBytes(four_bytes);

                        // Convert that into an uint.
                        scale = BitConverter.ToUInt32(four_bytes, 0);
                    }

                    // Add min to the scaled difference between max and min.
                    return (int)(min + (max - min) * (scale / (double)uint.MaxValue));
                }

                int finalrn = RandomN(1, 100);

                Thread.Sleep(100);
                timer.Start();

                shot_count++;
                bool isHit = false;
                if (finalrn <= Hit_Chance)
                {
                    isHit = true;
                }

                timer.Stop();
                string runtime = timer.ElapsedMilliseconds.ToString();
                
            }

        }


        private void AlgorithmSimple() // ERA is still getting worked on
        {
            try
            {
                // Base Variables

                decimal AP = Convert.ToInt32(APWVTB.Text);
                decimal HE = Convert.ToInt32(HEWVTB.Text);
                decimal Base_AV = Convert.ToInt32(VAVTB.Text);
                decimal Base_HP = Convert.ToInt32(VHPTB.Text);

                int Shots_Taken = 0;

                // ERA variables

                int ERA = Convert.ToInt32(ERA_Count.Text);
                decimal ERA_Bonus = Convert.ToDecimal(VERATB.Text);
                decimal ERA_AP_Multiplier = Convert.ToDecimal(ERA_AP_MultiplierTB.Text);
                decimal ERA_HE_Multiplier = Convert.ToDecimal(ERA_HE_MultiplierTB.Text);

                if (ERA > 1)
                {
                    Console.WriteLine("ERAAAA");

                }

                // Process Variables

                decimal APAV_Multiplier = Convert.ToDecimal(APAVDMTB.Text);
                decimal APHP_Multiplier = Convert.ToDecimal(APHPDMTB.Text);

                decimal HEAV_Multiplier = Convert.ToDecimal(HEAVTB.Text);
                decimal HEHP_Multiplier = Convert.ToDecimal(HEHPDMTB.Text);

                decimal Current_AV = Base_AV;
                decimal Current_HP = Base_HP;

                decimal AP_AV_Damage = 0;
                decimal HE_AV_Damage = 0;

                while (Current_HP > 0)
                {
                    var watch = Stopwatch.StartNew();

                    if (ERA < 0)
                    {
                        ERA = 0;
                    }

                    // Shows the details for each shot

                    DetailsBox.Items.Add($"Shots taken: {Shots_Taken} - AV: {Current_AV} - HP: {Current_HP} - ERA Plates: {ERA} - ERA Bonus: {ERA_Bonus}");

                    // add the shot to data
                    Shots_Taken++;


                    // using try because of ERA mechanics
                    try
                    {
                        // AP integrity reduction
                        AP_AV_Damage = 0;

                        if (ERA >= 1)
                        {
                            ERA_Bonus = Convert.ToDecimal(VERATB.Text);
                            if (Current_AV > 0)
                            {
                                if (ERABlocksAPTick.Checked)
                                {
                                    decimal Current_ERA_Armor = ERA_Bonus;
                                    AP_AV_Damage = (AP / ERA_Bonus) * APAV_Multiplier;
                                    Current_ERA_Armor = ERA_Bonus - (AP_AV_Damage * ERA_AP_Multiplier);
                                    if (Current_ERA_Armor < 0)
                                    {
                                        decimal Post_ERA_AP = Current_ERA_Armor * (-1);

                                        AP_AV_Damage = (Post_ERA_AP / Current_AV) * APAV_Multiplier;
                                    }
                                    else if (Current_ERA_Armor > 0)
                                    {
                                        AP_AV_Damage = 0;
                                    }
                                }
                                else
                                {
                                    AP_AV_Damage = (AP / Current_AV) * APAV_Multiplier;
                                }
                            }
                            else
                            {
                                AP_AV_Damage = AP * APAV_Multiplier;
                            }

                        }
                        else
                        {

                            ERA_Bonus = 0;
                            if (Current_AV > 0)
                            {
                                AP_AV_Damage = (AP / Current_AV) * APAV_Multiplier;
                            }
                            else
                            {
                                AP_AV_Damage = AP * APAV_Multiplier;
                            }

                        }

                        // HE integrity reduction
                        HE_AV_Damage = 0;

                        if (ERA >= 1)
                        {
                            ERA_Bonus = Convert.ToDecimal(VERATB.Text);


                            if (Current_AV > 0)
                            {
                                if (ERABlocksHETick.Checked)
                                {
                                    decimal Current_ERA_Armor = ERA_Bonus;
                                    HE_AV_Damage = (HE / ERA_Bonus) * HEAV_Multiplier;
                                    Current_ERA_Armor = ERA_Bonus - (HE_AV_Damage * ERA_HE_Multiplier);
                                    if (Current_ERA_Armor < 0)
                                    {
                                        decimal Post_ERA_HE = Current_ERA_Armor * (-1);

                                        HE_AV_Damage = (Post_ERA_HE / Current_AV) * HEAV_Multiplier;
                                    }
                                    else if (Current_ERA_Armor > 0)
                                    {
                                        HE_AV_Damage = 0;
                                    }
                                }
                                else
                                {
                                    HE_AV_Damage = (HE / Current_AV) * HEAV_Multiplier;
                                }
                            }
                            else
                            {
                                HE_AV_Damage = HE * HEAV_Multiplier;
                            }



                        }
                        else if (ERA <= 0)
                        {

                            ERA_Bonus = 0;
                            if (Current_AV > 0)
                            {
                                HE_AV_Damage = (HE / Current_AV) * HEAV_Multiplier;
                            }
                            else
                            {
                                HE_AV_Damage = HE * HEAV_Multiplier;
                            }

                        }

                        ERA = ERA - 1;
                    }
                    catch
                    {

                    }



                    // Total Integrity reduction
                    decimal FULL_AV_Damage = AP_AV_Damage + HE_AV_Damage;
                    Current_AV = Current_AV - FULL_AV_Damage;
                    if (Current_AV < 0)
                    {
                        Current_AV = 0;
                    }

                    // AP -> HP Damage
                    decimal AP_HP_Damage = (AP - Current_AV) * APHP_Multiplier;
                    if (AP_HP_Damage < 0)
                    {
                        AP_HP_Damage = 0;
                    }

                    // HE -> HP Damage
                    decimal HE_HP_Damage = (HE - Current_AV) * HEHP_Multiplier;
                    if (HE_HP_Damage < 0)
                    {
                        HE_HP_Damage = 0;
                    }

                    // Total HP Damage
                    decimal FULL_HP_Damage = AP_HP_Damage + HE_HP_Damage;
                    if (FULL_HP_Damage < 0)
                    {
                        FULL_HP_Damage = 0;
                    }
                    else
                    {
                        Current_HP = Current_HP - FULL_HP_Damage;
                    }

                    watch.Stop();
                    var executiontime = watch.ElapsedMilliseconds;
                    WatchLabel.Text = executiontime.ToString();
                }

                if (Current_HP < 0)
                {
                    // make unit die in actual game

                    Current_HP = 0;
                    DetailsBox.Items.Add($"ITS DEAD! Shots taken: {Shots_Taken} - AV: {Current_AV} - HP: {Current_HP}");

                    
                }

                ShotCount.Text = $"Shots needed to bring enemy to 0 HP: {Shots_Taken}";


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }



        }

        private void AlgoWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Algorithm();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}

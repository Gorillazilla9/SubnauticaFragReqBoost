using System;
using System.Collections.Generic;
using System.IO;
using Harmony;

namespace FragmentCountChanger
{
    [HarmonyPatch(typeof(PDAScanner))]
    [HarmonyPatch("Initialize")]
    internal class PDAScannerPatcher
    {

        /*static Dictionary<TechType, int> fragmentTotalChanges = new Dictionary<TechType, int>
        {
            {TechType.AquariumFragment, 4}, {TechType.BaseFiltrationMachine, 2}, {TechType.BaseObservatoryFragment, 3}, {TechType.BaseRoomFragment, 2},
            {TechType.BaseUpgradeConsoleFragment, 4}, {TechType.BaseWaterParkFragment, 4}, {TechType.BenchFragment, 4}, {TechType.BuilderFragment, 3},
            {TechType.ConstructorFragment, 6}, {TechType.CyclopsBridgeFragment, 6}, {TechType.CyclopsDockingBayFragment, 6}, {TechType.CyclopsEngineFragment, 6},
            {TechType.CyclopsHullFragment, 6}, {TechType.ExosuitFragment, 6}, {TechType.MoonpoolFragment, 5}, {TechType.PlanterBoxFragment, 2},
            {TechType.PowerTransmitterFragment, 2}, {TechType.PropulsionCannonFragment, 4}, {TechType.RadioFragment, 2}, {TechType.SeaglideFragment, 6},
            {TechType.SeamothFragment, 6}, {TechType.StasisRifleFragment, 4}, {TechType.ThermalPlantFragment, 4}, {TechType.WorkbenchFragment, 6},
            {TechType.FiltrationMachine, 2}, {TechType.Bench, 2}, {TechType.BaseMapRoomFragment, 6}, {TechType.Warper, 2},
            {TechType.BaseBioReactorFragment, 6}, {TechType.BaseNuclearReactorFragment, 6}, {TechType.Welder, 4}, {TechType.LaserCutter, 4},
            {TechType.PowerCellChargerFragment, 4}, {TechType.BatteryChargerFragment, 4}, {TechType.ExosuitDrillArmFragment, 6}, {TechType.ExosuitGrapplingArmFragment, 6},
            {TechType.ExosuitTorpedoArmFragment, 4}, {TechType.ExosuitPropulsionArmFragment, 5}, {TechType.PrecursorTeleporter, 2}, {TechType.LaserCutterFragment, 6},
            {TechType.BeaconFragment, 6}, {TechType.GravSphereFragment, 4}
        };*/

        [HarmonyPostfix]
        public static void Postfix()
        {

            // tranverses the PDAScanner static class to get its private Dictionary of TechTypes and modify them
            Traverse pdaTraverse = Traverse.Create(typeof(PDAScanner));
            Traverse pdaMappingTraverse = pdaTraverse.Field("mapping");
            Dictionary<TechType, PDAScanner.EntryData> mapping = pdaMappingTraverse.GetValue<Dictionary<TechType, PDAScanner.EntryData>>();

            // reads in the JSON specifying the new totalFragments values
            List<string> fragmentStrs = new List<string>();
            try
            {
                string line;
                using (var reader = File.OpenText("QMods\\FragmentCountChanger\\fragments.txt"))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        fragmentStrs.Add(line);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("[FragmentCountChanger] " + ex.ToString());
                return;
            }

            Console.WriteLine("[FragmentCountChanger] loading");
            
            // iterate over every fragment pairing in the txt and update the value
            try
            {
                foreach (string fragPair in fragmentStrs)
                {
                    try
                    {
                        string[] fragPairSplit = fragPair.Split(':');

                        TechType fragType = (TechType) Enum.Parse(typeof(TechType), fragPairSplit[0].Trim());

                        PDAScanner.EntryData entryData;
                        if (mapping.TryGetValue(fragType, out entryData))
                        {
                            entryData.totalFragments = Int16.Parse(fragPairSplit[1].Trim());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[FragmentCountChanger] " + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[FragmentCountChanger] " + ex.ToString());
            }
            Console.WriteLine("[FragmentCountChanger] done");
        }
    }
}
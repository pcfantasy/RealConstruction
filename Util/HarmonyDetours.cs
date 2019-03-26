using Harmony;
using System.Reflection;
using System;
using UnityEngine;
using RealConstruction.CustomAI;

namespace RealConstruction.Util
{
    public static class HarmonyDetours
    {
        private static HarmonyInstance harmony = null;
        private static void ConditionalPatch(this HarmonyInstance harmony, MethodBase method, HarmonyMethod prefix, HarmonyMethod postfix)
        {
            var fullMethodName = string.Format("{0}.{1}", method.ReflectedType?.Name ?? "(null)", method.Name);
            if (harmony.GetPatchInfo(method)?.Owners?.Contains(harmony.Id) == true)
            {
                DebugLog.LogToFileOnly("Harmony patches already present for {0}" + fullMethodName.ToString());
            }
            else
            {
                DebugLog.LogToFileOnly("Patching {0}..." + fullMethodName.ToString());
                harmony.Patch(method, prefix, postfix);
            }
        }

        private static void ConditionalUnPatch(this HarmonyInstance harmony, MethodBase method, HarmonyMethod prefix = null, HarmonyMethod postfix = null)
        {
            var fullMethodName = string.Format("{0}.{1}", method.ReflectedType?.Name ?? "(null)", method.Name);
            if (prefix != null)
            {
                DebugLog.LogToFileOnly("UnPatching Prefix{0}..." + fullMethodName.ToString());
                harmony.Unpatch(method, HarmonyPatchType.Prefix);
            }
            if (postfix != null)
            {
                DebugLog.LogToFileOnly("UnPatching Postfix{0}..." + fullMethodName.ToString());
                harmony.Unpatch(method, HarmonyPatchType.Postfix);
            }
        }

        public static void Apply()
        {
            harmony = HarmonyInstance.Create("RealConstruction");
            //1
            var commonBuildingAIReleaseBuilding = typeof(CommonBuildingAI).GetMethod("ReleaseBuilding");
            var commonBuildingAIReleaseBuildingPostFix = typeof(CustomCommonBuildingAI).GetMethod("CommonBuildingAIReleaseBuildingPostfix");
            harmony.ConditionalPatch(commonBuildingAIReleaseBuilding,
                null,
                new HarmonyMethod(commonBuildingAIReleaseBuildingPostFix));
            //2
            var privateBuildingAISimulationStep = typeof(PrivateBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null);
            var privateBuildingAISimulationStepPostFix = typeof(CustomPrivateBuildingAI).GetMethod("PrivateBuildingAISimulationStepPostFix");
            harmony.ConditionalPatch(privateBuildingAISimulationStep,
                null,
                new HarmonyMethod(privateBuildingAISimulationStepPostFix));
            //3
            var playerBuildingAISimulationStep = typeof(PlayerBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null);
            var playerBuildingAISimulationStepPostFix = typeof(CustomPlayerBuildingAI).GetMethod("PlayerBuildingAISimulationStepPostFix");
            harmony.ConditionalPatch(playerBuildingAISimulationStep,
                null,
                new HarmonyMethod(playerBuildingAISimulationStepPostFix));
            //4
            var carAIPathfindFailure = typeof(CarAI).GetMethod("PathfindFailure", BindingFlags.NonPublic | BindingFlags.Instance);
            var carAIPathfindFailurePostFix = typeof(CustomCarAI).GetMethod("CarAIPathfindFailurePostFix");
            harmony.ConditionalPatch(carAIPathfindFailure,
                null,
                new HarmonyMethod(carAIPathfindFailurePostFix));
            //5
            var carAIPathfindSuccess = typeof(CarAI).GetMethod("PathfindSuccess", BindingFlags.NonPublic | BindingFlags.Instance);
            var carAIPathfindSuccessPostFix = typeof(CustomCarAI).GetMethod("CarAIPathfindSuccessPostFix");
            harmony.ConditionalPatch(carAIPathfindSuccess,
                null,
                new HarmonyMethod(carAIPathfindSuccessPostFix));
            Loader.HarmonyDetourInited = true;
            DebugLog.LogToFileOnly("Harmony patches applied");
        }

        public static void DeApply()
        {
            //1
            var commonBuildingAIReleaseBuilding = typeof(CommonBuildingAI).GetMethod("ReleaseBuilding");
            var commonBuildingAIReleaseBuildingPostFix = typeof(CustomCommonBuildingAI).GetMethod("CommonBuildingAIReleaseBuildingPostfix");
            harmony.ConditionalUnPatch(commonBuildingAIReleaseBuilding,
                null,
                new HarmonyMethod(commonBuildingAIReleaseBuildingPostFix));
            //2
            var privateBuildingAISimulationStep = typeof(PrivateBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null);
            var privateBuildingAISimulationStepPostFix = typeof(CustomPrivateBuildingAI).GetMethod("PrivateBuildingAISimulationStepPostFix");
            harmony.ConditionalUnPatch(privateBuildingAISimulationStep,
                null,
                new HarmonyMethod(privateBuildingAISimulationStepPostFix));
            //3
            var playerBuildingAISimulationStep = typeof(PlayerBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null);
            var playerBuildingAISimulationStepPostFix = typeof(CustomPlayerBuildingAI).GetMethod("PlayerBuildingAISimulationStepPostFix");
            harmony.ConditionalUnPatch(playerBuildingAISimulationStep,
                null,
                new HarmonyMethod(playerBuildingAISimulationStepPostFix));
            //4
            var carAIPathfindFailure = typeof(CarAI).GetMethod("PathfindFailure", BindingFlags.NonPublic | BindingFlags.Instance);
            var carAIPathfindFailurePostFix = typeof(CustomCarAI).GetMethod("CarAIPathfindFailurePostFix");
            harmony.ConditionalUnPatch(carAIPathfindFailure,
                null,
                new HarmonyMethod(carAIPathfindFailurePostFix));
            //5
            var carAIPathfindSuccess = typeof(CarAI).GetMethod("PathfindSuccess", BindingFlags.NonPublic | BindingFlags.Instance);
            var carAIPathfindSuccessPostFix = typeof(CustomCarAI).GetMethod("CarAIPathfindSuccessPostFix");
            harmony.ConditionalUnPatch(carAIPathfindSuccess,
                null,
                new HarmonyMethod(carAIPathfindSuccessPostFix));
            Loader.HarmonyDetourInited = false;
            DebugLog.LogToFileOnly("Harmony patches DeApplied");
        }
    }
}

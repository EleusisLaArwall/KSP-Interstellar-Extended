﻿using FNPlugin.Extensions;
using System;
using UnityEngine;

namespace FNPlugin
{
    public class ModuleEnginesWarp : ModuleEnginesFX
    {
        // GUI display values
        // Thrust
        [KSPField(isPersistant = true)]
        bool IsForceActivated;

        [KSPField(guiActive = true, guiName = "Warp Thrust")]
        protected string Thrust = "";
        // Isp
        [KSPField(guiActive = true, guiName = "Warp Isp")]
        protected string Isp = "";
        // Throttle
        [KSPField(guiActive = true, guiName = "Warp Throttle")]
        protected string Throttle = "";

        [KSPField(guiActive = false, guiName = "Demand")]
        public double propellantUsed;

        [KSPField]
        public double GThreshold = 2;

        // Resource used for deltaV and mass calculations
        //[KSPField]
        //public string resourceDeltaV = "LqdHydrogen";

        [KSPField]
        public string propellant1 = "LqdHydrogen";
        [KSPField]
        public string propellant2;
        [KSPField]
        public string propellant3;
        [KSPField]
        public string propellant4;

        [KSPField]
        public double ratio1 = 1;
        [KSPField]
        public double ratio2;
        [KSPField]
        public double ratio3;
        [KSPField]
        public double ratio4;

        //[KSPField(guiActive = false, guiName = "Calc Flow")]
        //public double calcualtedFlow;

        [KSPField(guiActive = false, guiName = "Mass Flow")]
        public double requestedFlow;

        [KSPField]
        public double demandMass;
        [KSPField]
        public double fuelRatio;
        [KSPField]
        private double averageDensityForPopellantWithMass;
        [KSPField]
        private double massPropellantRatio;
        [KSPField]
        private double ratioSumWithoutMass;

        // Numeric display values
        protected double thrust_d;
        protected double isp_d;
        protected double throttle_d;

        // Persistent values to use during timewarp
        float IspPersistent;
        float ThrustPersistent;
        float ThrottlePersistent;
        float previousThrottle;

        private double fuelWithMassPercentage1;
        private double fuelWithMassPercentage2;
        private double fuelWithMassPercentage3;
        private double fuelWithMassPercentage4;

        private double masslessFuelPercentage1;
        private double masslessFuelPercentage2;
        private double masslessFuelPercentage3;
        private double masslessFuelPercentage4;

        PartResourceDefinition propellantResourceDefinition1;
        PartResourceDefinition propellantResourceDefinition2;
        PartResourceDefinition propellantResourceDefinition3;
        PartResourceDefinition propellantResourceDefinition4;

        // Are we transitioning from timewarp to reatime?
        bool _warpToReal = false;

        // Update
        public override void OnUpdate()
        {
            // When transitioning from timewarp to real update throttle
            if (_warpToReal)
            {
                vessel.ctrlState.mainThrottle = ThrottlePersistent;
                _warpToReal = false;
            }

            // Persistent thrust GUI
            Fields["Thrust"].guiActive = isEnabled;
            Fields["Isp"].guiActive = isEnabled;
            Fields["Throttle"].guiActive = isEnabled;

            // Update display values
            Thrust = FormatThrust(thrust_d);
            Isp = Math.Round(isp_d, 2) + " s";
            Throttle = Math.Round(throttle_d * 100) + "%";

            if (IsForceActivated || !isEnabled || !isOperational) return;

            IsForceActivated = true;
            part.force_activate();
        }

        private void UpdateFuelFactors()
        {
            propellantResourceDefinition1 = !String.IsNullOrEmpty(propellant1) ? PartResourceLibrary.Instance.GetDefinition(propellant1) : null;
            propellantResourceDefinition2 = !String.IsNullOrEmpty(propellant2) ? PartResourceLibrary.Instance.GetDefinition(propellant2) : null;
            propellantResourceDefinition3 = !String.IsNullOrEmpty(propellant3) ? PartResourceLibrary.Instance.GetDefinition(propellant3) : null;
            propellantResourceDefinition4 = !String.IsNullOrEmpty(propellant4) ? PartResourceLibrary.Instance.GetDefinition(propellant4) : null;

            var ratioSumOveral = 0.0;
            var ratioSumWithMass = 0.0;
            var densitySum = 0.0;

            if (propellantResourceDefinition1 != null)
            {
                ratioSumOveral += ratio1;
                if (propellantResourceDefinition1.density > 0)
                {
                    ratioSumWithMass = ratio1;
                    densitySum += propellantResourceDefinition1.density * ratio1;
                }
            }
            if (propellantResourceDefinition2 != null)
            {
                ratioSumOveral += ratio2;
                if (propellantResourceDefinition2.density > 0)
                {
                    ratioSumWithMass = ratio2;
                    densitySum += propellantResourceDefinition2.density * ratio2;
                }
            }
            if (propellantResourceDefinition3 != null)
            {
                ratioSumOveral += ratio3;
                if (propellantResourceDefinition3.density > 0)
                {
                    ratioSumWithMass = ratio3;
                    densitySum += propellantResourceDefinition3.density * ratio3;
                }
            }
            if (propellantResourceDefinition4 != null)
            {
                ratioSumOveral += ratio4;
                if (propellantResourceDefinition4.density > 0)
                {
                    ratioSumWithMass = ratio4;
                    densitySum += propellantResourceDefinition4.density * ratio4;
                }
            }
            
            averageDensityForPopellantWithMass = densitySum / ratioSumWithMass;
            massPropellantRatio = ratioSumWithMass / ratioSumOveral;
            ratioSumWithoutMass = ratioSumOveral - ratioSumWithMass;

            fuelWithMassPercentage1 = propellantResourceDefinition1 != null && propellantResourceDefinition1.density > 0 ? ratio1 / ratioSumWithMass : 0;
            fuelWithMassPercentage2 = propellantResourceDefinition2 != null && propellantResourceDefinition2.density > 0 ? ratio2 / ratioSumWithMass : 0;
            fuelWithMassPercentage3 = propellantResourceDefinition3 != null && propellantResourceDefinition3.density > 0 ? ratio3 / ratioSumWithMass : 0;
            fuelWithMassPercentage4 = propellantResourceDefinition4 != null && propellantResourceDefinition4.density > 0 ? ratio4 / ratioSumWithMass : 0;

            masslessFuelPercentage1 = propellantResourceDefinition1 != null && propellantResourceDefinition1.density <= 0 ? ratio1 / ratioSumWithoutMass : 0;
            masslessFuelPercentage2 = propellantResourceDefinition2 != null && propellantResourceDefinition2.density <= 0 ? ratio2 / ratioSumWithoutMass : 0;
            masslessFuelPercentage3 = propellantResourceDefinition3 != null && propellantResourceDefinition3.density <= 0 ? ratio3 / ratioSumWithoutMass : 0;
            masslessFuelPercentage4 = propellantResourceDefinition4 != null && propellantResourceDefinition4.density <= 0 ? ratio4 / ratioSumWithoutMass : 0;
        }

        private double CollectFuel(double demandMass)
        {
            if (CheatOptions.InfinitePropellant)
                return 1;

            double fuelRequestAmount1 = 0;
            double fuelRequestAmount2 = 0;
            double fuelRequestAmount3 = 0;
            double fuelRequestAmount4 = 0;

            var propellantWithMassNeeded = demandMass / averageDensityForPopellantWithMass;
            var overalAmountNeeded = propellantWithMassNeeded / massPropellantRatio;
            var masslessResourceNeeded = overalAmountNeeded - propellantWithMassNeeded;

            // first determine lowest availalable resource ratio
            double availableRatio = 1;
            if (propellantResourceDefinition1 != null && ratio1 > 0)
            {
                fuelRequestAmount1 = fuelWithMassPercentage1 > 0 ? fuelWithMassPercentage1 * propellantWithMassNeeded : masslessFuelPercentage1 * masslessResourceNeeded;
                availableRatio = Math.Min(availableRatio, part.GetResourceAvailable(ResourceFlowMode.STACK_PRIORITY_SEARCH, propellantResourceDefinition1) / fuelRequestAmount1);
            }
            if (propellantResourceDefinition2 != null && ratio2 > 0)
            {
                fuelRequestAmount2 = fuelWithMassPercentage2 > 0 ? fuelWithMassPercentage2 * propellantWithMassNeeded : masslessFuelPercentage2 * masslessResourceNeeded;
                availableRatio = Math.Min(availableRatio, part.GetResourceAvailable(ResourceFlowMode.STACK_PRIORITY_SEARCH, propellantResourceDefinition2) / fuelRequestAmount2);
            }
            if (propellantResourceDefinition3 != null && ratio3 > 0)
            {
                fuelRequestAmount3 = fuelWithMassPercentage3 > 0 ? fuelWithMassPercentage3 * propellantWithMassNeeded : masslessFuelPercentage3 * masslessResourceNeeded;
                availableRatio = Math.Min(availableRatio, part.GetResourceAvailable(ResourceFlowMode.STACK_PRIORITY_SEARCH, propellantResourceDefinition3) / fuelRequestAmount3);
            }
            if (propellantResourceDefinition4 != null && ratio4 > 0)
            {
                fuelRequestAmount4 = fuelWithMassPercentage4 > 0 ? fuelWithMassPercentage4 * propellantWithMassNeeded : masslessFuelPercentage4 * masslessResourceNeeded;
                availableRatio = Math.Min(availableRatio, part.GetResourceAvailable(ResourceFlowMode.STACK_PRIORITY_SEARCH, propellantResourceDefinition4) / fuelRequestAmount4);
            }

            // ignore insignificant amount
            if (availableRatio < 1e-3)
                return 0;

            double recievedRatio = 1;
            if (fuelRequestAmount1 > 0)
            {
                propellantUsed = part.RequestResource(propellantResourceDefinition1.id, fuelRequestAmount1 * availableRatio, ResourceFlowMode.STACK_PRIORITY_SEARCH);
                recievedRatio = Math.Min(recievedRatio, fuelRequestAmount1 > 0 ? propellantUsed / fuelRequestAmount1 : 0);
            }
            if (fuelRequestAmount2 > 0)
            {
                propellantUsed = part.RequestResource(propellantResourceDefinition2.id, fuelRequestAmount2 * availableRatio, ResourceFlowMode.STACK_PRIORITY_SEARCH);
                recievedRatio = Math.Min(recievedRatio, fuelRequestAmount2 > 0 ? propellantUsed / fuelRequestAmount2 : 0);
            }
            if (fuelRequestAmount3 > 0)
            {
                propellantUsed = part.RequestResource(propellantResourceDefinition3.id, fuelRequestAmount3 * availableRatio, ResourceFlowMode.STACK_PRIORITY_SEARCH);
                recievedRatio = Math.Min(recievedRatio, fuelRequestAmount3 > 0 ? propellantUsed / fuelRequestAmount3 : 0);
            }
            if (fuelRequestAmount4 > 0)
            {
                propellantUsed = part.RequestResource(propellantResourceDefinition4.id, fuelRequestAmount4 * availableRatio, ResourceFlowMode.STACK_PRIORITY_SEARCH);
                recievedRatio = Math.Min(recievedRatio, fuelRequestAmount4 > 0 ? propellantUsed / fuelRequestAmount4 : 0);
            }

            return recievedRatio;
        }

        // Physics update
        public override void OnFixedUpdate()
        {
            if (FlightGlobals.fetch == null || !isEnabled) return;

            UpdateFuelFactors();

            // Realtime or Dynamic mode
            if (!vessel.packed)
            {
                // allow throtle to be used up to 2G
                TimeWarp.GThreshold = GThreshold;

                requestedFlow = (double)(decimal)this.requestedMassFlow;
                //calcualtedFlow = ThrustPersistent / (IspPersistent * 9.81); // Mass burn rate of engine
                demandMass = requestedFlow * (double)(decimal)TimeWarp.fixedDeltaTime;
                propellantUsed = demandMass / propellantResourceDefinition1.density; // Resource demand

                // if not transitioning from warp to real
                // Update values to use during timewarp
                if (!_warpToReal) //&& vessel.ctrlState.mainThrottle == previousThrottle)
                {
                    IspPersistent = realIsp;
                    ThrottlePersistent = vessel.ctrlState.mainThrottle;

                    this.CalculateThrust();
                    // verify we have thrust
                    if ((vessel.ctrlState.mainThrottle > 0 && finalThrust > 0) || (vessel.ctrlState.mainThrottle == 0 && finalThrust == 0))
                        ThrustPersistent = finalThrust;
                }
            }
            else
            {
                // Timewarp mode: perturb orbit using thrust
                _warpToReal = true; // Set to true for transition to realtime
                var universalTime = Planetarium.GetUniversalTime(); // Universal time

                requestedFlow = (double)(decimal)this.requestedMassFlow;
                //calcualtedFlow = ThrustPersistent / (IspPersistent * 9.81); // Mass burn rate of engine
                demandMass = requestedFlow * (double)(decimal)TimeWarp.fixedDeltaTime; // Change in mass over dT

                fuelRatio =	CollectFuel(demandMass);

                // Calculate thrust and deltaV if demand output > 0
                if (fuelRatio > 0)
                {
                    var vesselMass = this.vessel.GetTotalMass(); // Current mass
                    var remainingMass = vesselMass - demandMass; // Mass at end of burn
                    var deltaV = IspPersistent * PluginHelper.GravityConstant * Math.Log(vesselMass / remainingMass); // Delta V from burn

                    Vector3d thrustV = this.part.transform.up; // Thrust direction
                    var deltaVV = deltaV * thrustV; // DeltaV vector
                    vessel.orbit.Perturb(deltaVV, universalTime); // Update vessel orbit
                }
                // Otherwise, if throttle is turned on, and demand out is 0, show warning
                    
                else if (ThrottlePersistent > 0)
                {
                    Debug.Log("Propellant depleted");
                }
            }            

            // Update display numbers
            thrust_d = ThrustPersistent;
            isp_d = IspPersistent;
            throttle_d = ThrottlePersistent;
            previousThrottle = vessel.ctrlState.mainThrottle;
        }

        // Format thrust into mN, N, kN
        public static string FormatThrust(double thrust)
        {
            if (thrust < 0.001)
                return Math.Round(thrust * 1e+6, 3) + " mN";
            else if (thrust < 1)
                return Math.Round(thrust * 1e+3, 3) + " N";
            else
                return Math.Round(thrust, 3) + " kN";
        }
    }

}
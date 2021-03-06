﻿using FNPlugin.Extensions;
using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace FNPlugin
{
    [KSPModule("Confinement Fusion Engine")]
    class FusionEngineController : DaedalusEngineController { }

    [KSPModule("Confinement Fusion Engine")]
    class DaedalusEngineController : ResourceSuppliableModule, IUpgradeableModule 
    {
        // Persistant
        [KSPField(isPersistant = true)]
        public bool IsEnabled;
        [KSPField(isPersistant = true)]
        public bool rad_safety_features = true;

        [KSPField]
        public double massThrustExp = 0;
        [KSPField]
        public double massIspExp = 0;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "#LOC_KSPIE_FusionEngine_speedLimit", guiUnits = "c"), UI_FloatRange(stepIncrement = 0.005f, maxValue = 1, minValue = 0.005f)]
        public float speedLimit = 1;
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "#LOC_KSPIE_FusionEngine_fuelLimit", guiUnits = "%"), UI_FloatRange(stepIncrement = 0.5f, maxValue = 100, minValue = 0.5f)]
        public float fuelLimit = 100;
        [KSPField(isPersistant = true, guiActiveEditor = false, guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_maximizeThrust"), UI_Toggle(disabledText = "Off", enabledText = "On")]
        public bool maximizeThrust = true;

        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_powerUsage")]
        public string powerUsage;

        [KSPField(guiActive = false, guiActiveEditor = false, guiName = "#LOC_KSPIE_FusionEngine_fusionFuel")]
        public string fusionFuel1 = "FusionPellets";
        [KSPField(guiActive = false, guiActiveEditor = false)]
        public string fusionFuel2;
        [KSPField(guiActive = false, guiActiveEditor = false)]
        public string fusionFuel3;

        [KSPField(guiActive = false, guiActiveEditor = false)]
        public double fusionFuelRatio1 = 1;
        [KSPField(guiActive = false, guiActiveEditor = false)]
        public double fusionFuelRatio2 = 0;
        [KSPField(guiActive = false, guiActiveEditor = false)]
        public double fusionFuelRatio3 = 0;

        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_temperatureStr")]
        public string temperatureStr = "";
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_speedOfLight", guiUnits = " m/s")]
        public double speedOfLight;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_lightSpeedRatio", guiFormat = "F9", guiUnits = "c")]
        public double lightSpeedRatio = 0;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_relativity", guiFormat = "F10")]
        public double relativity;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_timeDilation", guiFormat = "F10")]
        public double timeDilation;

        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_radhazardstr")]
        public string radhazardstr = "";
        [KSPField(guiActiveEditor = true, guiName = "#LOC_KSPIE_FusionEngine_partMass", guiUnits = " t")]
        public float partMass = 1;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_fusionRatio", guiFormat = "F6")]
        public double fusionRatio = 0;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_fuelAmountsRatio")]
        public string fuelAmountsRatio;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_thrustPowerInTeraWatt", guiFormat = "F2", guiUnits = " TW")]
        public double thrustPowerInTeraWatt = 0;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_calculatedFuelflow", guiFormat = "F6", guiUnits = " U")]
        public double calculatedFuelflow = 0;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_massFlowRateKgPerSecond", guiFormat = "F6", guiUnits = " kg/s")]
        public double massFlowRateKgPerSecond;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_massFlowRateTonPerHour", guiFormat = "F6", guiUnits = " t/h")]
        public double massFlowRateTonPerHour;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_storedThrotle")]
        public float storedThrotle = 0;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_effectiveMaxThrustInKiloNewton", guiFormat = "F2", guiUnits = " kN")]
        public double effectiveMaxThrustInKiloNewton = 0;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_effectiveIsp", guiFormat = "F2", guiUnits = "s")]
        public double effectiveIsp = 0;
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_FusionEngine_worldSpaceVelocity", guiFormat = "F3", guiUnits = " m/s")]
        public double worldSpaceVelocity;

        [KSPField]
        public double universalTime;

        [KSPField(guiActiveEditor = true, guiName = "<size=10>Upgrade Tech</size>")]
        public string translatedTechMk1;
        [KSPField(guiActiveEditor = true, guiName = "<size=10>Upgrade Tech</size>")]
        public string translatedTechMk2;
        [KSPField(guiActiveEditor = true, guiName = "<size=10>Upgrade Tech</size>")]
        public string translatedTechMk3;
        [KSPField(guiActiveEditor = true, guiName = "<size=10>Upgrade Tech</size>")]
        public string translatedTechMk4;
        [KSPField(guiActiveEditor = true, guiName = "<size=10>Upgrade Tech</size>")]
        public string translatedTechMk5;
        [KSPField(guiActiveEditor = true, guiName = "<size=10>Upgrade Tech</size>")]
        public string translatedTechMk6;
        [KSPField(guiActiveEditor = true, guiName = "<size=10>Upgrade Tech</size>")]
        public string translatedTechMk7;
        [KSPField(guiActiveEditor = true, guiName = "<size=10>Upgrade Tech</size>")]
        public string translatedTechMk8;

        [KSPField]
        public float maxThrustMk1 = 300;
        [KSPField]
        public float maxThrustMk2 = 500;
        [KSPField]
        public float maxThrustMk3 = 800;
        [KSPField]
        public float maxThrustMk4 = 1200;
        [KSPField]
        public float maxThrustMk5 = 1500;
        [KSPField]
        public float maxThrustMk6 = 2000;
        [KSPField]
        public float maxThrustMk7 = 2500;
        [KSPField]
        public float maxThrustMk8 = 3000;
        [KSPField]
        public float maxThrustMk9 = 3500;

        [KSPField(guiActiveEditor = true, guiName = "Thust/Isp Mk1")]
        public string guiThrustMk1;
        [KSPField(guiActiveEditor = true, guiName = "Thust/Isp Mk2")]
        public string guiThrustMk2;
        [KSPField(guiActiveEditor = true, guiName = "Thust/Isp Mk3")]
        public string guiThrustMk3;
        [KSPField(guiActiveEditor = true, guiName = "Thust/Isp Mk4")]
        public string guiThrustMk4;
        [KSPField(guiActiveEditor = true, guiName = "Thust/Isp Mk5")]
        public string guiThrustMk5;
        [KSPField(guiActiveEditor = true, guiName = "Thust/Isp Mk6")]
        public string guiThrustMk6;
        [KSPField(guiActiveEditor = true, guiName = "Thust/Isp Mk7")]
        public string guiThrustMk7;
        [KSPField(guiActiveEditor = true, guiName = "Thust/Isp Mk8")]
        public string guiThrustMk8;
        [KSPField(guiActiveEditor = true, guiName = "Thust/Isp Mk9")]
        public string guiThrustMk9;

        [KSPField(guiActiveEditor = true, guiName = "Power/Waste Mk1")]
        public string guiPowerMk1;
        [KSPField(guiActiveEditor = true, guiName = "Power/Waste Mk2")]
        public string guiPowerMk2;
        [KSPField(guiActiveEditor = true, guiName = "Power/Waste Mk3")]
        public string guiPowerMk3;
        [KSPField(guiActiveEditor = true, guiName = "Power/Waste Mk4")]
        public string guiPowerMk4;
        [KSPField(guiActiveEditor = true, guiName = "Power/Waste Mk5")]
        public string guiPowerMk5;
        [KSPField(guiActiveEditor = true, guiName = "Power/Waste Mk6")]
        public string guiPowerMk6;
        [KSPField(guiActiveEditor = true, guiName = "Power/Waste Mk7")]
        public string guiPowerMk7;
        [KSPField(guiActiveEditor = true, guiName = "Power/Waste Mk8")]
        public string guiPowerMk8;
        [KSPField(guiActiveEditor = true, guiName = "Power/Waste Mk9")]
        public string guiPowerMk9;

        [KSPField]
        public float wasteheatMk1 = 2500;
        [KSPField]
        public float wasteheatMk2 = 2500;
        [KSPField]
        public float wasteheatMk3 = 2500;
        [KSPField]
        public float wasteheatMk4 = 2500;
        [KSPField]
        public float wasteheatMk5 = 2500;
        [KSPField]
        public float wasteheatMk6 = 2500;
        [KSPField]
        public float wasteheatMk7 = 2500;
        [KSPField]
        public float wasteheatMk8 = 2500;
        [KSPField]
        public float wasteheatMk9 = 2500;

        [KSPField]
        public double powerRequirementMk1 = 4304;
        [KSPField]
        public double powerRequirementMk2 = 4783;
        [KSPField]
        public double powerRequirementMk3 = 5314;
        [KSPField]
        public double powerRequirementMk4 = 5905;
        [KSPField]
        public double powerRequirementMk5 = 6561;
        [KSPField]
        public double powerRequirementMk6 = 7290;
        [KSPField]
        public double powerRequirementMk7 = 8100;
        [KSPField]
        public double powerRequirementMk8 = 9000;
        [KSPField]
        public double powerRequirementMk9 = 10000;

        [KSPField]
        public double thrustIspMk1 = 83886;
        [KSPField]
        public double thrustIspMk2 = 104857;
        [KSPField]
        public double thrustIspMk3 = 131072;
        [KSPField]
        public double thrustIspMk4 = 163840;
        [KSPField]
        public double thrustIspMk5 = 204800;
        [KSPField]
        public double thrustIspMk6 = 256000;
        [KSPField]
        public double thrustIspMk7 = 320000;
        [KSPField]
        public double thrustIspMk8 = 400000;
        [KSPField]
        public double thrustIspMk9 = 500000;

        [KSPField(guiActive = true, guiName = "Available Upgrade Techs")]
        public int numberOfAvailableUpgradeTechs;
        [KSPField]
        public float maxAtmosphereDensity = 0;
        [KSPField]
        public float leathalDistance = 2000;
        [KSPField]
        public float killDivider = 50;
        [KSPField]
        public float wasteHeatMultiplier = 1;
        [KSPField]
        public float powerRequirementMultiplier = 1;
        [KSPField]
        public float maxTemp = 3200;
        [KSPField]
        public double powerThrottleExponent = 0.5;
        [KSPField]
        public double ispThrottleExponent = 0.5;
        [KSPField]
        public int powerPriority = 4;
        [KSPField]
        public float upgradeCost = 100;
        [KSPField]
        public string originalName = "Prototype Deadalus IC Fusion Engine";
        [KSPField]
        public string upgradedName = "Deadalus IC Fusion Engine";

        [KSPField]
        public string upgradeTechReq1 = null;
        [KSPField]
        public string upgradeTechReq2 = null;
        [KSPField]
        public string upgradeTechReq3 = null;
        [KSPField]
        public string upgradeTechReq4 = null;
        [KSPField]
        public string upgradeTechReq5 = null;
        [KSPField]
        public string upgradeTechReq6 = null;
        [KSPField]
        public string upgradeTechReq7 = null;
        [KSPField]
        public string upgradeTechReq8 = null;

        [KSPField]
        public double demandMass;
        [KSPField]
        public double fuelRatio;
        [KSPField]
        double averageDensity;

        bool radhazard;
        bool warpToReal;
        double engineIsp;
        double percentageFuelRemaining;

        double fusionFuelFactor1;
	    double fusionFuelFactor2;
	    double fusionFuelFactor3;

        Stopwatch stopWatch;
        ModuleEngines curEngineT;
        BaseEvent deactivateRadSafetyEvent;
        BaseEvent activateRadSafetyEvent;
        BaseEvent retrofitEngineEvent;
        BaseField radhazardstrField;

        PartResourceDefinition fusionFuelResourceDefinition1;
        PartResourceDefinition fusionFuelResourceDefinition2;
        PartResourceDefinition fusionFuelResourceDefinition3;

        const string LIGHTBLUE = "#7fdfffff";

        private int totalNumberOfGenerations; 

        private int _engineGenerationType;
        public GenerationType EngineGenerationType
        {
            get { return (GenerationType) _engineGenerationType; }
            private set { _engineGenerationType = (int) value; }
        }

        [KSPEvent(guiActive = true, guiName = "Disable Radiation Safety", active = true)]
        public void DeactivateRadSafety() 
        {
            rad_safety_features = false;
        }

        [KSPEvent(guiActive = true, guiName = "Activate Radiation Safety", active = false)]
        public void ActivateRadSafety() 
        {
            rad_safety_features = true;
        }

        #region IUpgradeableModule

        public String UpgradeTechnology { get { return upgradeTechReq1; } }

        private float RawMaximumThrust
        {
            get
            {
                switch (_engineGenerationType)
                {
                    case (int)GenerationType.Mk1:
                        return maxThrustMk1;
                    case (int)GenerationType.Mk2:
                        return maxThrustMk2;
                    case (int)GenerationType.Mk3:
                        return maxThrustMk3;
                    case (int)GenerationType.Mk4:
                        return maxThrustMk4;
                    case (int)GenerationType.Mk5:
                        return maxThrustMk5;
                    case (int)GenerationType.Mk6:
                        return maxThrustMk6;
                    case (int)GenerationType.Mk7:
                        return maxThrustMk7;
                    case (int)GenerationType.Mk8:
                        return maxThrustMk8;
                    default:
                        return maxThrustMk9;
                }
            }
        }

        private double MaximumThrust
        {
            get
            {
                return RawMaximumThrust * Math.Pow(part.mass / partMass, massThrustExp);
            }
        }

        private float FusionWasteHeat
        {
            get
            {
                switch (_engineGenerationType)
                {
                    case (int)GenerationType.Mk1:
                        return wasteheatMk1;
                    case (int)GenerationType.Mk2:
                        return wasteheatMk2;
                    case (int)GenerationType.Mk3:
                        return wasteheatMk3;
                    case (int)GenerationType.Mk4:
                        return wasteheatMk4;
                    case (int)GenerationType.Mk5:
                        return wasteheatMk5;
                    case (int)GenerationType.Mk6:
                        return wasteheatMk6;
                    case (int)GenerationType.Mk7:
                        return wasteheatMk7;
                    case (int)GenerationType.Mk8:
                        return wasteheatMk8;
                    default:
                        return maxThrustMk9;
                }
            }
        }

        public double PowerRequirement
        {
            get
            {
                switch (_engineGenerationType)
                {
                    case (int)GenerationType.Mk1:
                        return powerRequirementMk1;
                    case (int)GenerationType.Mk2:
                        return powerRequirementMk2;
                    case (int)GenerationType.Mk3:
                        return powerRequirementMk3;
                    case (int)GenerationType.Mk4:
                        return powerRequirementMk4;
                    case (int)GenerationType.Mk5:
                        return powerRequirementMk5;
                    case (int)GenerationType.Mk6:
                        return powerRequirementMk6;
                    case (int)GenerationType.Mk7:
                        return powerRequirementMk7;
                    case (int)GenerationType.Mk8:
                        return powerRequirementMk8;
                    default:
                        return powerRequirementMk9;
                }
            }
        }

        public double RawEngineIsp
        {
            get
            {
                switch (_engineGenerationType)
                {
                    case (int)GenerationType.Mk1:
                        return thrustIspMk1;
                    case (int)GenerationType.Mk2:
                        return thrustIspMk2;
                    case (int)GenerationType.Mk3:
                        return thrustIspMk3;
                    case (int)GenerationType.Mk4:
                        return thrustIspMk4;
                    case (int)GenerationType.Mk5:
                        return thrustIspMk5;
                    case (int)GenerationType.Mk6:
                        return thrustIspMk6;
                    case (int)GenerationType.Mk7:
                        return thrustIspMk7;
                    case (int)GenerationType.Mk8:
                        return thrustIspMk8;
                    default:
                        return thrustIspMk9;
                }
            }
        }

        public double EngineIsp
        {
            get
            {
                return RawEngineIsp * Math.Pow(part.mass / partMass, massIspExp);
            }
        }

        private double EffectivePowerRequirement
        {
            get
            {
                return PowerRequirement * powerRequirementMultiplier;
            }
        }

        public void upgradePartModule()
        {
            //isupgraded = true;
        }

        #endregion

        public override void OnStart(StartState state) 
        {
            try
            {
                stopWatch = new Stopwatch();
                speedOfLight = GameConstants.speedOfLight * PluginHelper.SpeedOfLightMult;

                UpdateFuelFactors();

                part.maxTemp = maxTemp;
                part.thermalMass = 1;
                part.thermalMassModifier = 1;

                curEngineT = this.part.FindModuleImplementing<ModuleEngines>();

                if (curEngineT == null) return;

                //engineIsp = curEngineT.atmosphereCurve.Evaluate(0);

                //// if we can upgrade, let's do so
                //if (isupgraded)
                //    upgradePartModule();
                //else if (this.HasTechsRequiredToUpgrade())
                //    hasrequiredupgrade = true;

                //if (state == StartState.Editor && this.HasTechsRequiredToUpgrade())
                //{
                //    isupgraded = true;
                //    upgradePartModule();
                //}

                DetermineTechLevel();

                engineIsp = EngineIsp;

                // bind with fields and events
                deactivateRadSafetyEvent = Events["DeactivateRadSafety"];
                activateRadSafetyEvent = Events["ActivateRadSafety"];
                retrofitEngineEvent = Events["RetrofitEngine"];
                radhazardstrField = Fields["radhazardstr"];

                translatedTechMk1 = DisplayTech(upgradeTechReq1);
                translatedTechMk2 = DisplayTech(upgradeTechReq2);
                translatedTechMk3 = DisplayTech(upgradeTechReq3);
                translatedTechMk4 = DisplayTech(upgradeTechReq4);
                translatedTechMk5 = DisplayTech(upgradeTechReq5);
                translatedTechMk6 = DisplayTech(upgradeTechReq6);
                translatedTechMk7 = DisplayTech(upgradeTechReq7);
                translatedTechMk8 = DisplayTech(upgradeTechReq8);

                Fields["translatedTechMk1"].guiActiveEditor = !String.IsNullOrEmpty(translatedTechMk1);
                Fields["translatedTechMk2"].guiActiveEditor = !String.IsNullOrEmpty(translatedTechMk2);
                Fields["translatedTechMk3"].guiActiveEditor = !String.IsNullOrEmpty(translatedTechMk3);
                Fields["translatedTechMk4"].guiActiveEditor = !String.IsNullOrEmpty(translatedTechMk4);
                Fields["translatedTechMk5"].guiActiveEditor = !String.IsNullOrEmpty(translatedTechMk5);
                Fields["translatedTechMk6"].guiActiveEditor = !String.IsNullOrEmpty(translatedTechMk6);
                Fields["translatedTechMk7"].guiActiveEditor = !String.IsNullOrEmpty(translatedTechMk7);
                Fields["translatedTechMk8"].guiActiveEditor = !String.IsNullOrEmpty(translatedTechMk8);

                Fields["guiThrustMk1"].guiActiveEditor = totalNumberOfGenerations > 0;
                Fields["guiThrustMk2"].guiActiveEditor = totalNumberOfGenerations > 1;
                Fields["guiThrustMk3"].guiActiveEditor = totalNumberOfGenerations > 2;
                Fields["guiThrustMk4"].guiActiveEditor = totalNumberOfGenerations > 3;
                Fields["guiThrustMk5"].guiActiveEditor = totalNumberOfGenerations > 4;
                Fields["guiThrustMk6"].guiActiveEditor = totalNumberOfGenerations > 5;
                Fields["guiThrustMk7"].guiActiveEditor = totalNumberOfGenerations > 6;
                Fields["guiThrustMk8"].guiActiveEditor = totalNumberOfGenerations > 7;
                Fields["guiThrustMk9"].guiActiveEditor = totalNumberOfGenerations > 8;

                Fields["guiPowerMk1"].guiActiveEditor = totalNumberOfGenerations > 0;
                Fields["guiPowerMk2"].guiActiveEditor = totalNumberOfGenerations > 1;
                Fields["guiPowerMk3"].guiActiveEditor = totalNumberOfGenerations > 2;
                Fields["guiPowerMk4"].guiActiveEditor = totalNumberOfGenerations > 3;
                Fields["guiPowerMk5"].guiActiveEditor = totalNumberOfGenerations > 4;
                Fields["guiPowerMk6"].guiActiveEditor = totalNumberOfGenerations > 5;
                Fields["guiPowerMk7"].guiActiveEditor = totalNumberOfGenerations > 6;
                Fields["guiPowerMk8"].guiActiveEditor = totalNumberOfGenerations > 7;
                Fields["guiPowerMk9"].guiActiveEditor = totalNumberOfGenerations > 8;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error OnStart " + e.Message + " stack " + e.StackTrace);
            }
        }

        private void UpdateFuelFactors()
        {
            if (!String.IsNullOrEmpty(fusionFuel1))
                fusionFuelResourceDefinition1 = PartResourceLibrary.Instance.GetDefinition(fusionFuel1);
            if (!String.IsNullOrEmpty(fusionFuel2))
                fusionFuelResourceDefinition2 = PartResourceLibrary.Instance.GetDefinition(fusionFuel2);
            if (!String.IsNullOrEmpty(fusionFuel3))
                fusionFuelResourceDefinition3 = PartResourceLibrary.Instance.GetDefinition(fusionFuel3);

            var ratioSum = 0.0;
            var densitySum = 0.0;

            if (fusionFuelResourceDefinition1 != null)
            {
                ratioSum += fusionFuelRatio1;
                densitySum += fusionFuelResourceDefinition1.density * fusionFuelRatio1; 
            }
            if (fusionFuelResourceDefinition2 != null)
            {
                ratioSum += fusionFuelRatio2;
                densitySum += fusionFuelResourceDefinition2.density * fusionFuelRatio2; 
            }
            if (fusionFuelResourceDefinition3 != null)
            {
                ratioSum += fusionFuelRatio3;
                densitySum += fusionFuelResourceDefinition3.density * fusionFuelRatio3; 
            }

            averageDensity = densitySum / ratioSum;

            fusionFuelFactor1 = fusionFuelResourceDefinition1 != null ? fusionFuelRatio1/ratioSum : 0;
            fusionFuelFactor2 = fusionFuelResourceDefinition2 != null ? fusionFuelRatio2/ratioSum : 0;
            fusionFuelFactor3 = fusionFuelResourceDefinition3 != null ? fusionFuelRatio3/ratioSum : 0;
        }

        private string DisplayTech(string techid)
        {
            if (String.IsNullOrEmpty(techid))
                return string.Empty;

            var translatedTech = Localizer.Format(PluginHelper.GetTechTitleById(techid));

            if (PluginHelper.UpgradeAvailable(techid))
                return "<size=10><color=green>Ѵ</color> " + translatedTech + "</size>";
            else
                return "<size=10><color=red>X</color> " + translatedTech + "</size>";
        }

        private void DetermineTechLevel()
        {
            totalNumberOfGenerations = 1;
            if (!string.IsNullOrEmpty(upgradeTechReq1))
                totalNumberOfGenerations++;
            if (!string.IsNullOrEmpty(upgradeTechReq2))
                totalNumberOfGenerations++;
            if (!string.IsNullOrEmpty(upgradeTechReq3))
                totalNumberOfGenerations++;
            if (!string.IsNullOrEmpty(upgradeTechReq4))
                totalNumberOfGenerations++;
            if (!string.IsNullOrEmpty(upgradeTechReq5))
                totalNumberOfGenerations++;
            if (!string.IsNullOrEmpty(upgradeTechReq6))
                totalNumberOfGenerations++;
            if (!string.IsNullOrEmpty(upgradeTechReq7))
                totalNumberOfGenerations++;
            if (!string.IsNullOrEmpty(upgradeTechReq8))
                totalNumberOfGenerations++;

            numberOfAvailableUpgradeTechs = 0;
            if (PluginHelper.UpgradeAvailable(upgradeTechReq1))
                numberOfAvailableUpgradeTechs++;
            if (PluginHelper.UpgradeAvailable(upgradeTechReq2))
                numberOfAvailableUpgradeTechs++;
            if (PluginHelper.UpgradeAvailable(upgradeTechReq3))
                numberOfAvailableUpgradeTechs++;
            if (PluginHelper.UpgradeAvailable(upgradeTechReq4))
                numberOfAvailableUpgradeTechs++;
            if (PluginHelper.UpgradeAvailable(upgradeTechReq5))
                numberOfAvailableUpgradeTechs++;
            if (PluginHelper.UpgradeAvailable(upgradeTechReq6))
                numberOfAvailableUpgradeTechs++;
            if (PluginHelper.UpgradeAvailable(upgradeTechReq7))
                numberOfAvailableUpgradeTechs++;
            if (PluginHelper.UpgradeAvailable(upgradeTechReq8))
                numberOfAvailableUpgradeTechs++;

            EngineGenerationType = (GenerationType) numberOfAvailableUpgradeTechs;
        }

        public void Update()
        {
            try
            {
                if (HighLogic.LoadedSceneIsEditor)
                {
                    UpdateThrustGui();

                    // configure engine for Kerbal Engeneer support
                    UpdateAtmosphericCurve(EngineIsp);
                    effectiveMaxThrustInKiloNewton = MaximumThrust;
                    calculatedFuelflow = effectiveMaxThrustInKiloNewton / EngineIsp / PluginHelper.GravityConstant;
                    curEngineT.maxFuelFlow = (float)calculatedFuelflow;
                    curEngineT.maxThrust = (float)effectiveMaxThrustInKiloNewton;

                    return;
                }

                double fusionFuelCurrentAmount1;
                double fusionFuelMaxAmount1;
                part.GetConnectedResourceTotals(fusionFuelResourceDefinition1.id, out fusionFuelCurrentAmount1, out fusionFuelMaxAmount1);

                percentageFuelRemaining = fusionFuelCurrentAmount1 / fusionFuelMaxAmount1 * 100;
                fuelAmountsRatio = percentageFuelRemaining.ToString("0.0000") + "% " + fusionFuelMaxAmount1.ToString("0") + " L";
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error Update " + e.Message + " stack " + e.StackTrace);
            }
        }

        private void UpdateThrustGui()
        {
            guiThrustMk1 = FormatThrustStatistics(maxThrustMk1, thrustIspMk1, EngineGenerationType == GenerationType.Mk1 ? LIGHTBLUE : null);
            guiThrustMk2 = FormatThrustStatistics(maxThrustMk2, thrustIspMk2, EngineGenerationType == GenerationType.Mk2 ? LIGHTBLUE : null);
            guiThrustMk3 = FormatThrustStatistics(maxThrustMk3, thrustIspMk3, EngineGenerationType == GenerationType.Mk3 ? LIGHTBLUE : null);
            guiThrustMk4 = FormatThrustStatistics(maxThrustMk4, thrustIspMk4, EngineGenerationType == GenerationType.Mk4 ? LIGHTBLUE : null);
            guiThrustMk5 = FormatThrustStatistics(maxThrustMk5, thrustIspMk5, EngineGenerationType == GenerationType.Mk5 ? LIGHTBLUE : null);
            guiThrustMk6 = FormatThrustStatistics(maxThrustMk6, thrustIspMk6, EngineGenerationType == GenerationType.Mk6 ? LIGHTBLUE : null);
            guiThrustMk7 = FormatThrustStatistics(maxThrustMk7, thrustIspMk7, EngineGenerationType == GenerationType.Mk7 ? LIGHTBLUE : null);
            guiThrustMk8 = FormatThrustStatistics(maxThrustMk8, thrustIspMk8, EngineGenerationType == GenerationType.Mk8 ? LIGHTBLUE : null);
            guiThrustMk9 = FormatThrustStatistics(maxThrustMk9, thrustIspMk9, EngineGenerationType == GenerationType.Mk9 ? LIGHTBLUE : null);

            guiPowerMk1 = FormatPowerStatistics(powerRequirementMk1, wasteheatMk1, EngineGenerationType == GenerationType.Mk1 ? LIGHTBLUE : null);
            guiPowerMk2 = FormatPowerStatistics(powerRequirementMk2, wasteheatMk2, EngineGenerationType == GenerationType.Mk2 ? LIGHTBLUE : null);
            guiPowerMk3 = FormatPowerStatistics(powerRequirementMk3, wasteheatMk3, EngineGenerationType == GenerationType.Mk3 ? LIGHTBLUE : null);
            guiPowerMk4 = FormatPowerStatistics(powerRequirementMk4, wasteheatMk4, EngineGenerationType == GenerationType.Mk4 ? LIGHTBLUE : null);
            guiPowerMk5 = FormatPowerStatistics(powerRequirementMk5, wasteheatMk5, EngineGenerationType == GenerationType.Mk5 ? LIGHTBLUE : null);
            guiPowerMk6 = FormatPowerStatistics(powerRequirementMk6, wasteheatMk6, EngineGenerationType == GenerationType.Mk6 ? LIGHTBLUE : null);
            guiPowerMk7 = FormatPowerStatistics(powerRequirementMk7, wasteheatMk7, EngineGenerationType == GenerationType.Mk7 ? LIGHTBLUE : null);
            guiPowerMk8 = FormatPowerStatistics(powerRequirementMk8, wasteheatMk8, EngineGenerationType == GenerationType.Mk8 ? LIGHTBLUE : null);
            guiPowerMk9 = FormatPowerStatistics(powerRequirementMk9, wasteheatMk9, EngineGenerationType == GenerationType.Mk9 ? LIGHTBLUE : null);
        }

        private string FormatThrustStatistics(double value, double isp, string color = null, string format = "F0")
        {
            var result = value.ToString(format) + " kN @ " + isp.ToString(format) + "s";

            if (String.IsNullOrEmpty(color))
                return result;

            return "<color=" + color + ">" + result + "</color>";
        }

        private string FormatPowerStatistics(double powerRequirement, double wasteheat, string color = null, string format = "F0")
        {
            var result = (powerRequirement * powerRequirementMultiplier).ToString(format) + " MWe / " + wasteheat.ToString(format) + " MJ";

            if (String.IsNullOrEmpty(color))
                return result;

            return "<color=" + color + ">" + result + "</color>";
        }

        //
        public override void OnUpdate()
        {

            if (curEngineT == null) return;

            try
            {
                // When transitioning from timewarp to real update throttle
                if (warpToReal)
                {
                    vessel.ctrlState.mainThrottle = storedThrotle;
                    warpToReal = false;
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error OnUpdate warpToReal: " + e.Message);
            }

            try
            {
                deactivateRadSafetyEvent.active = rad_safety_features;
                activateRadSafetyEvent.active = !rad_safety_features;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error OnUpdate Events: " + e.Message);
            }

            try
            {

                if (curEngineT.isOperational && !IsEnabled)
                {
                    IsEnabled = true;
                    part.force_activate();
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error OnUpdate force_activate: " + e.Message);
            }


            try
            {
                var kerbalHazardCount = 0;
                foreach (var vess in FlightGlobals.Vessels)
                {
                    var distance = Vector3d.Distance(vessel.transform.position, vess.transform.position);
                    if (distance < leathalDistance && vess != this.vessel)
                        kerbalHazardCount += vess.GetCrewCount();
                }

                if (kerbalHazardCount > 0)
                {
                    radhazard = true;
                    if (kerbalHazardCount > 1)
                        radhazardstr = kerbalHazardCount + " Kerbals.";
                    else
                        radhazardstr = kerbalHazardCount + " Kerbal.";

                    radhazardstrField.guiActive = true;
                }
                else
                {
                    radhazardstrField.guiActive = false;
                    radhazard = false;
                    radhazardstr = "None.";
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error OnUpdate kerbalHazardCount " + e.Message);
            }
        }

        private void ShutDown(string reason)
        {
            try
            {
                curEngineT.Events["Shutdown"].Invoke();
                curEngineT.currentThrottle = 0;
                curEngineT.requestedThrottle = 0;

                ScreenMessages.PostScreenMessage(reason, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                foreach (var fxGroup in part.fxGroups)
                {
                    fxGroup.setActive(false);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error ShutDown " + e.Message + " stack " + e.StackTrace);
            }
        }

        private void CalculateTimeDialation()
        {
            try
            {
                worldSpaceVelocity = vessel.orbit.GetFrameVel().magnitude;

                lightSpeedRatio = Math.Min(worldSpaceVelocity / speedOfLight, 0.9999999999);

                timeDilation = Math.Sqrt(1 - (lightSpeedRatio * lightSpeedRatio));

                relativity = 1 / timeDilation;                
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error CalculateTimeDialation " + e.Message + " stack " + e.StackTrace);
            }
        }

        public void FixedUpdate()
        {
            try
            {
                if (HighLogic.LoadedSceneIsEditor)
                    return;

                if (!IsEnabled)
                    UpdateTime();

                temperatureStr = part.temperature.ToString("0.0") + "K / " + part.maxTemp.ToString("0.0") + "K";
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error FixedUpdate " + e.Message + " stack " + e.StackTrace);
            }
        }

        private void UpdateTime()
        {
            try
            {
                universalTime = Planetarium.GetUniversalTime();
                CalculateTimeDialation();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error UpdateTime " + e.Message + " stack " + e.StackTrace);
            }
        }

        public override void OnFixedUpdate()
        {
            try
            {
                if (curEngineT == null) return;

                stopWatch.Reset();
                stopWatch.Start();

                UpdateTime();

                var throttle = curEngineT.currentThrottle > 0 ? Mathf.Max(curEngineT.currentThrottle, 0.01f) : 0;

                if (throttle > 0)
                {
                    if (vessel.atmDensity > maxAtmosphereDensity)
                        ShutDown("Inertial Fusion cannot operate in atmosphere!");

                    if (radhazard && rad_safety_features)
                        ShutDown("Engines throttled down as they presently pose a radiation hazard");
                }

                KillKerbalsWithRadiation(throttle);

                if (!this.vessel.packed && !warpToReal)
                    storedThrotle = vessel.ctrlState.mainThrottle;

                // Update ISP
                effectiveIsp = timeDilation * engineIsp;

                UpdateAtmosphericCurve(effectiveIsp);

                if (throttle > 0 && !this.vessel.packed)
                {
                    TimeWarp.GThreshold = 2;

                    var thrustPercentage = (double)(decimal)curEngineT.thrustPercentage;
                    var thrustRatio = Math.Max(thrustPercentage * 0.01, 0.01);
                    var scaledThrottle = Math.Pow(thrustRatio * throttle, ispThrottleExponent);
                    effectiveIsp = timeDilation * engineIsp * scaledThrottle;

                    UpdateAtmosphericCurve(effectiveIsp);

                    fusionRatio = ProcessPowerAndWasteHeat(throttle);

                    // Update FuelFlow
                    effectiveMaxThrustInKiloNewton = timeDilation * timeDilation * MaximumThrust * fusionRatio;
                    calculatedFuelflow = effectiveMaxThrustInKiloNewton / effectiveIsp / PluginHelper.GravityConstant;
                    massFlowRateKgPerSecond = thrustRatio * curEngineT.currentThrottle * calculatedFuelflow * 1000;

                    if (!curEngineT.getFlameoutState && fusionRatio < 0.01)
                    {
                        curEngineT.status = "Insufficient Electricity";
                    }
                }
                else if (this.vessel.packed && curEngineT.enabled && FlightGlobals.ActiveVessel == vessel && throttle > 0 && percentageFuelRemaining > (100 - fuelLimit) && lightSpeedRatio < speedLimit)
                {
                    warpToReal = true; // Set to true for transition to realtime

                    fusionRatio = CheatOptions.InfiniteElectricity 
                        ? 1 
                        : maximizeThrust 
                            ? ProcessPowerAndWasteHeat(1) 
                            : ProcessPowerAndWasteHeat(storedThrotle);

                    effectiveMaxThrustInKiloNewton = timeDilation * timeDilation * MaximumThrust * fusionRatio;
                    calculatedFuelflow = effectiveMaxThrustInKiloNewton / effectiveIsp / PluginHelper.GravityConstant;
                    massFlowRateKgPerSecond = calculatedFuelflow * 1000;

                    if (TimeWarp.fixedDeltaTime > 20)
                    {
                        var deltaCalculations = (float)Math.Ceiling(TimeWarp.fixedDeltaTime / 20);
                        var deltaTimeStep = TimeWarp.fixedDeltaTime / deltaCalculations;

                        for (var step = 0; step < deltaCalculations; step++)
                        {
                            PersistantThrust(deltaTimeStep, universalTime + (step * deltaTimeStep), this.part.transform.up, this.vessel.GetTotalMass());
                            CalculateTimeDialation();
                        }
                    }
                    else
                        PersistantThrust(TimeWarp.fixedDeltaTime, universalTime, this.part.transform.up, this.vessel.GetTotalMass());
                }
                else
                {
                    powerUsage = "0.000 GW / " + (EffectivePowerRequirement / 1000d).ToString("0.000") + " GW";

                    if (!(percentageFuelRemaining > (100 - fuelLimit) || lightSpeedRatio > speedLimit))
                    {
                        warpToReal = false;
                        vessel.ctrlState.mainThrottle = 0;
                    }

                    effectiveMaxThrustInKiloNewton = timeDilation * timeDilation * MaximumThrust;
                    calculatedFuelflow = effectiveMaxThrustInKiloNewton / effectiveIsp / 9.81;
                    massFlowRateKgPerSecond = 0;
                }

                curEngineT.maxFuelFlow = (float)calculatedFuelflow;
                curEngineT.maxThrust = (float)effectiveMaxThrustInKiloNewton;
                
                massFlowRateTonPerHour = massFlowRateKgPerSecond * 3.6;
                thrustPowerInTeraWatt = effectiveMaxThrustInKiloNewton * 500 * effectiveIsp * 9.81 * 1e-12;

                stopWatch.Stop();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("[KSPI] - Error UpdateTime " + e.Message + " stack " + e.StackTrace);
            }
        }

        private void UpdateAtmosphericCurve(double isp)
        {
            var newAtmosphereCurve = new FloatCurve();
            newAtmosphereCurve.Add(0, (float)isp);
            newAtmosphereCurve.Add(maxAtmosphereDensity, 0);
            curEngineT.atmosphereCurve = newAtmosphereCurve;
        }

        private void PersistantThrust(float modifiedFixedDeltaTime, double modifiedUniversalTime, Vector3d thrustVector, float vesselMass)
        {
            var timeDilationMaximumThrust = timeDilation * timeDilation * MaximumThrust * (maximizeThrust ? 1 : storedThrotle);
            var timeDialationEngineIsp = timeDilation * engineIsp;

            thrustVector.CalculateDeltaVV(vesselMass, modifiedFixedDeltaTime, timeDilationMaximumThrust * fusionRatio, timeDialationEngineIsp, out demandMass);

            fuelRatio = CollectFuel(demandMass);

            effectiveMaxThrustInKiloNewton = timeDilationMaximumThrust * fuelRatio;

            if (!(fuelRatio > 0.01)) return;

            var deltaVv = thrustVector.CalculateDeltaVV(vesselMass, modifiedFixedDeltaTime, effectiveMaxThrustInKiloNewton, timeDialationEngineIsp, out demandMass);
            vessel.orbit.Perturb(deltaVv, modifiedUniversalTime);
        }

        private double CollectFuel(double demandMass)
        {
            if (CheatOptions.InfinitePropellant)
                return 1;

            var fusionFuelRequestAmount1 = 0.0;
            var fusionFuelRequestAmount2 = 0.0;
            var fusionFuelRequestAmount3 = 0.0;

            var totalAmount = demandMass / averageDensity;

            double availableRatio = 1;
            if (fusionFuelFactor1 > 0)
            {
                fusionFuelRequestAmount1 = fusionFuelFactor1 * totalAmount;
                availableRatio = Math.Min(part.GetResourceAvailable(ResourceFlowMode.STACK_PRIORITY_SEARCH, fusionFuelResourceDefinition1) / fusionFuelRequestAmount1, availableRatio);
            }
            if (fusionFuelFactor2 > 0)
            {
                fusionFuelRequestAmount2 = fusionFuelFactor2 * totalAmount;
                availableRatio = Math.Min(part.GetResourceAvailable(ResourceFlowMode.STACK_PRIORITY_SEARCH, fusionFuelResourceDefinition2) / fusionFuelRequestAmount2, availableRatio);
            }
            if (fusionFuelFactor3 > 0)
            {
                fusionFuelRequestAmount3 = fusionFuelFactor3 * totalAmount;
                availableRatio = Math.Min(part.GetResourceAvailable(ResourceFlowMode.STACK_PRIORITY_SEARCH, fusionFuelResourceDefinition3) / fusionFuelRequestAmount3, availableRatio);
            }

            if (availableRatio <= float.Epsilon)
                return 0;

            double recievedRatio = 1;
            if (fusionFuelFactor1 > 0)
            {
                var recievedFusionFuel = part.RequestResource(fusionFuelResourceDefinition1.id, fusionFuelRequestAmount1 * availableRatio, ResourceFlowMode.STACK_PRIORITY_SEARCH);
                recievedRatio = Math.Min(recievedRatio, fusionFuelRequestAmount1 > 0 ? recievedFusionFuel / fusionFuelRequestAmount1 : 0);
            }
            if (fusionFuelFactor2 > 0)
            {
                var recievedFusionFuel = part.RequestResource(fusionFuelResourceDefinition2.id, fusionFuelRequestAmount2 * availableRatio, ResourceFlowMode.STACK_PRIORITY_SEARCH);
                recievedRatio = Math.Min(recievedRatio, fusionFuelRequestAmount2 > 0 ? recievedFusionFuel / fusionFuelRequestAmount2 : 0);
            }
            if (fusionFuelFactor3 > 0)
            {
                var recievedFusionFuel = part.RequestResource(fusionFuelResourceDefinition3.id, fusionFuelRequestAmount3 * availableRatio, ResourceFlowMode.STACK_PRIORITY_SEARCH);
                recievedRatio = Math.Min(recievedRatio, fusionFuelRequestAmount3 > 0 ? recievedFusionFuel / fusionFuelRequestAmount3 : 0);
            }
            return recievedRatio;
        }

        private double ProcessPowerAndWasteHeat(float throtle)
        {
            // Calculate Fusion Ratio
            var effectivePowerRequirement = EffectivePowerRequirement;
            var thrustPercentage = (double)(decimal)curEngineT.thrustPercentage;
            var thrustRatio = Math.Max(thrustPercentage * 0.01, 0.01);

            var scaledThrottle = Math.Pow(thrustRatio * throtle, powerThrottleExponent);

            var requestedPower = scaledThrottle * effectivePowerRequirement;

            var recievedPower = CheatOptions.InfiniteElectricity 
                ? requestedPower
                : consumeFNResourcePerSecond(requestedPower, ResourceManager.FNRESOURCE_MEGAJOULES);

            var plasmaRatio = effectivePowerRequirement > 0 ? recievedPower / requestedPower : 0;

            powerUsage = (recievedPower * 0.001).ToString("0.000") + " GW / " + (requestedPower * 0.001).ToString("0.000") + " GW";

            // The Aborbed wasteheat from Fusion production and reaction
            if (!CheatOptions.IgnoreMaxTemperature)
                supplyFNResourcePerSecond(scaledThrottle * FusionWasteHeat * wasteHeatMultiplier * plasmaRatio, ResourceManager.FNRESOURCE_WASTEHEAT);

            return plasmaRatio;
        }

        private void KillKerbalsWithRadiation(float throttle)
        {
            if (!radhazard || throttle <= 0 || rad_safety_features) return;

            var vesselsToRemove = new List<Vessel>();
            var crewToRemove = new List<ProtoCrewMember>();

            foreach (var vess in FlightGlobals.Vessels)
            {
                var distance = Vector3d.Distance(vessel.transform.position, vess.transform.position);

                if (distance >= leathalDistance || vess == this.vessel || vess.GetCrewCount() <= 0) continue;

                var invSqDist = distance / killDivider;
                var invSqMult = 1d / invSqDist / invSqDist;

                foreach (var crewMember in vess.GetVesselCrew())
                {
                    if (UnityEngine.Random.value < (1d - TimeWarp.fixedDeltaTime * invSqMult)) continue;

                    if (!vess.isEVA)
                    {
                        ScreenMessages.PostScreenMessage(crewMember.name + " was killed by Neutron Radiation!", 5f, ScreenMessageStyle.UPPER_CENTER);
                        crewToRemove.Add(crewMember);
                    }
                    else
                    {
                        ScreenMessages.PostScreenMessage(crewMember.name + " was killed by Neutron Radiation!", 5f, ScreenMessageStyle.UPPER_CENTER);
                        vesselsToRemove.Add(vess);
                    }
                }
            }

            foreach (var vess in vesselsToRemove)
            {
                vess.rootPart.Die();
            }

            foreach (var crewMember in crewToRemove)
            {
                var vess = FlightGlobals.Vessels.Find(p => p.GetVesselCrew().Contains(crewMember));
                var partWithCrewMember = vess.Parts.Find(p => p.protoModuleCrew.Contains(crewMember));
                partWithCrewMember.RemoveCrewmember(crewMember);
                crewMember.Die();
            }
        }

        public override int getPowerPriority() 
        {
            return powerPriority;
        }

        public override string GetInfo()
        {
            DetermineTechLevel();

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(upgradeTechReq1))
            {
                sb.AppendLine("<color=#7fdfffff>" + Localizer.Format("#LOC_KSPIE_Generic_upgradeTechnologies") + ":</color><size=10>");
                if (!string.IsNullOrEmpty(upgradeTechReq1)) sb.AppendLine("- " + Localizer.Format(PluginHelper.GetTechTitleById(upgradeTechReq1)));
                if (!string.IsNullOrEmpty(upgradeTechReq2)) sb.AppendLine("- " + Localizer.Format(PluginHelper.GetTechTitleById(upgradeTechReq2)));
                if (!string.IsNullOrEmpty(upgradeTechReq3)) sb.AppendLine("- " + Localizer.Format(PluginHelper.GetTechTitleById(upgradeTechReq3)));
                if (!string.IsNullOrEmpty(upgradeTechReq4)) sb.AppendLine("- " + Localizer.Format(PluginHelper.GetTechTitleById(upgradeTechReq4)));
                if (!string.IsNullOrEmpty(upgradeTechReq5)) sb.AppendLine("- " + Localizer.Format(PluginHelper.GetTechTitleById(upgradeTechReq5)));
                if (!string.IsNullOrEmpty(upgradeTechReq6)) sb.AppendLine("- " + Localizer.Format(PluginHelper.GetTechTitleById(upgradeTechReq6)));
                if (!string.IsNullOrEmpty(upgradeTechReq7)) sb.AppendLine("- " + Localizer.Format(PluginHelper.GetTechTitleById(upgradeTechReq7)));
                if (!string.IsNullOrEmpty(upgradeTechReq8)) sb.AppendLine("- " + Localizer.Format(PluginHelper.GetTechTitleById(upgradeTechReq8)));
                sb.Append("</size>");
                sb.AppendLine();
            }

            sb.AppendLine("<color=#7fdfffff>" + Localizer.Format("#LOC_KSPIE_Generic_EnginePerformance") + ":</color><size=10>");
            sb.AppendLine(FormatThrustStatistics(maxThrustMk1, thrustIspMk1));
            if (!string.IsNullOrEmpty(upgradeTechReq1)) sb.AppendLine(FormatThrustStatistics(maxThrustMk2, thrustIspMk2));
            if (!string.IsNullOrEmpty(upgradeTechReq2)) sb.AppendLine(FormatThrustStatistics(maxThrustMk3, thrustIspMk3));
            if (!string.IsNullOrEmpty(upgradeTechReq3)) sb.AppendLine(FormatThrustStatistics(maxThrustMk4, thrustIspMk4));
            if (!string.IsNullOrEmpty(upgradeTechReq4)) sb.AppendLine(FormatThrustStatistics(maxThrustMk5, thrustIspMk5));
            if (!string.IsNullOrEmpty(upgradeTechReq5)) sb.AppendLine(FormatThrustStatistics(maxThrustMk6, thrustIspMk6));
            if (!string.IsNullOrEmpty(upgradeTechReq6)) sb.AppendLine(FormatThrustStatistics(maxThrustMk7, thrustIspMk7));
            if (!string.IsNullOrEmpty(upgradeTechReq7)) sb.AppendLine(FormatThrustStatistics(maxThrustMk8, thrustIspMk8));
            if (!string.IsNullOrEmpty(upgradeTechReq8)) sb.AppendLine(FormatThrustStatistics(maxThrustMk9, thrustIspMk9));
            
            sb.Append("</size>");
            sb.AppendLine();

            sb.AppendLine("<color=#7fdfffff>" + Localizer.Format("#LOC_KSPIE_Generic_PowerRequirementAndWasteheat") + ":</color><size=10>");
            sb.AppendLine(FormatPowerStatistics(powerRequirementMk1, wasteheatMk1));
            if (!string.IsNullOrEmpty(upgradeTechReq1)) sb.AppendLine(FormatPowerStatistics(powerRequirementMk2, wasteheatMk2));
            if (!string.IsNullOrEmpty(upgradeTechReq2)) sb.AppendLine(FormatPowerStatistics(powerRequirementMk3, wasteheatMk3));
            if (!string.IsNullOrEmpty(upgradeTechReq3)) sb.AppendLine(FormatPowerStatistics(powerRequirementMk4, wasteheatMk4));
            if (!string.IsNullOrEmpty(upgradeTechReq4)) sb.AppendLine(FormatPowerStatistics(powerRequirementMk5, wasteheatMk5));
            if (!string.IsNullOrEmpty(upgradeTechReq5)) sb.AppendLine(FormatPowerStatistics(powerRequirementMk6, wasteheatMk6));
            if (!string.IsNullOrEmpty(upgradeTechReq6)) sb.AppendLine(FormatPowerStatistics(powerRequirementMk7, wasteheatMk7));
            if (!string.IsNullOrEmpty(upgradeTechReq7)) sb.AppendLine(FormatPowerStatistics(powerRequirementMk8, wasteheatMk8));
            if (!string.IsNullOrEmpty(upgradeTechReq8)) sb.AppendLine(FormatPowerStatistics(powerRequirementMk9, wasteheatMk9));
            sb.Append("</size>");

            return sb.ToString();
        }
    }
}


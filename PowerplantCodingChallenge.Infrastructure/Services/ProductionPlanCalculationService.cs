using Microsoft.Extensions.Logging;
using PowerplantCodingChallenge.Domain.Models;
using PowerplantCodingChallenge.Domain.Services;

namespace PowerplantCodingChallenge.Infrastructure.Services
{
    public class ProductionPlanCalculationService : IProductionPlanCalculationService
    {
        private const string WIND_PERCENTAGE_KEY = "wind(%)";
        private const string FUEL_TYPE_GAS_KEY = "gas(euro/MWh)";
        private const string FUEL_TYPE_KEROSINE_KEY = "kerosine(euro/MWh)";

        private readonly ILogger<ProductionPlanCalculationService> _logger;

        public ProductionPlanCalculationService(ILogger<ProductionPlanCalculationService> logger)
        {
            _logger = logger;
        }

        public IList<ProductionPlanResponse> GenerateProductionPlan(ProductionPlanPayload productionPlanPayload)
        {
            if (!IsProductionPlanInputValid(productionPlanPayload)) return Enumerable.Empty<ProductionPlanResponse>().ToList();

            return GetProductionPlan(productionPlanPayload);
        }

        private IList<ProductionPlanResponse> GetProductionPlan(ProductionPlanPayload payload)
        {
            var selectedPowerplants = new List<ProductionPlanResponse>();

            if (!(payload.Fuels.TryGetValue(WIND_PERCENTAGE_KEY, out double windPercentage)
                && payload.Fuels.TryGetValue(FUEL_TYPE_GAS_KEY, out double gasPrice)
                && payload.Fuels.TryGetValue(FUEL_TYPE_KEROSINE_KEY, out double kerosinePrice)))
            {
                _logger.LogError("Input data is invalid.");
            }
            else
            {
                var load = payload.Load;
                var powerplants = payload.Powerplants;

                // Using plants as per their cost and efficiency
                if (windPercentage > 0) load = UseWindPlants(load, powerplants, windPercentage, selectedPowerplants);
                load = UseGasFiredPlants(load, powerplants, gasPrice, selectedPowerplants);
                UseTurboJetPlants(load, powerplants, kerosinePrice, selectedPowerplants);
            }

            return selectedPowerplants;
        }

        private double UseWindPlants(double load, IList<Powerplant> powerplants, double windPercentage, List<ProductionPlanResponse> selectedPowerplants)
        {
            // Filtering wind turbines
            var windTurbines = powerplants.Where(p => p.Type == PowerplantType.WindTurbine).ToList();
            foreach (var windTurbine in windTurbines)
            {
                // Calculating turbine power based on wind
                double turbinePower = windTurbine.Pmax * windPercentage / 100;
                load -= turbinePower;
                selectedPowerplants.Add(new ProductionPlanResponse { Name = windTurbine.Name, Power = turbinePower });
            }
            return load;
        }

        private double UseGasFiredPlants(double load, IList<Powerplant> powerplants, double gasPrice, List<ProductionPlanResponse> selectedPowerplants)
        {
            if (load <= 0)
                return load;

            // Filtering gasfired turbines
            var gasfiredPlants = powerplants.Where(p => p.Type == PowerplantType.GasFired).ToList();

            // If load is less then use a gasfired plant having minimum power so that extra energy can be minimized
            var minPowerplant = gasfiredPlants.OrderBy(x => x.Pmin).FirstOrDefault();
            if (load <= minPowerplant.Pmin)
            {
                selectedPowerplants.Add(new ProductionPlanResponse { Name = minPowerplant.Name, Power = minPowerplant.Pmin });
                return 0;
            }

            // Ordering plants by their efficiency and then by their cost as gasfired plant produces a certain min. power when turned on
            gasfiredPlants = gasfiredPlants.OrderByDescending(p => p.Efficiency).ThenBy(p => p.Pmin * gasPrice).ToList();
            return SelectPowerplantByLoad(load, gasfiredPlants, selectedPowerplants);
        }

        private double UseTurboJetPlants(double load, IList<Powerplant> powerplants, double kerosinePrice, List<ProductionPlanResponse> selectedPowerplants)
        {
            if (load <= 0)
                return load;

            // Filtering turbojet turbines and ordering them by their efficiency and then by their cost wrt to their max power
            var turboJetPlants = powerplants.Where(p => p.Type == PowerplantType.TurboJet).OrderByDescending(p => p.Efficiency).ThenBy(p => p.Pmax * kerosinePrice).ToList();
            return SelectPowerplantByLoad(load, turboJetPlants, selectedPowerplants);
        }

        private double SelectPowerplantByLoad(double load, List<Powerplant> powerplants, List<ProductionPlanResponse> selectedPowerplants)
        {
            // Allocating remaining load
            foreach (var powerplant in powerplants)
            {
                // Checking if plant capacity or load is lower and then decreasing load and moving to next plant
                double availablePower = Math.Min(load, powerplant.Pmax);
                if (availablePower >= powerplant.Pmin)
                {
                    selectedPowerplants.Add(new ProductionPlanResponse { Name = powerplant.Name, Power = availablePower });
                    load -= availablePower;
                }
                else
                {
                    selectedPowerplants.Add(new ProductionPlanResponse { Name = powerplant.Name, Power = powerplant.Pmin });
                    load -= powerplant.Pmin;
                }

                if (load <= 0)
                    break;
            }
            return load;
        }

        private bool IsProductionPlanInputValid(ProductionPlanPayload productionPlanPayload)
        {
            if (productionPlanPayload == null
                || productionPlanPayload.Load <= 0
                || !(productionPlanPayload.Fuels?.Any() ?? false)
                || !(productionPlanPayload.Powerplants?.Any() ?? false))
            {
                _logger.LogError("Input data is invalid.");
                return false;
            }

            var totalPower = productionPlanPayload.Powerplants.Sum(x => x.Pmax);
            if (productionPlanPayload.Load > totalPower)
            {
                _logger.LogError("Power plants capacity is less than load.");
                return false;
            }

            return true;
        }
    }
}

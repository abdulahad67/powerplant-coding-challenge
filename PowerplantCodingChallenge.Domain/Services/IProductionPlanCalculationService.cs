using PowerplantCodingChallenge.Domain.Models;

namespace PowerplantCodingChallenge.Domain.Services
{
    public interface IProductionPlanCalculationService
    {
        IList<ProductionPlanResponse> GenerateProductionPlan(ProductionPlanPayload productionPlanPayload);
    }
}

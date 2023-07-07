using Microsoft.AspNetCore.Mvc;
using PowerplantCodingChallenge.Domain.Models;
using PowerplantCodingChallenge.Domain.Services;

namespace PowerplantCodingChallenge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PowerplantController : ControllerBase
    {
        private readonly IProductionPlanCalculationService _productionPlanCalculationService;

        public PowerplantController(IProductionPlanCalculationService productionPlanCalculationService)
        {
            _productionPlanCalculationService = productionPlanCalculationService;
        }

        [HttpPost("productionplan")]
        public ActionResult<IEnumerable<ProductionPlanResponse>> BuildProductionPlan([FromBody] ProductionPlanPayload productionPlanPayload)
        {
            var productionPlan = _productionPlanCalculationService.GenerateProductionPlan(productionPlanPayload);
            if (productionPlan?.Any() ?? false)
                return Ok(productionPlan);
            return BadRequest();
        }
    }
}

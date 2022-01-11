using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MindGamesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InCaseINeedItController : ControllerBase
    {
        private readonly ILogger<InCaseINeedItController> _logger;

        public InCaseINeedItController(ILogger<InCaseINeedItController> logger) => this._logger = logger;

        [HttpGet]
        public int Get()
        {
            var rng = new Random();

            return rng.Next(0, 100);
        }
    }
}

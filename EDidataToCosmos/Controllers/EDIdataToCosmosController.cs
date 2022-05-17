using EDIdataToCosmos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EDidataToCosmos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EDIdataToCosmosController : ControllerBase
    {
        // POST api/<EDIdataToCosmosController>
        [HttpPost]
        public IActionResult Post(string value)
        {
            if (value == "init")
            {
                EdiParcer ediParcer = new EdiParcer();
                string textjsonModel = ediParcer.parseEDI();
                //string textjsonModel = System.IO.File.ReadAllText(@"D:\intership\frontend\week9\ediParcer\ediParcer\output.json");
                dynamic jsonModel = JsonConvert.DeserializeObject(textjsonModel);
                dynamic temp = jsonModel["Groups"][0]["Transactions"];

                CosmosConnect cosmosConnect = new CosmosConnect();
                cosmosConnect.CreateDatabaseAsync().Wait();
                cosmosConnect.CreateContainerAsync().Wait();

                foreach (var x in temp)
                {
                    cosmosConnect.AddItemsToContainerAsync(x).Wait();
                }
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


    }
}

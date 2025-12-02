using DogsHouseApp.Models;
using DogsHouseApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DogsHouseApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDogService _dogService;

        public HomeController(IDogService dogService)
        {
            _dogService = dogService;
        }

        [HttpGet("Ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok("Dogs House service. Version 1.0.1");
        }

        [HttpGet("Dogs")]
        public async Task<IActionResult> Dogs(string attribute = null, string order = null, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                var result = await _dogService.GetDogsAsync(attribute, order, pageNumber, pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("Dog")]
        public async Task<IActionResult> Dog([FromBody]DogModel dogModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest($"Request model is not valid");
                }

                if (await _dogService.IsDogNameAlreadyExistsAsync(dogModel.Name))
                {
                    return BadRequest($"Dog with name {dogModel.Name} is already exist!");
                }
                
                if (dogModel.Weight <= 0)
                {
                    return BadRequest($"Weight is not valid");
                }
                
                if (dogModel.TailLength <= 0)
                {
                    return BadRequest($"Tail length is not valid");
                }

                await _dogService.AddDogAsync(dogModel);

                return Ok("Created");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }           
            
        }
    }
}

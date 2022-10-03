using drones_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;
using drones_api.Entities.AppDBContext;
using System.Threading.Tasks;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace drones_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DronesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly DronDBContext dbcontext;

        public DronesController(IMapper mapper, DronDBContext dbcontext)
        {
            this.mapper = mapper;
            this.dbcontext = dbcontext;
        }
        // GET: api/<DronesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list =await dbcontext.Drones.ToListAsync();
            return Ok( list);
        }

        // GET api/<DronesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DronesController>
        [HttpPost]
        public async  Task<IActionResult> Post([FromBody] Dron dron)
        {
            try
            {
                var found = await dbcontext.Drones.AnyAsync(x => x.SerialNumber.Equals(dron.SerialNumber));
                if (found) return BadRequest();
                //mapper aqui
                dbcontext.Drones.Add(dron);
                await dbcontext.SaveChangesAsync();

                return Ok(dron);
            }
            catch(Exception e)
            {
                return this.Problem();
            }
        }

        // PUT api/<DronesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Dron dron)
        {
        }
    }
}

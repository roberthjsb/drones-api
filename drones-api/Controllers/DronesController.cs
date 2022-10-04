using drones_api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;
using drones_api.Entities.AppDBContext;
using System.Threading.Tasks;
using System;
using drones_api.DTOS;
using System.Collections.Generic;

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
            var list = await dbcontext.Drones.ToListAsync();
            List<DronDtoResult> result = mapper.Map<List<Dron>, List<DronDtoResult>>(list);
            return Ok( result);
        }

        [HttpGet("{dronSerialNumber}/batery")]
        public async Task<IActionResult> GetStatusBatery(string dronSerialNumber)
        {
            try
            {
                Dron dronFound = await dbcontext
                    .Drones
                    .FirstOrDefaultAsync(x => x.SerialNumber.Equals(dronSerialNumber));
                if (dronFound == null) return BadRequest();
                return Ok(dronFound.BateryLevel);
            }
            catch (Exception)
            {
                return this.Problem();
            }
        }

        [HttpGet("{dronSerialNumber}/weight")]
        public async Task<IActionResult> GetStatusWeight(string dronSerialNumber)
        {
            try
            {
                Dron dronFound = await dbcontext
                     .Drones.Include(e => e.Medicines)
                     .FirstOrDefaultAsync(x => x.SerialNumber.Equals(dronSerialNumber));
                if (dronFound == null) return BadRequest();
                var pesoCargaDron = dronFound.Medicines.Select(e => e.Weigth).Sum();
                return StatusCode(pesoCargaDron);
            }
            catch (Exception)
            {
                return this.Problem();
            }
        }

        // POST api/<DronesController>
        [HttpPost]
        public async  Task<IActionResult> Post([FromBody] RegisterDronDTO drondto)
        {
            try
            {
                var found = await dbcontext.Drones.AnyAsync(x => x.SerialNumber.Equals(drondto.SerialNumber));
                if (found) return BadRequest();
                var dron =mapper.Map<RegisterDronDTO, Dron>(drondto);
                dbcontext.Drones.Add(dron);
                await dbcontext.SaveChangesAsync();

                return Ok(dron);
            }
            catch(Exception)
            {
                return this.Problem();
            }
        }

        // PUT api/<DronesController>/5
       
        [HttpPost("{dronSerialNumber}/medicine")]
        public async Task<IActionResult> PostMedicine(string dronSerialNumber, [FromForm] MedicineDto medicinedto)
        {
            try
            {
                Dron dronFound = await dbcontext
                    .Drones.Include(e => e.Medicines)
                    .FirstOrDefaultAsync(x => x.SerialNumber.Equals(dronSerialNumber));

                if (dronFound==null) return NotFound();
                //TODO: validar estados
                if (dronFound.BateryLevel < 25) return Conflict();
                dronFound.State = "CARGANDO";
                await dbcontext.SaveChangesAsync();



                var medicine = mapper.Map<MedicineDto, Medicine>(medicinedto);
                var pesoCargaDron = dronFound.Medicines.Select(e => e.Weigth).Sum();

                if((pesoCargaDron + medicine.Weigth) > dronFound.LimitWeight) return Conflict();

                dronFound.Medicines.Add(medicine);
                await dbcontext.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (Exception)
            {
                return this.Problem();
            }
        }


    }
}

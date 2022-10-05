using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using drones_api.DTOS;
using drones_api.Entities;
using drones_api.Enums;
using drones_api.Results;
using drones_api.Services.Contracts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace drones_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DronesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IDronService dronService;
        private readonly ILogger logger;

        public DronesController(IMapper mapper, IDronService dronService, ILogger<DronesController> logger)
        {
            this.mapper = mapper;
            this.dronService = dronService;
            this.logger = logger;
        }
        // GET: api/<DronesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Dron> list = await dronService.GetListDrones();
                List<DronDtoResult> result = mapper.Map<List<Dron>, List<DronDtoResult>>(list);
                return Ok(result);
            }
            catch (Exception e)
            {
                return HandlerError(e);
            }

        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            try
            {
                var list = await dronService.GetListDronesAvailable();
                List<DronDtoResult> result = mapper.Map<List<Dron>, List<DronDtoResult>>(list);
                return Ok(result);
            }
            catch (Exception e)
            {
                return HandlerError(e);
            }
        }


        [HttpGet("{dronSerialNumber}/batery")]
        public async Task<IActionResult> GetStatusBatery(string dronSerialNumber)
        {
            try
            {
                Dron dronFound = await dronService.GetDronById(dronSerialNumber);
                if (dronFound == null) return BadRequest();
                return Ok(dronFound.BateryLevel);
            }
            catch (Exception e)
            {
                return HandlerError(e);
            }
        }

        [HttpGet("{dronSerialNumber}/weight")]
        public async Task<IActionResult> GetStatusWeight(string dronSerialNumber)
        {
            try
            {
                Dron dronFound = await dronService.GetDronById(dronSerialNumber);
                if (dronFound == null) return BadRequest();
                var pesoCargaDron = dronFound.Medicines.Select(e => e.Weigth).Sum();
                return StatusCode(pesoCargaDron);
            }
            catch (Exception e)
            {
                return HandlerError(e);
            }
        }

        // POST api/<DronesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterDronDTO drondto)
        {
            try
            {
                if (await dronService.ExistDron(drondto.SerialNumber))
                    return BadRequest();
                var dron = mapper.Map<RegisterDronDTO, Dron>(drondto);
                await dronService.Register(dron);
                return Ok(dron);
            }
            catch (Exception e)
            {
                return HandlerError(e);
            }
        }

       

        // PUT api/<DronesController>/5

        [HttpPost("{dronSerialNumber}/medicine")]
        public async Task<IActionResult> PostMedicine(string dronSerialNumber, [FromForm] MedicineDto medicinedto)
        {
            try
            {
                Dron dronFound = await dronService.GetDronById(dronSerialNumber);

                if (dronFound == null) return NotFound(new ErrorResult("Dron not registed"));
                if (dronFound.BateryLevel < 25) return Conflict(new ErrorResult("Dron low Battery"));
                if (dronFound.State != DronState.INACTIVO) return Conflict(new ErrorResult("Dron not available"));

                dronFound.State = DronState.CARGANDO;
                await dronService.SaveChange();

                var medicine = mapper.Map<MedicineDto, Medicine>(medicinedto);
                var currentWeigthDron = dronFound.Medicines.Select(e => e.Weigth).Sum();
                if ((currentWeigthDron + medicine.Weigth) > dronFound.LimitWeight)
                {
                    return Conflict(new ErrorResult("over max weigth"));
                }
                if ((currentWeigthDron + medicine.Weigth) == dronFound.LimitWeight)
                {
                    dronFound.State = DronState.CARGADO;
                    await dronService.SaveChange();
                }

                dronFound.Medicines.Add(medicine);
                await dronService.SaveChange();

                return StatusCode(201);
            }
            catch (Exception e)
            {
                return HandlerError(e);
            }
        }

        private ObjectResult HandlerError(Exception e)
        {
            logger.LogError(e.Message);
            return this.Problem();
        }


    }
}

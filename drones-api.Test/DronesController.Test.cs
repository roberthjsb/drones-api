using Xunit;
using drones_api.Controllers;
using Moq;
using drones_api.Services.Contracts;
using drones_api.Adapters;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net;
using drones_api.Entities;
using System.Collections.Generic;
using drones_api.DTOS;
using System.Linq;
using drones_api.Test.FixtureClasses;
using System;
using drones_api.Results;

namespace drones_api.Test
{
    public class DroneControllerTest
    {
        private readonly Mock<IDronService> dronServiceMock;
        private readonly IMapper mapper;
        private readonly Mock<ILogger<DronesController>> loggerMock;

        private readonly DronesController controller;
        public DroneControllerTest()
        {
            dronServiceMock = new Mock<IDronService>();
            AutoMapperProfile mapperProfile = new AutoMapperProfile();
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            mapper = new Mapper(mapperConfiguration);
            loggerMock = new Mock<ILogger<DronesController>>();


            controller = new DronesController(mapper, dronServiceMock.Object, loggerMock.Object);
        }
        [Fact]
        public async Task DronController_Get_must_return_List_drones()
        {
            List<Dron> drones = new List<Dron> { GetDronTestData()};

            dronServiceMock.Setup(s => s.GetListDrones()).ReturnsAsync(drones);


            var result = await controller.Get() as OkObjectResult;


            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.Collection(result.Value as List<DronDtoResult>, item =>
            {
                Assert.Equal(drones[0].SerialNumber, item.SerialNumber);
                Assert.Equal(drones[0].BateryLevel, item.BateryLevel);
            });


        }

        [Fact]
        public async Task DronController_Get_must_return_list_drones_state_inactive()
        {

            dronServiceMock.Setup(s => s.GetListDronesAvailable());

            await controller.GetAvailable();

            dronServiceMock.Verify(s => s.GetListDronesAvailable(), Times.Once);
        }


        [Fact]
        public async Task DronController_GetStatusBatery_must_call_GetDronById_in_dronservice_and_return_status_battery()
        {
            Dron dron = GetDronTestData();

            dronServiceMock.Setup(s => s.GetDronById(It.IsAny<string>())).ReturnsAsync(dron);

            var result = await controller.GetStatusBatery(dron.SerialNumber) as OkObjectResult;

            dronServiceMock.Verify(s => s.GetDronById(It.IsAny<string>()), Times.Once);
            Assert.Equal(dron.BateryLevel, (int)result.Value);
        }

        [Fact]
        public async Task DronController_GetStatusBatery_must_call_GetDronById_in_dronservice_and_return_NotFound()
        {
            dronServiceMock.Setup(s => s.GetDronById(It.IsAny<string>()));

            var result = await controller.GetStatusBatery("A00000") as NotFoundObjectResult;
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);

            dronServiceMock.Verify(s => s.GetDronById(It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public async Task DronController_GetStatusWeight_must_call_GetDronById_in_dronservice_and_return_weight_dron()
        {
            Dron dron = GetDronTestData();
            dron.Medicines = dron.Medicines = GetListMedicine();
            int totalWeigthMedicine = 54;

            dronServiceMock.Setup(s => s.GetDronById(It.IsAny<string>())).ReturnsAsync(dron);

            var result = await controller.GetStatusWeight("A0045") as OkObjectResult;

            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.Equal(totalWeigthMedicine, (int)result.Value);

            dronServiceMock.Verify(s => s.GetDronById(It.Is<string>(arg => arg.Equals(dron.SerialNumber))), Times.Once);
        }

        [Fact]
        public async Task DronController_GetStatusWeight_must_call_GetDronById_in_dronservice_and_return_not_found()
        {
            Dron dron = GetDronTestData();
            dron.Medicines = GetListMedicine();

            dronServiceMock.Setup(s => s.GetDronById(It.IsAny<string>()));

            var result = await controller.GetStatusWeight("A0000") as NotFoundObjectResult;
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);

            dronServiceMock.Verify(s => s.GetDronById(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public async Task DronController_Post_must_call_register_in_dronservice_and_return_dron_created()
        {
            RegisterDronDTO dronDto = GetDronDtoTestData();

            dronServiceMock.Setup(s => s.Register(It.IsAny<Dron>()));
            dronServiceMock.Setup(s => s.ExistDron(It.IsAny<string>())).ReturnsAsync(false);

            var result = await controller.Post(dronDto) as ObjectResult;


            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            dronServiceMock.Verify(s => s.Register(It.Is<Dron>(o => o.SerialNumber.Equals(dronDto.SerialNumber))), Times.Once);

        }


        [Fact]
        public async Task DronController_Post_must_call_register_in_dronservice_and_return_dron_not_found()
        {
            RegisterDronDTO dronDto = GetDronDtoTestData();

            dronServiceMock.Setup(s => s.Register(It.IsAny<Dron>()));
            dronServiceMock.Setup(s => s.ExistDron(It.IsAny<string>())).ReturnsAsync(true);
            

            var result = await controller.Post(dronDto) as ObjectResult;


            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
            dronServiceMock.Verify(s => s.Register(It.Is<Dron>(o => o.SerialNumber.Equals(dronDto.SerialNumber))), Times.Never);

        }

      

        [Fact]
        public async Task DronController_PostMedicine_must_load_medicine_information_in_dron()
        {

            Dron dron = GetDronTestData();
            var med = GetListMedicineDto()[0];
            dronServiceMock.Setup(s => s.GetDronById(It.IsAny<string>())).ReturnsAsync(dron);
            dronServiceMock.Setup(s => s.SaveChange());

            var result = await controller.PostMedicine("A0045", med) as StatusCodeResult;
            
            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            dronServiceMock.Verify(C => C.SaveChange(), Times.Exactly(2));

        }

        [Fact]
        public async Task DronController_PostMedicine_must_return_NotFound_when_dron_not_exists()
        {
            var med = GetListMedicineDto()[0];
            dronServiceMock.Setup(s => s.GetDronById(It.IsAny<string>()));
            dronServiceMock.Setup(s => s.SaveChange());

            var result = await controller.PostMedicine("A0045", med) as ObjectResult;
            
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
            dronServiceMock.Verify(C => C.SaveChange(), Times.Never);

        }

        [Fact]
        public async Task DronController_PostMedicine_must_return_status_code_201()
        {
            Dron dron = GetDronTestData();
            dron.BateryLevel = 100;
            var med = GetListMedicineDto()[0];
            med.Weigth = dron.LimitWeight;
            dronServiceMock.Setup(s => s.GetDronById(It.IsAny<string>())).ReturnsAsync(dron);
            dronServiceMock.Setup(s => s.SaveChange());

            var result = await controller.PostMedicine("A0045", med) as StatusCodeResult;

            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            dronServiceMock.Verify(C => C.SaveChange(), Times.Exactly(3));
        }

        [Fact]
        public async Task DronController_PostMedicine_must_return_Conflict_because_State_is_not_available()
        {
            Dron dron = GetDronTestData();
            dron.State = Enums.DronState.CARGANDO;
            var med = GetListMedicineDto()[0];
            dronServiceMock.Setup(s => s.GetDronById(It.IsAny<string>())).ReturnsAsync(dron);
            dronServiceMock.Setup(s => s.SaveChange());

            var result = await controller.PostMedicine("A0045", med) as ObjectResult;

            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)result.StatusCode);
            dronServiceMock.Verify(C => C.SaveChange(), Times.Never);
            Assert.Equal("Dron not available", ((ErrorResult)result.Value).Message);

        }
        [Fact]
        public async Task DronController_PostMedicine_must_return_Conflict_because_Dron_is_low_battery()
        {
            Dron dron = GetDronTestData();
            dron.State = Enums.DronState.INACTIVO;
            dron.BateryLevel = 23;
            var med = GetListMedicineDto()[0];
            dronServiceMock.Setup(s => s.GetDronById(It.IsAny<string>())).ReturnsAsync(dron);
            dronServiceMock.Setup(s => s.SaveChange());

            var result = await controller.PostMedicine("A0045", med) as ObjectResult;

            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)result.StatusCode);
            dronServiceMock.Verify(C => C.SaveChange(), Times.Never);
            Assert.Equal("Dron low Battery", ((ErrorResult)result.Value).Message);
        }


        [Fact]
        public async Task DronController_PostMedicine_must_return_Conflict_because_Dron_weigth_exceed_limit()
        {
            Dron dron = GetDronTestData();
            dron.State = Enums.DronState.INACTIVO;
            var med = GetListMedicineDto()[0];
            med.Weigth = dron.LimitWeight+1;
            dronServiceMock.Setup(s => s.GetDronById(It.IsAny<string>())).ReturnsAsync(dron);
            dronServiceMock.Setup(s => s.SaveChange());

            var result = await controller.PostMedicine("A0045", med) as ObjectResult;

            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)result.StatusCode);
            Assert.Equal("over max weigth", ((ErrorResult)result.Value).Message);
        }

        private Dron GetDronTestData()
        {
            return new Dron()
            {
                SerialNumber = "A0045",
                State = Enums.DronState.INACTIVO,
                BateryLevel = 56,
                LimitWeight = 200,
                Model = "peso ligero"

            };
        }
        private RegisterDronDTO GetDronDtoTestData()
        {
            return new RegisterDronDTO()
            {
                SerialNumber = "A0045",
                LimitWeight = 200,
                Model = "peso ligero",
            };
        }

        private List<Medicine> GetListMedicine()
        {
            return new List<Medicine>() { new Medicine() { Code = "0989433", Nombre = "medicina 2021", Weigth = 22 }, new Medicine() { Code = "0989433", Nombre = "medicina ABCD", Weigth = 32 } };
        }
        private List<MedicineDto> GetListMedicineDto()
        {
            return new List<MedicineDto>()
            {
                new MedicineDto()
                {
                    Code = "0989433", Nombre = "medicina 2021", Weigth = 22
                },
                new MedicineDto()
                {
                    Code = "0989005", Nombre = "medicina abc", Weigth = 99
                },
            };
        }


    }
}

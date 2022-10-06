using drones_api.Services.Contracts;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace drones_api.Test.FixtureClasses
{
    class DronControllerClassFixture : IDisposable
    {
        Mock<IDronService> dronServiceMock;

        public DronControllerClassFixture()
        {
           dronServiceMock = new Mock<IDronService>();
           

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

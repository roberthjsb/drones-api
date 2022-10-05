using System.Collections.Generic;
using System.Threading.Tasks;
using drones_api.Entities;

namespace drones_api.Services.Contracts
{
    public interface IDronService
    {
        Task<bool> ExistDron(string dronSerialNumber);
        Task<Dron> GetDronById(string dronSerialNumber);
        Task Register(Dron dron);
        Task SaveChange();
        Task<List<Dron>> GetListDrones();
        Task<List<Dron>> GetListDronesAvailable();
    }
}

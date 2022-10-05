using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using drones_api.Entities;
using drones_api.Enums;
using drones_api.Entities.AppDBContext;
using drones_api.Services.Contracts;

namespace drones_api.Services.Implementations
{
    public class DronService : IDronService
    {
        private readonly DronDBContext dbcontext;

        public DronService(DronDBContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public async Task<Dron> GetDronById(string dronSerialNumber)
        {
            return await dbcontext
                    .Drones
                    .FirstOrDefaultAsync(x => x.SerialNumber.Equals(dronSerialNumber));
        }
        public async Task<bool> ExistDron(string dronSerialNumber)
        {
            return await dbcontext
                    .Drones
                    .AnyAsync(x => x.SerialNumber.Equals(dronSerialNumber));
        }

        public async Task Register(Dron dron)
        {
            dbcontext.Drones.Add(dron);
            await SaveChange();
        }

        public async Task SaveChange()
        {
            await dbcontext.SaveChangesAsync();
        }

        public async Task<List<Dron>> GetListDrones()
        {
            return await dbcontext.Drones.ToListAsync();
        }

        public async  Task<List<Dron>> GetListDronesAvailable()
        {
            return await dbcontext.Drones.Where(x => x.State == DronState.INACTIVO).ToListAsync();
        }
    }
}

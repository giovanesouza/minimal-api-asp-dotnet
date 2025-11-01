
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly DBContext _dBContext;
        public VehicleService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public Vehicle Create(Vehicle vehicle)
        {
            _dBContext.Vehicles.Add(vehicle);
            _dBContext.SaveChanges();
            return vehicle;
        }

        public List<Vehicle> GetAll(int? page = 1, string? name = null, string? brand = null)
        {
            var query = _dBContext.Vehicles.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(v => v.Name.Contains(name));
            }

            int itemsPerPage = 10;

            if (page != null) query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
            return query.ToList();
        }

        public Vehicle? GetById(int id)
        {
            return _dBContext.Vehicles.Where(v => v.Id == id).FirstOrDefault();
        }

        public Vehicle Update(Vehicle vehicle)
        {
            var foundVehicle = GetById(vehicle.Id) ??
            throw new KeyNotFoundException($"No vehicle found with ID {vehicle.Id}.");

            foundVehicle.Name = vehicle.Name;
            foundVehicle.Brand = vehicle.Brand;
            foundVehicle.Year = vehicle.Year;

            _dBContext.SaveChanges();

            return foundVehicle;
        }

        public void Delete(Vehicle vehicle)
        {
            _dBContext.Vehicles.Remove(vehicle);
            _dBContext.SaveChanges();
        }
    }
}
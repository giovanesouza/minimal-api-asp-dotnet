using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;

namespace Test.Mocks
{
    public class VehicleServiceMock : IVehicleService
    {
        private List<Vehicle> _vehicles;

        public VehicleServiceMock()
        {
            _vehicles =
            [
                new Vehicle { Id = 1, Name = "Corolla", Brand = "Toyota", Year = 2020 },
                new Vehicle { Id = 2, Name = "Civic", Brand = "Honda", Year = 2019 }
            ];
        }

        public List<Vehicle> GetAll(int? page = 1, string? name = null, string? brand = null)
        {
            return _vehicles;
        }

        public Vehicle? GetById(int id)
        {
            return _vehicles.FirstOrDefault(v => v.Id == id);
        }

        public Vehicle Create(Vehicle vehicle)
        {
            vehicle.Id = _vehicles.Max(v => v.Id) + 1;
            _vehicles.Add(vehicle);
            return vehicle;
        }

        public Vehicle Update(Vehicle vehicle)
        {
            var index = _vehicles.FindIndex(v => v.Id == vehicle.Id);
            if (index >= 0) _vehicles[index] = vehicle;
            return vehicle;
        }

        public void Delete(Vehicle vehicle)
        {
            _vehicles.Remove(vehicle);
        }
    }
}
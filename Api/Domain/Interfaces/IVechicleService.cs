
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces
{
    public interface IVechicleService
    {
        List<Vehicle> GetAll(int? page = 1, string? name = null, string? brand = null);
        Vehicle? GetById(int id);
        Vehicle Create(Vehicle vehicle);
        Vehicle Update(Vehicle vehicle);
        void Delete(Vehicle vehicle);
    }
}
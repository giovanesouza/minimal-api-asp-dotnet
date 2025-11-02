using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure.Db;
using BCryptNet = BCrypt.Net.BCrypt;

namespace MinimalApi.Domain.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly DBContext _dBContext;
        public AdministratorService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public List<Administrator> GetAll(int? pagina)
        {
            var query = _dBContext.Administrators.AsQueryable();

            int itemsPerPage = 10;

            if(pagina != null) query = query.Skip(((int)pagina - 1) * itemsPerPage).Take(itemsPerPage);
            return query.ToList();
        }

        public Administrator? GetById(int id)
        {
            return _dBContext.Administrators.Where(a => a.Id == id).FirstOrDefault();
        }

        public Administrator Create(Administrator administrator)
        {
            _dBContext.Administrators.Add(administrator);
            _dBContext.SaveChanges();

            return administrator;
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            // var admin = _dBContext.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            var admin = _dBContext.Administrators.Where(a => a.Email == loginDTO.Email).FirstOrDefault();

            if (admin == null) return null;

            bool isPasswordValid = BCryptNet.Verify(loginDTO.Password, admin.Password);
            return isPasswordValid ? admin : null;
        }
    }
}
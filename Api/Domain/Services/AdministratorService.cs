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
            throw new NotImplementedException();
        }

        public Administrator? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Administrator Create(Administrator administrator)
        {
            throw new NotImplementedException();
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
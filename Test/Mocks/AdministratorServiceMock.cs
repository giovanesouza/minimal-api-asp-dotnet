using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;

namespace Test.Mocks
{
    public class AdministratorServiceMock : IAdministratorService
    {
        private static List<Administrator> administrators = [
            new() {
                Id = 1,
                Email = "admin@test.com",
                Password = "123456",
                Profile = "Admin"
            },
            new() {
                Id = 2,
                Email = "editor@test.com",
                Password = "123456",
                Profile = "Editor"
            }
        ];

        public Administrator? GetById(int id)
        {
            return administrators.Find(a => a.Id == id);
        }

        public Administrator Create(Administrator administrador)
        {
            administrador.Id = administrators.Count + 1;
            administrators.Add(administrador);

            return administrador;
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            return administrators.Find(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password);
        }

        public List<Administrator> GetAll(int? pagina)
        {
            return administrators;
        }
    }
}
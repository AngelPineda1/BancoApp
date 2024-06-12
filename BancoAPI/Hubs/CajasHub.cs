using BancoAPI.Models.Dtos;
using BancoAPI.Models.Entities;
using BancoAPI.Models.Validators;
using BancoAPI.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace BancoAPI.Hubs
{
    public class CajasHub:Hub
    {
        private readonly CajasRepository _repository;
        private readonly CajasValidator _validator;
        public CajasHub(CajasRepository cajas,CajasValidator validations)
        {
            _repository = cajas;
            _validator = validations;
        }

        public async Task Post(CajasDto dto)
        {
            if (dto != null)
            {
                var results=_validator.Validate(dto);
                if(results.IsValid)
                {
                    Cajas cajas = new Cajas()
                    {
                        Id = 0,
                        Nombre = dto.Nombre,
                        Username = dto.Username,
                        Contrasena = dto.Contrasena,
                        Activa = true
                    };
                    _repository.Insert(cajas);
                    await Clients.All.SendAsync("CajaAgregada", cajas);
                }
            }
        }

    }
}

using BancoAPI.Models.Dtos;
using FluentValidation;
namespace BancoAPI.Models.Validators
{
    public class CajasValidator2:AbstractValidator<CajasUpDto>
    {
        public CajasValidator2()
        {
            
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("Debe ingresar un Nombre");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Debe ingresar un Nombre de usuario");
        }
    }
}

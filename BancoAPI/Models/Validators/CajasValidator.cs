using BancoAPI.Models.Dtos;
using FluentValidation;
namespace BancoAPI.Models.Validators
{
    public class CajasValidator : AbstractValidator<CajasDto>
    {
        public CajasValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("Debe ingresar un Nombre");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Debe ingresar un Nombre de usuario");
            RuleFor(x => x.Contrasena).NotEmpty().WithMessage("Debe ingresar una contrasena");

        }
    }
}

using FluentValidation;

namespace BancoMexicoWeb.Areas.Admin.Models.Validators
{
    public class CajasValidator:AbstractValidator<AgregarCajaViewModel>
    {
        public CajasValidator()
        {
            
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("Debe ingresar un Nombre");
            RuleFor(x => x.Username).NotEmpty().WithMessage("Debe ingresar un Nombre de usuario");
            RuleFor(x => x.Contrasena).NotEmpty().WithMessage("Debe ingresar una contrasena");
        }
    }
}

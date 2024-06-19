using FluentValidation;

namespace BancoMexicoWeb.Areas.Admin.Models.Validators
{
    public class CajasValidator:AbstractValidator<object>
    {
        public CajasValidator()
        {
            When(x => x is AgregarCajaViewModel, () =>
            {
                RuleFor(x => ((AgregarCajaViewModel)x).Nombre).NotEmpty().WithMessage("Debe ingresar un Nombre");
                RuleFor(x => ((AgregarCajaViewModel)x).Username).NotEmpty().WithMessage("Debe ingresar un Nombre de usuario");
                RuleFor(x => ((AgregarCajaViewModel)x).Contrasena).NotEmpty().WithMessage("Debe ingresar una contrasena");
            });

            When(x => x is ActualizarCajaViewModel, () =>
            {
                RuleFor(x => ((ActualizarCajaViewModel)x).Nombre).NotEmpty().WithMessage("Debe ingresar un Nombre");
                RuleFor(x => ((ActualizarCajaViewModel)x).Username).NotEmpty().WithMessage("Debe ingresar un Nombre de usuario");
               
            });
        }
    }
}

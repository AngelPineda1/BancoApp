using FluentValidation;

namespace BancoMexicoWeb.Areas.Admin.Models.Validators
{
    public class UsuariosValidator:AbstractValidator<object>
    {
        public UsuariosValidator()
        {
            
            When(x => x is AgregarUsuarioViewModel, () =>
                {
                RuleFor(x => ((AgregarUsuarioViewModel)x).Nombre).NotEmpty().WithMessage("Debe ingresar un Nombre");
                RuleFor(x => ((AgregarUsuarioViewModel)x).Username).NotEmpty().WithMessage("Debe ingresar un Nombre de usuario");
                RuleFor(x => ((AgregarUsuarioViewModel)x).Contrasena).NotEmpty().WithMessage("Debe ingresar una contrasena");
            });

            When(x => x is ActualizarUsuarioViewModel, () =>
            {
                RuleFor(x => ((ActualizarUsuarioViewModel)x).Nombre).NotEmpty().WithMessage("Debe ingresar un Nombre");
                RuleFor(x => ((ActualizarUsuarioViewModel)x).Username).NotEmpty().WithMessage("Debe ingresar un Nombre de usuario");

            });
        }
    }
}

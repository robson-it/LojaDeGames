using FluentValidation;
using LojaDeGames.Model;

namespace LojaDeGames.Validator
{
    public class ProdutoValidator : AbstractValidator<Produto>
    {
        public ProdutoValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(250);

            RuleFor(p => p.Descricao)
                .MaximumLength(1000);

            RuleFor(p => p.Console)
               .NotEmpty()
               .MinimumLength(5)
               .MaximumLength(250);

            RuleFor(p => p.Preco)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

            RuleFor(p => p.Foto)
               .MaximumLength(250);



        }
    }
}

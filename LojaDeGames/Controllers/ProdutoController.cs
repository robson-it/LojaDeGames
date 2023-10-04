using FluentValidation;
using LojaDeGames.Model;
using LojaDeGames.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaDeGames.Controllers
{

    [Authorize]
    [ApiController]
    [Route("~/produtos")]
    public class ProdutoController : ControllerBase
    {

        private readonly IProdutoService _produtoService;
        private readonly IValidator<Produto> _produtoValidator;

        public ProdutoController(IProdutoService produtoService, IValidator<Produto> produtoValidator)
        {
            _produtoService = produtoService;
            _produtoValidator = produtoValidator;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _produtoService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(long id)
        {
            var Resposta = await _produtoService.GetById(id);
            if (Resposta is not null)
            {
                return Ok(Resposta);
            }
            else
            {
                return NotFound();
            }

        }


        [HttpGet("nomeOuConsole/{nomeOuConsole}")]
        public async Task<ActionResult> GetByNomeOuConsole(string nomeOuConsole)
        {
            return Ok(await _produtoService.GetByNomeOuConsole(nomeOuConsole));
        }

        [HttpGet("intervaloDePreco")]
        public async Task<ActionResult> GetByIntervaloDePreco(decimal precoInicial, decimal precoFinal)
        {
            return Ok(await _produtoService.GetByIntervaloDePreco(precoInicial, precoFinal));
        }

        [HttpPost("cadastrar")]
        public async Task<ActionResult> Create([FromBody] Produto produto)
        {
            var ValidarProduto = await _produtoValidator.ValidateAsync(produto);
            if (!ValidarProduto.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ValidarProduto);
            }
            var Resposta = await _produtoService.Create(produto);

            if (Resposta is null)
            {
                return BadRequest("Categoria não encontrada!");
            }

            return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
        }

        [HttpPost("cadastrarLista")]
        public async Task<ActionResult> CreateList([FromBody] ICollection<Produto> produtos)
        {
            var itens = "";
            foreach (var produto in produtos) {
                var ValidarProduto = await _produtoValidator.ValidateAsync(produto);
                if (!ValidarProduto.IsValid)
                {
                    //return StatusCode(StatusCodes.Status400BadRequest, ValidarProduto);
                    itens += "\n O Produto: " + produto.Nome + " não é válido! Erro: " + ValidarProduto;
                    continue;
                }
                var Resposta = await _produtoService.Create(produto);

                if (Resposta is null)
                {
                    //return BadRequest("Categoria não encontrada!");
                    itens += "\n A categoria do Produto: " + produto.Nome + " não foi encontrada! Erro: " + ValidarProduto;
                    continue;

                }

                //return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
                itens += "\n O Produto: " + produto.Nome + " foi cadastrado com sucesso!\n" 
                    + "[INFO] => \n "
                    + "Id:" + produto.Id + "\n"
                    + "Nome:" + produto.Nome + "\n"
                    + "Descrição:" + produto.Descricao + "\n"
                    + "Plataformas Disponíveis:" + produto.Console + "\n"
                    + "Preço:" + produto.Preco + "\n"
                    + "Imagem de Capa:" + produto.Foto + "\n"
                    + "Categoria:" + produto.Categoria.Tipo + "\n"
                    + "Usuário que Cadastrou:" + produto.Usuario.Nome + "\n";
            }
            return  Ok(itens);
        }


        [HttpPut]
        public async Task<ActionResult> Update([FromBody] Produto produto)
        {
            if (produto.Id == 0)
            {
                return BadRequest("O Id do produto é inválido");
            }

            var ValidarProduto = await _produtoValidator.ValidateAsync(produto);
            if (!ValidarProduto.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ValidarProduto);
            }

            var Resposta = await _produtoService.Update(produto);

            if (Resposta is null)
            {
                return NotFound("Produto e/ou categoria não encontrado(s)!");
            }

            return Ok(Resposta);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {

            var BuscaProduto = await _produtoService.GetById(id);
            if (BuscaProduto is null)
            {
                return NotFound("O Produto não foi encontrado!");
            }

            await _produtoService.Delete(BuscaProduto);
            return NoContent();

        }

    }
}

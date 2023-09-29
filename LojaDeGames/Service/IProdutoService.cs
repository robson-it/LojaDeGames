using LojaDeGames.Model;

namespace LojaDeGames.Service
{
    public interface IProdutoService
    {
        Task<IEnumerable<Produto>> GetAll();
        Task<Produto?> GetById(long id);
        Task<IEnumerable<Produto>> GetByNomeOuConsole(string nomeOuConsole);
        Task<IEnumerable<Produto>> GetByIntervaloDePreco(decimal precoInicial, decimal precoFinal);
        Task<Produto?> Create(Produto produto);
        Task<Produto?> Update(Produto produto);
        Task Delete(Produto produto);
    }
}

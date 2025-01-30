using NSE.WebApp.MVC.Models;

namespace NSE.WebApp.MVC.Service
{
    public interface ICatalogoService
    {
        Task<IEnumerable<ProdutoViewModel>> ObterTodos();
        Task<ProdutoViewModel> ObterPorId(Guid id);
    }
}

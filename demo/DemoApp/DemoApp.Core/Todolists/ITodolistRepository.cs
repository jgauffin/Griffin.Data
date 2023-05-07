using DemoApp.Core;

namespace DemoApp.Core.Todolists
{
    public interface ITodolistRepository
    {
        Task<Todolist> GetById(int id);

        Task Create(Todolist entity);

        Task Update(Todolist entity);

        Task Delete(Todolist entity);

    }
}

using Griffin.Data.Domain;

namespace DemoApp.Domain.Todolists
{
    public interface ITodolistRepository : ICrudOperations<Todolist>
    {
        Task<Todolist> GetById(int id);
    }
}

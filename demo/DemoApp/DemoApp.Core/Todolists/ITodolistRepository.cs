namespace DemoApp.Core.Todolists;

public interface ITodolistRepository
{
    Task Create(Todolist entity);

    Task Delete(Todolist entity);
    Task<Todolist> GetById(int id);

    Task Update(Todolist entity);
}

namespace DemoApp.Core.TodoTasks;

public interface ITodoTaskRepository
{
    Task Create(TodoTask entity);

    Task Delete(TodoTask entity);
    Task<TodoTask> GetById(int id);

    Task Update(TodoTask entity);
}

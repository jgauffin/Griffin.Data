using DemoApp.Core;

namespace DemoApp.Core.TodoTasks
{
    public interface ITodoTaskRepository
    {
        Task<TodoTask> GetById(int id);

        Task Create(TodoTask entity);

        Task Update(TodoTask entity);

        Task Delete(TodoTask entity);

    }
}

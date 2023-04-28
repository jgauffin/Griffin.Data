using Griffin.Data.Domain;

namespace DemoApp.Domain.TodoTasks
{
    public interface ITodoTaskRepository : ICrudOperations<TodoTask>
    {
        Task<TodoTask> GetById(int id);
    }
}

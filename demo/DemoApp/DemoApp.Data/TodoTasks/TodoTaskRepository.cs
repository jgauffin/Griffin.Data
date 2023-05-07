using Griffin.Data;
using Griffin.Data.Mapper;
using Griffin.Data.Domain;
using DemoApp.Core.TodoTasks;

namespace DemoApp.Data.TodoTasks
{
    public class TodoTaskRepository : CrudOperations<TodoTask>, ITodoTaskRepository
    {
        public TodoTaskRepository(Session session) : base(session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
        }
        public async Task<TodoTask> GetById(int id)
        {
            return await Session.First<TodoTask>(new {id});
        }
    }
}

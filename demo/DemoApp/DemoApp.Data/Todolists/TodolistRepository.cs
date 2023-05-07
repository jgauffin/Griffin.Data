using Griffin.Data;
using Griffin.Data.Mapper;
using Griffin.Data.Domain;
using DemoApp.Core.Todolists;

namespace DemoApp.Data.Todolists
{
    public class TodolistRepository : CrudOperations<Todolist>, ITodolistRepository
    {
        public TodolistRepository(Session session) : base(session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
        }
        public async Task<Todolist> GetById(int id)
        {
            return await Session.First<Todolist>(new {id});
        }
    }
}

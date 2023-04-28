using DemoApp.Core.TodoTasks;
using Griffin.Data;
using Griffin.Data.Domain;
using Griffin.Data.Mapper;

namespace DemoApp.Data.TodoTasks;

public class DocumentReviewRepository : CrudOperations<DocumentReview>, IDocumentReviewRepository
{
    public DocumentReviewRepository(Session session) : base(session)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }
    }

    public async Task<DocumentReview> GetById(int taskId)
    {
        return await Session.First<DocumentReview>(new { taskId });
    }
}

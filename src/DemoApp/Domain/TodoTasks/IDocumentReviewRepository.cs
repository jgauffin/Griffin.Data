using Griffin.Data.Domain;

namespace DemoApp.Domain.TodoTasks
{
    public interface IDocumentReviewRepository : ICrudOperations<DocumentReview>
    {
        Task<DocumentReview> GetById(int taskId);
    }
}

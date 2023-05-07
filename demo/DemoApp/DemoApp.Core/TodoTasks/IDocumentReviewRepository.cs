using DemoApp.Core;

namespace DemoApp.Core.TodoTasks
{
    public interface IDocumentReviewRepository
    {
        Task<DocumentReview> GetById(int taskId);

        Task Create(DocumentReview entity);

        Task Update(DocumentReview entity);

        Task Delete(DocumentReview entity);

    }
}

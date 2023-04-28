namespace DemoApp.Core.TodoTasks;

public interface IDocumentReviewRepository
{
    Task Create(DocumentReview entity);

    Task Delete(DocumentReview entity);
    Task<DocumentReview> GetById(int taskId);

    Task Update(DocumentReview entity);
}

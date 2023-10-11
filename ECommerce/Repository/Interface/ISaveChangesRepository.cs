namespace ECommerce.Repository.Interface
{
    public interface ISaveChangesRepository
    {
        Task AsynchronousSaveChanges();
    }
}

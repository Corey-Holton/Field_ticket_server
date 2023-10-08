using CA.Ticketing.Persistance.Models.Abstracts;

namespace CA.Ticketing.Business.Services.Removal
{
    public interface IRemovalService
    {
        void Remove<T>(T entity) where T : IdentityModel;
    }
}

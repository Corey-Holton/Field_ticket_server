using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models.Abstracts;

namespace CA.Ticketing.Business.Extensions
{
    public static class DbExtensions
    {
        public static void DeleteRelated<T>(this ICollection<T> collection, CATicketingContext context) where T : IdentityModel =>
            collection.ToList()
                .ForEach(x => context.Entry(x).State = Microsoft.EntityFrameworkCore.EntityState.Deleted);
    }
}

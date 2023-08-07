using CA.Ticketing.Business.Services.Charges.Dto;

namespace CA.Ticketing.Business.Services.Charges
{
    public interface IChargesService
    {
        Task<IEnumerable<ChargeDto>> GetAll();

        Task Update(ChargeDto entity);
    }
}

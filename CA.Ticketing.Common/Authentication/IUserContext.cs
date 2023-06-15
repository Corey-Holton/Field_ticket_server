namespace CA.Ticketing.Common.Authentication
{
    public interface IUserContext
    {
        IContextUser? User { get; }
    }
}

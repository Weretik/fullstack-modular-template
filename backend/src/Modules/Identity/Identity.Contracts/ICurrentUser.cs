namespace Identity.Contracts;

/// <summary>The signed-in user of the current request, for modules that record who did something.</summary>
public interface ICurrentUser
{
    Guid? UserId { get; }

    bool IsAuthenticated { get; }
}

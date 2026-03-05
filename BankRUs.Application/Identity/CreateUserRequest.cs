namespace BankRUs.Application.Identity;

public sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string SocialSecurityNumber,
    string Email
);

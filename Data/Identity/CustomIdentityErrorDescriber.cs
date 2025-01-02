using Microsoft.AspNetCore.Identity;

namespace OnlineBalance.Data.Identity
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            var error = base.DuplicateUserName(userName);
            error.Description = "This email is not allowed.";
            return error;
        }
    }
}

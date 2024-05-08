using Microsoft.AspNetCore.Identity;

namespace TrashTracker.Web.Utils
{
    public class TrashTrackerIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError ConcurrencyFailure()
        {
            return base.ConcurrencyFailure();
        }

        public override IdentityError DefaultError()
        {
            return base.DefaultError();
        }

        public override IdentityError DuplicateEmail(String email)
        {
            return new IdentityError()
            {
                Code = nameof(DuplicateEmail),
                Description = "Ez az e-mail cím már foglalt!"
            };
        }

        public override IdentityError DuplicateRoleName(String role)
        {
            return base.DuplicateRoleName(role);
        }

        public override IdentityError DuplicateUserName(String userName)
        {
            return new IdentityError()
            {
                Code = nameof(DuplicateUserName),
                Description = "Ez a felhasználónév már foglalt!"
            };
        }

        public override IdentityError InvalidEmail(String? email)
        {
            return new IdentityError()
            {
                Code = nameof(InvalidEmail),
                Description = "Érvénytelen e-mail cím!"
            };
        }

        public override IdentityError InvalidRoleName(String? role)
        {
            return base.InvalidRoleName(role);
        }

        public override IdentityError InvalidToken()
        {
            return base.InvalidToken();
        }

        public override IdentityError InvalidUserName(String? userName)
        {
            return new IdentityError()
            {
                Code = nameof(InvalidUserName),
                Description = "Érvénytelen felhasználónév!"
            };
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return base.LoginAlreadyAssociated();
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordMismatch),
                Description = "Sikertelen bejelentkezés!"
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "Jelszavának tartalmaznia kell legalább egy számjegyet!"
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresLower),
                Description = "Jelszavának tartalmaznia kell legalább egy kisbetűt!"
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "Jelszavának tartalmaznia kell" +
                    "legalább egy nem alfanumerikus karaktert!"
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(Int32 uniqueChars)
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = "Jelszavának tartalmaznia kell" +
                    $"legalább {uniqueChars} egyedi karaktert!"
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "Jelszavának tartalmaznia kell legalább 1 nagybetűt!"
            };
        }

        public override IdentityError PasswordTooShort(Int32 length)
        {
            return new IdentityError()
            {
                Code = nameof(PasswordTooShort),
                Description = "Jelszava túl rövid!"
            };
        }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return base.RecoveryCodeRedemptionFailed();
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return base.UserAlreadyHasPassword();
        }

        public override IdentityError UserAlreadyInRole(String role)
        {
            return new IdentityError()
            {
                Code = nameof(UserAlreadyInRole),
                Description = $"A felhasználó már szerepel a {role} szerepkörben!"
            };
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return base.UserLockoutNotEnabled();
        }

        public override IdentityError UserNotInRole(String role)
        {
            return new IdentityError()
            {
                Code = nameof(UserNotInRole),
                Description = $"A felhasználó nem szerepel ebben a {role} szerepkörben!"
            };
        }
    }
}

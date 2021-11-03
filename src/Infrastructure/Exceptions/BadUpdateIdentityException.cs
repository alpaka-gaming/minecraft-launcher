using System;

namespace Infrastructure.Exceptions
{
    public class BadUpdateIdentityException: ArgumentException{
        public BadUpdateIdentityException() : base("Bad updateIdentity") { }
    }

}
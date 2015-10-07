using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailPhase
{
    public class User: Model<User>
    {
        [Key]
        public string Username { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public Session GetSession()
        {
            var existingSession = Session.Objects.SingleOrDefault(s => s.User != null && s.User.Username == Username);

            if (existingSession != null)
                return existingSession;
            else
            {
                var newSession = new Session
                {
                    User = this
                };
                Session.Objects.Add(newSession);

                return newSession;
            }
        }

        public void SetPassword(string plainTextPassword)
        {
            PasswordHash = Crypto.HashPassword(plainTextPassword);
        }

        public bool CheckPassword(string plainTextPassword)
        {
            var givenHash = Crypto.HashPassword(plainTextPassword);
            return givenHash == PasswordHash;
        }

        public static User Login(string usernameOrMail, string plaintextPassword)
        {
            var user = Objects.FirstOrDefault(u => u.Username == usernameOrMail || u.Email == usernameOrMail);

            if (user.CheckPassword(plaintextPassword))
                return user;
            else
                return null;
        }
    }
}

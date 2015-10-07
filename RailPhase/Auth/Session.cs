using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace RailPhase
{
    public class Session: Model<Session>
    {
        public Session(User user)
        {
            User = user;
            LastSeen = DateTime.Now;
            Token = Crypto.RandomHex(16);
        }

        public Session():
            this(null)
        {

        }

        [Key]
        public string Token { get; set; }
        public DateTime LastSeen { get; set; }
        public virtual User User { get; set; }

        public static TimeSpan SessionExpirationTime = TimeSpan.FromDays(10);

        public Cookie MakeCookie()
        {
            return new Cookie
            {
                Name = "Session_Token",
                Value = Token,
                Expires = DateTime.Now + SessionExpirationTime
            };
        }

        public static Session FromRequest(HttpRequest request)
        {
            if (request.Cookies.ContainsKey("Session_Token"))
            {
                var token = request.Cookies["Session_Token"];
                var session = Objects.FirstOrDefault(i => i.Token == token);
                return session;
            }
            else
                return null;
        }
    }
}

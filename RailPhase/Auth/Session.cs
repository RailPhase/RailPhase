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
        
        public static Session FromContext(Context context)
        {
            var cookie = context.Request.Cookies["Session_Token"];
            if (cookie != null)
            {
                var token = cookie.Value;
                var session = Objects.FirstOrDefault(i => i.Token == token);
                return session;
            }
            else
                return null;
        }
    }
}

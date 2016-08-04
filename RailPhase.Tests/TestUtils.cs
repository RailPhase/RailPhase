using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RailPhase.Tests
{
    public class TestUtils
    {
        static Random rand = new Random();
        public static string GenerateRandomString(int length)
        {
            var s = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                char c = (char)rand.Next(65536);
                s.Append(c.ToString());
            }

            return s.ToString();
        }

        static char[] urlCharacters = new char[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '-', '.', '+', '/'
        };

        public static string GenerateRandomURL(int length)
        {
            var s = new StringBuilder();

            char lastChar = '\0';

            for (int i = 0; i < length; i++)
            {
                char c = urlCharacters[rand.Next(urlCharacters.Length)];

                // Prevent two consecutive slashes, don't start with a slash
                if (c != '/' || lastChar != '/' && (c != '/' || i > 0))
                {
                    s.Append(c.ToString());
                    lastChar = c;
                }
            }

            return s.ToString();
        }

        public const string AppPrefix = "http://localhost:21808/";

        public static void AppTest(App app, Action innerAction)
        {
            var appThread = new Thread(() => {
                app.RunHttpServer(AppPrefix);
            });

            try
            {
                appThread.Start();
                innerAction();
            }
            finally
            {
                app.Stop();
                appThread.Join();
            }
        }
    }
}

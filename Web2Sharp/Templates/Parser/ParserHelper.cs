using System;
using System.Collections.Generic;
using System.Text;
using QUT.Gppg;

namespace Web2Sharp.Templates.Parser
{
    internal partial class Parser
    {
        public string ResultText = null;
        public Dictionary<string, string> ResultBlocks = new Dictionary<string, string>();
        public List<string> ResultUsings = new List<string>();
        public string ResultContextType = null;
        public string ResultExtends = null;

        static string EscapeText(string text)
        {
            //TODO: Do this in a more performant way
            if (text == null)
                return null;

            text = text.Replace("\\", "\\\\");
            text = text.Replace("\"", "\\\"");
            text = text.Replace("\n", "\\n");
            text = text.Replace("\r", "\\r");

            return text;
        }

        /*
         * Copied from GPPG documentation.
         */

        Parser(Lexer s) : base(s) { }

        public static Parser FromText(string text)
        {
            return new Parser(new Lexer(new System.IO.StringReader(text)));
        }

        class Lexer : QUT.Gppg.AbstractScanner<string, LexLocation>
        {
            private System.IO.TextReader reader;

            public Lexer(System.IO.TextReader reader)
            {
                this.reader = reader;
            }

            bool inTag = false;

            public override int yylex()
            {
                char ch;
                int ord = reader.Read();
                //
                // Must check for EOF
                //
                if (ord == -1)
                    return (int)Tokens.EOF;
                else
                    ch = (char)ord;

                if (ch == '{' && reader.Peek() == '%')
                {
                    reader.Read();
                    inTag = true;

                    // Consume Whitespace
                    ch = (char)reader.Read();
                    while(char.IsWhiteSpace(ch))
                    {
                        ch = (char)reader.Read();
                    }

                    StringBuilder text = new StringBuilder();
                    text.Append(ch);

                    while (char.IsLetter((char)reader.Peek()))
                        text.Append((char)reader.Read());

                    switch(text.ToString())
                    {
                        case "block":
                            return (int)Tokens.TAG_START_BLOCK;
                        case "endblock":
                            return (int)Tokens.TAG_START_ENDBLOCK;
                        case "if":
                            return (int)Tokens.TAG_START_IF;
                        case "endif":
                            return (int)Tokens.TAG_START_ENDIF;
                        case "else":
                            return (int)Tokens.TAG_START_ELSE;
                        case "for":
                            return (int)Tokens.TAG_START_FOR;
                        case "endfor":
                            return (int)Tokens.TAG_START_ENDFOR;
                        case "using":
                            return (int)Tokens.TAG_START_USING;
                        case "context":
                            return (int)Tokens.TAG_START_CONTEXT;
                        case "extends":
                            return (int)Tokens.TAG_START_EXTENDS;
                        default:
                            return (int)Tokens.error;
                    }
                }
                else if (ch == '%' && reader.Peek() == '}')
                {
                    reader.Read();
                    inTag = false;
                    return (int)Tokens.TAG_END;
                }
                else if (ch == '{' && reader.Peek() == '{')
                {
                    reader.Read();
                    inTag = true;
                    return (int)Tokens.VALUE_START;
                }
                else if (ch == '}' && reader.Peek() == '}')
                {
                    reader.Read();
                    inTag = false;
                    return (int)Tokens.VALUE_END;
                }
                else if (!inTag)
                {
                    StringBuilder text = new StringBuilder();
                    text.Append(ch);

                    while (reader.Peek() >= 0 && reader.Peek() != '{')
                    {
                        ch = (char)reader.Read();
                        text.Append(ch);
                    }

                    yylval = text.ToString();
                    return (int)Tokens.TEXT;
                }
                else if (char.IsWhiteSpace(ch) && inTag)
                {
                    return yylex();
                }
                else if (char.IsLetter(ch) && inTag)
                {
                    char peek;
                    StringBuilder text = new StringBuilder();
                    text.Append(ch);

                    while (char.IsLetter(peek = (char)reader.Peek()))
                        text.Append((char)reader.Read());

                    switch (text.ToString())
                    {
                        case "in":
                            return (int)Tokens.KEY_IN;
                        default:
                            yylval = text.ToString();
                            return (int)Tokens.TEXT;
                    }
                }
                else
                {
                    yylval = ch.ToString();
                    return (int)Tokens.TEXT;
                }
            }

            public override void yyerror(string format, params object[] args)
            {
                Console.Error.WriteLine(format, args);
            }
        }
    }
}

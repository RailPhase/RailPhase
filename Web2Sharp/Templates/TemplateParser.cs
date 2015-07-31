using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web2Sharp.Templates
{
    public static class TemplateParser
    {
        public static string CompileToCSharp(string template, string name)
        {
            var s = new StringBuilder();
            s.AppendLine("// Automagically generated code by Web2Sharp.Templates.TemplateParser");
            s.AppendLine("using System.Text;");
            s.AppendLine("namespace Web2Sharp.TemplateCache {");
            s.AppendLine("public class " + name + ": ITemplate {");
            s.AppendLine("public string Render(IDictionary<string, object> context) {");
            s.AppendLine("StringBuilder output = new StringBuilder();");

            CompileFragment(template, s);

            s.AppendLine("return output.ToString();");
            s.AppendLine("} // Build");
            s.AppendLine("} // class");
            s.AppendLine("} // namespace");

            return s.ToString();
        }

        static void CompileFragment(string fragment, StringBuilder s)
        {
            int textStart = 0;
            int tagStart = fragment.IndexOf("{{");
            while (tagStart >= 0)
            {
                tagStart += 2;
                int tagEnd = fragment.IndexOf("}}", textStart);

                string text = fragment.Substring(textStart, tagStart-textStart);

                if (text.Length > 0)
                {
                    s.Append("output.Append(\"");
                    s.Append(EscapeText(text));
                    s.Append("\");\n");
                }

                string tag = fragment.Substring(tagStart, tagEnd-tagStart);
                tag = tag.Trim();
                if(tag.Length > 0)
                {
                    s.AppendLine("output.Append(context[\""+EscapeText(tag)+"\"].ToString());");
                }

                textStart = tagEnd + 2;
                tagStart = fragment.IndexOf("{{", textStart);
            }

            string remainingText = fragment.Substring(textStart);
            if (remainingText.Length > 0)
            {
                s.Append("output.Append(\"");
                s.Append(EscapeText(remainingText));
                s.Append("\");\n");
            }
        }

        static string EscapeText(string text)
        {
            //TODO: Actually escape the text
            return text;
        }
    }
}

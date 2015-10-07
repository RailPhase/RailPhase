using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailPhase
{
    public class FormField
    {
        public string Name;
        public Type Type;
        public virtual bool CanWrite { get { return false; } }
        public Action<object, object> Set;
        public Func<object, object> Get;

        public virtual void SetFromString(object form, string value)
        {
            throw new InvalidOperationException("Can not set FormField value on FormField base class.");
        }

        public virtual string Render(object value)
        {
            return "<span class=\"form-field-readonly\">" + value.ToString() + "</span>";
        }
    }

    public class TextField: FormField
    {
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override void SetFromString(object form, string value)
        {
            Set(form, value);
        }

        public override string Render(object value)
        {
            string s = (string)value;
            if (s == null)
                s = "";

            return "<input type=\"text\" name=\"" + Name + "\" id=\"" + Name + "\" value=\"" + s + "\">";
        }

        public static TextField Factory(FormField prototype)
        {
            return new TextField
            {
                Name = prototype.Name,
                Type = prototype.Type,
                Get = prototype.Get,
                Set = prototype.Set
            };
        }

    }
}

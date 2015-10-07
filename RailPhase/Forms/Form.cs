using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace RailPhase
{
    public class Form
    {
        public virtual Dictionary<string, FormField> FormFields { get; protected set; }

        public string RenderField(string fieldName)
        {
            var field = FormFields[fieldName];
            return field.Render(field.Get(this));
        }

        public delegate FormField FormFieldFactory(FormField prototype);
        public static Dictionary<Type, FormFieldFactory> FormFieldFactories = new Dictionary<Type, FormFieldFactory>();

        public static FormField FieldFromMember(MemberInfo member)
        {
            var name = member.Name;
            Type type = null;
            Action<object, object> setter;
            Func<object, object> getter;

            // Handle the different member types
            if (member.GetType() == typeof(PropertyInfo) || member.GetType().IsSubclassOf(typeof(PropertyInfo)))
            {
                var property = (PropertyInfo)member;
                type = property.PropertyType;
                getter = (o) => property.GetValue(o);
                setter = (o, v) => property.SetValue(o, v);
            }
            else if (member.GetType() == typeof(FieldInfo) || member.GetType().IsSubclassOf(typeof(FieldInfo)))
            {
                var field = (FieldInfo)member;
                type = field.FieldType;
                getter = (o) => field.GetValue(o);
                setter = (o, v) => field.SetValue(o, v);
            }
            else
            {
                //TODO: Readonly-fields for methods
                return null;
            }

            var fieldPrototype = new FormField
            {
                Name = name,
                Type = type,
                Get = getter,
                Set = setter
            };

            // Make a FormField according to the field type
            if (FormFieldFactories.ContainsKey(type))
                return FormFieldFactories[type](fieldPrototype);
            else
                return fieldPrototype;
        }

        static Form()
        {
            // Register the builtin FormField types
            FormFieldFactories.Add(typeof(string), TextField.Factory);
        }
    }

    public class Form<T>: Form
        where T : Form<T>, new()
    {
        public virtual bool Validate()
        {
            return true;
        }

        public static T FromRequest(HttpRequest request)
        {
            var form = new T();

            // Fill in the form fields from the request POST parameters 
            foreach(var requestParam in request.POST)
            {
                if(FormFieldsStatic.ContainsKey(requestParam.Key))
                {
                    var field = FormFieldsStatic[requestParam.Key];
                    field.Set(form, requestParam.Value);
                }
            }

            return form;
        }

        public static Type FormType;
        public static Dictionary<string, FormField> FormFieldsStatic;

        public override Dictionary<string, FormField> FormFields
        {
            get
            {
                return FormFieldsStatic;
            }

            protected set
            {
                FormFieldsStatic = value;
            }
        }

        static Form()
        {
            FormType = typeof(T);

            FormFieldsStatic = new Dictionary<string, FormField>();

            // Initialize the properties dictionary from all accessible properties and fields in the form type
            var props = FormType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
            foreach (var property in props)
            {
                var formField = FieldFromMember(property);
                if(formField != null)
                    FormFieldsStatic.Add(property.Name, formField);
            }

            var fields = FormType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                var formField = FieldFromMember(field);
                if (formField != null)
                    FormFieldsStatic.Add(field.Name, formField);
            }
        }

    }
}

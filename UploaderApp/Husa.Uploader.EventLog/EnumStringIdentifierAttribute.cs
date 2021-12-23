using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Husa.Cargador.EventLog
{
    /// <summary>
    /// Provides a way to assign string codes to Enumerable types
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnumStringIdentifierAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of EnumStringIdentifierAttribute class.
        /// </summary>
        /// <param name="code">String code assigned to the Enumerable field</param>
        public EnumStringIdentifierAttribute(string code)
        {
            this._code = code;
            this._name = code;
        }

        /// <summary>
        /// Initializes a new instance of EnumStringIdentifierAttribute class.
        /// </summary>
        /// <param name="code">String code assigned to the Enumerable field</param>
        /// <param name="name">Name representing the Enumerable field. If not assigned the code will also hold this property</param>
        public EnumStringIdentifierAttribute(string code, string name)
        {
            this._code = code;
            this._name = name;
        }

        string _code;

        /// <summary>
        /// Gets or sets the code for the Enumerable field
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        string _name;

        /// <summary>
        /// Gets or sets the name for the Enumerable field
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Returns the enumerable instance represented by the code
        /// </summary>
        /// <param name="code">Code for the enumerable field</param>
        /// <param name="enumType">Enumerable Type to get the field from</param>
        /// <returns></returns>
        public static Enum GetEnumValueFromCode(string code, Type enumType)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            if (enumType.IsEnum)
            {
                var fwa = enumType.GetFields().Where(f => f.GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false).Count() > 0);
                var fwc = fwa.Where(f => ((EnumStringIdentifierAttribute)f.GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false).First()).Code == code);
                if (fwc.Count() > 0)
                    return (Enum)fwc.First().GetValue(null);
                else
                    return null;
            }
            else
                throw new Exception("enumType parameter is not an Enum Type.");
        }

        public static TEnum GetEnumValueFromCode<TEnum>(string code)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            var enumType = typeof(TEnum);
            if (enumType.IsEnum)
            {
                var fwa = enumType.GetFields().Where(f => f.GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false).Count() > 0);
                var fwc = fwa.Where(f => ((EnumStringIdentifierAttribute)f.GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false).First()).Code == code);
                if (fwc.Count() > 0)
                    return (TEnum)fwc.First().GetValue(null);
                else
                    throw new Exception("One or more members of '" + enumType.Name + " do not implement the 'EnumStringIdentifierAttribute'");
            }
            else
                throw new Exception("TEnum parameter is not an Enum Type.");
        }

        /// <summary>
        /// Returns the code for an Enumerable object where the EnumStringIdentifierAttribute has been assigned.
        /// </summary>
        /// <param name="enumObject">Enum to get the code from</param>
        /// <returns></returns>
        public static string GetCodeFromEnum(Enum enumObject)
        {
            if (enumObject == null)
                throw new ArgumentNullException("enumObject");

            Type enumType = enumObject.GetType();
            if (enumType.IsEnum)
            {
                var field = enumType.GetFields().Where(f => f.Name == enumObject.ToString());
                if (field.Count() > 0)
                {
                    var fea = field.First().GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false);
                    if (fea.Count() > 0)
                        return ((EnumStringIdentifierAttribute)fea.First()).Code;
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                throw new ArgumentException("enumObject argument is not an Enum Type.");
        }

        /// <summary>
        /// Returns the code for an Enumerable object where the EnumStringIdentifierAttribute has been assigned.
        /// </summary>
        /// <param name="enumObject">Enum to get the name from</param>
        /// <returns></returns>
        [Obsolete]
        public static string GetNameFromEnum(Enum enumObject)
        {
            if (enumObject == null)
                throw new ArgumentNullException("enumObject");

            Type enumType = enumObject.GetType();
            if (enumType.IsEnum)
            {
                var field = enumType.GetFields().Where(f => f.Name == enumObject.ToString());
                if (field.Count() > 0)
                {
                    var fea = field.First().GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false);
                    if (fea.Count() > 0)
                        return ((EnumStringIdentifierAttribute)fea.First()).Name;
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                throw new ArgumentException("enumObject argument is not an Enum Type.");
        }
    }

    public static class EnumString
    {
        /// <summary>
        /// Returns the enumerable instance represented by the code
        /// </summary>
        /// <param name="code">Code for the enumerable field</param>
        /// <param name="enumType">Enumerable Type to get the field from</param>
        /// <returns></returns>
        public static Enum GetEnumValueFromCode(string code, Type enumType)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            if (enumType.IsEnum)
            {
                var fwa = enumType.GetFields().Where(f => f.GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false).Count() > 0);
                var fwc = fwa.Where(f => ((EnumStringIdentifierAttribute)f.GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false).First()).Code == code);
                if (fwc.Count() > 0)
                    return (Enum)fwc.First().GetValue(null);
                else
                    return null;
            }
            else
                throw new Exception("enumType parameter is not an Enum Type.");
        }

        public static TEnum GetEnumValueFromCode<TEnum>(string code)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            var enumType = typeof(TEnum);
            if (enumType.IsEnum)
            {
                var fwa = enumType.GetFields().Where(f => f.GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false).Count() > 0);
                var fwc = fwa.Where(f => ((EnumStringIdentifierAttribute)f.GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false).First()).Code == code);
                if (fwc.Count() > 0)
                    return (TEnum)fwc.First().GetValue(null);
                else
                    throw new Exception("One or more members of '" + enumType.Name + " do not implement the 'EnumStringIdentifierAttribute'");
            }
            else
                throw new Exception("TEnum parameter is not an Enum Type.");
        }

        /// <summary>
        /// Returns the code for an Enumerable object where the EnumStringIdentifierAttribute has been assigned.
        /// </summary>
        /// <param name="enumObject">Enum to get the code from</param>
        /// <returns></returns>
        public static string GetCodeFromEnum(Enum enumObject)
        {
            if (enumObject == null)
                throw new ArgumentNullException("enumObject");

            Type enumType = enumObject.GetType();
            if (enumType.IsEnum)
            {
                var field = enumType.GetFields().Where(f => f.Name == enumObject.ToString());
                if (field.Count() > 0)
                {
                    var fea = field.First().GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false);
                    if (fea.Count() > 0)
                        return ((EnumStringIdentifierAttribute)fea.First()).Code;
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                throw new ArgumentException("enumObject argument is not an Enum Type.");
        }

        /// <summary>
        /// Returns the code for an Enumerable object where the EnumStringIdentifierAttribute has been assigned.
        /// </summary>
        /// <param name="enumObject">Enum to get the name from</param>
        /// <returns></returns>
        public static string GetNameFromEnum(Enum enumObject)
        {
            if (enumObject == null)
                throw new ArgumentNullException("enumObject");

            Type enumType = enumObject.GetType();
            if (enumType.IsEnum)
            {
                var field = enumType.GetFields().Where(f => f.Name == enumObject.ToString());
                if (field.Count() > 0)
                {
                    var fea = field.First().GetCustomAttributes(typeof(EnumStringIdentifierAttribute), false);
                    if (fea.Count() > 0)
                        return ((EnumStringIdentifierAttribute)fea.First()).Name;
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                throw new ArgumentException("enumObject argument is not an Enum Type.");
        }

        public static IEnumerable<KeyValuePair<int, string>> GetEnumNames(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("Invalid Type passed.", "enumType");

            var fields = enumType.GetFields();
            var l = new List<KeyValuePair<int, string>>();
            foreach (var f in fields)
            {
                var attr = f.GetCustomAttributes(false).OfType<EnumStringIdentifierAttribute>().SingleOrDefault();
                if(attr == null)
                    continue;

                l.Add(new KeyValuePair<int, string>((int)Enum.Parse(enumType, f.Name), attr.Name));
            }

            return l.OrderBy(x => x.Value);
        }
    }
}

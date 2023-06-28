using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Husa.Uploader.Extensions
{
    //// TODO: Remove class when App is updated to .NET 7.0
    //// and use Husa.Extensions.Common.EnumExtensions
    public static class EnumExtensions
    {
        public static string ToStringFromEnumMember<T>(this T type, bool isOptional = false)
            where T : Enum
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, type);
            var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).SingleOrDefault();
            if (!isOptional)
            {
                return enumMemberAttribute?.Value;
            }

            return enumMemberAttribute != null ?
                    enumMemberAttribute.Value :
                    name;
        }

        public static string ToStringFromEnumMembers<T>(this IEnumerable<T> enumElements, bool enumMember = true)
            where T : Enum
        {
            if (enumElements is null || !enumElements.Any())
            {
                return null;
            }

            return string.Join(",", enumElements.Select(enumElement => enumMember ? enumElement.ToStringFromEnumMember() : Enum.GetName(typeof(T), enumElement)));
        }
    }
}

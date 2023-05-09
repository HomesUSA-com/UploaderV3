using System.ComponentModel;
using System.Runtime.Serialization;

namespace Husa.Uploader.Crosscutting.Extensions
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

        public static string GetEnumDescription<T>(this T enumeration)
            where T : Enum
        {
            var type = typeof(T);
            var name = Enum.GetName(type, enumeration);
            var memberInfo = type.GetMember(name).FirstOrDefault();
            if (memberInfo == null)
            {
                throw new InvalidOperationException($"Unable to get memberinfo for enum '{type.FullName}'");
            }

            var attribute = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false).FirstOrDefault();
            return ((DescriptionAttribute)attribute)?.Description ?? throw new InvalidOperationException($"Unable to get memberinfo for enum '{type.FullName}'");
        }
    }
}

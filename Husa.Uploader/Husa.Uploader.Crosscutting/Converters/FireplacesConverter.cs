namespace Husa.Uploader.Crosscutting.Converters
{
    public static class FireplacesConverter
    {
        public static string FireplacesToString(int? fireplaces)
        {
            if (fireplaces == null)
            {
                return null;
            }

            return (fireplaces <= 2) ? fireplaces.ToString() : "3+";
        }
    }
}

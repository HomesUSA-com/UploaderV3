namespace Husa.Uploader.Core.Models
{
    public class RoomType
    {
        internal readonly string Value;
        internal readonly string Level;
        internal readonly string Length;
        internal readonly string Width;
        internal readonly string Features;

        public RoomType(string value, string level, int? length, int? width, string features)
        {
            this.Value = value;
            this.Level = level ?? string.Empty;
            this.Length = length == null ? string.Empty : length.ToString();
            this.Width = width == null ? string.Empty : width.ToString();
            this.Features = features ?? string.Empty;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(this.Level) && !string.IsNullOrWhiteSpace(this.Length);
        }
    }
}

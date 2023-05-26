namespace Husa.Uploader.Core.Models
{
    using System.Text;

    public class UserRequest
    {
        public UserRequest(string username, string password)
            : this()
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            this.UserName = username;
            this.Password = password;
        }

        public UserRequest()
        {
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public StringContent AsContent() => new(
            content: this.ToString(),
            encoding: Encoding.UTF8,
            mediaType: "application/json");

        public override string ToString() => $"{{ \"username\": \"{this.UserName}\", \"password\": \"{this.Password}\" }}";
    }
}

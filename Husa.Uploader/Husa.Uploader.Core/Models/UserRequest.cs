using System.Text;

namespace Husa.Uploader.Core.Models
{
    public class UserRequest
    {
        public UserRequest(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            UserName = username;
            Password = password;
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public StringContent AsContent() => new(
            content: ToString(),
            encoding: Encoding.UTF8,
            mediaType: "application/json");

        public override string ToString() => $"{{ \"username\": \"{UserName}\", \"password\": \"{Password}\" }}";
    }
}

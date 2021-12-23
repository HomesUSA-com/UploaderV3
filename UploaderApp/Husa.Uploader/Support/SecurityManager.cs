using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Husa.Cargador.Support
{
    public  class SecurityManager
    {
        HttpClient client = new HttpClient();

        public SecurityManager()
        {
            client.BaseAddress = new Uri("http://domain.homesusa.com/api/v1/Authentication/login");
            /*client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));*/
        }

        public  void SignIn(string username, string password)
        {
            

        }
    }

    public class User
    {
        private int userID;
        private Guid userGUID;
        private string firstName;
        private string lastName;
        private string fullName;
        private string email;
        private string email2;
        private string username;

        public int UserID { get => userID; set => userID = value; }
        public Guid UserGUID { get => userGUID; set => userGUID = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string FullName { get => fullName; set => fullName = value; }
        public string Email { get => email; set => email = value; }
        public string Email2 { get => email2; set => email2 = value; }
        public string Username { get => username; set => username = value; }
    }

    public class AuthenticationResponse
    {
        private LoginStatus loginStatus;
        private User userData;
        private string statusCode;

        public LoginStatus LoginStatus { get => loginStatus; set => loginStatus = value; }
        public User UserData { get => userData; set => userData = value; }
        public string StatusCode { get => statusCode; set => statusCode = value; }
    }

    public enum LoginStatus
    {
        LoggedIn = 0,
        LoggedFailed = 1,
        LoggedOutReload = 2
    }
}

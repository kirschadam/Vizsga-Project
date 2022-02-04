using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Base_service.JsonClasses
{
    [DataContract]
    public class Response_User
    {
        private string message = null;
        private List<User> users = new List<User>();

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public List<User> Users
        {
            get { return users; }
            set { users = value; }
        }

        public Response_User() { }

        public Response_User(string message, List<User> users)
        {
            Message = message;
            Users = users;
        }
    }

    [DataContract]
    public class Response_Login
    {
        private string message = null, uid = null;
        private User user = null;

        [DataMember]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        [DataMember]
        public string Uid
        {
            get { return uid; }
            set { uid = value; }
        }

        [DataMember]
        public User User
        {
            get { return user; }
            set { user = value; }
        }

        public Response_Login() { }

        public Response_Login(string message, string uid, User user)
        {
            Message = message;
            Uid = uid;
            User = user;
        }
    }

    [DataContract]
    public class User
    {
        private string username, password, location = null;
        private int? id, permission, active = null;

        [DataMember]
        public int? Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [DataMember]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [DataMember]
        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        [DataMember]
        public int? Permission
        {
            get { return permission; }
            set { permission = value; }
        }

        [DataMember]
        public int? Active
        {
            get { return active; }
            set { active = value; }
        }

        public User() { }

        public User(int? id, string username, string password, string location, int? permission, int? active)
        {
            Id = id;
            Username = username;
            Password = password;
            Location = location;
            Permission = permission;
            Active = active;
        }
    }
}
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace CinemaHall.DAL
{
    internal class UserDAL
    {
        Database db = new Database();
        public int checkUserName(string userName)
        {
            string query = "select 1 from users where Username=@u";
            MySqlParameter[] parameter = { new MySqlParameter("@u", userName) };
            object result = db.ExecuteScalar(query, parameter);
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }
        public int signUp(string username,string firstName,string lastName, string passwordHash,int role)
        {
            string query = "Insert into users (Username,Password,RoleID,FirstName,LastName) values (@n,@p,@r,@f,@l)";
            MySqlParameter[] parameter = { new MySqlParameter("@n", username), new MySqlParameter("@f", firstName),new MySqlParameter("@l", lastName), new MySqlParameter("@p", passwordHash), new MySqlParameter("@r", role) };
            return db.ExecuteNonQuery(query,parameter);
        }

        // Returns the stored password hash for a username+role combination, or null if no match.
        // Actual password verification happens in the caller (BAL) using PasswordHasher,
        // since the stored value is salted and can't be matched with a plain SQL equality check.
        public string getPasswordHash(string username, int role)
        {
            string query = "Select Password from users where Username=@u && RoleID=@r";
            MySqlParameter[] parameter = { new MySqlParameter("@u", username), new MySqlParameter("@r", role) };
            object result = db.ExecuteScalar(query, parameter);
            return result == null || result == DBNull.Value ? null : Convert.ToString(result);
        }

        public int getId(string username) {
            string query = "select UserID from users where Username=@u";
            MySqlParameter[] parameter = { new MySqlParameter("@u", username) };
            return Convert.ToInt32(db.ExecuteScalar(query, parameter));

        }
        public int deleteUser(int userId)
        {

            string query = "Delete from users where UserID=@u";
            MySqlParameter[] parameter = { new MySqlParameter("@u", userId) };
            return db.ExecuteNonQuery(query,parameter);
        }
        public int deleteUserBookings(int userId)
        {
            string query = "Delete from bookings where UserID=@u";
            MySqlParameter[] parameter = { new MySqlParameter("@u", userId) };
            return db.ExecuteNonQuery(query, parameter);
        }

        // Password is intentionally excluded here -- this list is displayed on screen,
        // and even a hashed password shouldn't be shown back to admins.
        public DataTable showUser()
        {
            string query = "Select u.UserID,u.Username,r.RoleName From users u join roles r on r.roleID=u.roleID ";
            return  db.ExecuteQuery(query);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace CinemaHall.DAL
{
    internal class MovieDAL
    {
        Database db = new Database();
        public int SaveMovie(string title, string genre, int duration, DateTime date)
        {
            string query = "Insert INTO movies (Title,Genre,Duration,ReleaseDate) Values(@m,@g,@d,@dt)";
            MySqlParameter[] Parameters = { new MySqlParameter("@m", title), new MySqlParameter("@g", genre), new MySqlParameter("@d", duration), new MySqlParameter("@dt", date) };
            return db.ExecuteNonQuery(query, Parameters);

        }
        public DataTable RetriveMovies() {
            string query = "SELECT * FROM movies;";
            return db.ExecuteQuery(query);
        }
        public int DeleteMovie(int id)
        {
            string query = "Delete From Movies WHERE MovieID=@id";
            MySqlParameter[] parameters = { new MySqlParameter("@id", id) };
            return db.ExecuteNonQuery(query, parameters);
        }
        public int UpdateMovie(int id,string field,string newData)
        {
            string query = $"Update movies SET {field}=@value Where MovieID = @id";
            MySqlParameter[] parameters = { new MySqlParameter("@id", id) ,new MySqlParameter("@value",newData)};
            return db.ExecuteNonQuery(query, parameters);
        }
        public DataTable searchMovieId(int id)
        {
            string query = "Select * From movies Where MovieID=@id";
            MySqlParameter[] parameters = { new MySqlParameter("@id", id) };
            return db.ExecuteQuery(query,parameters);

        }
        public int countMovieIDs(int id)
        {
            DataTable dt=searchMovieId(id);
            if (dt.Rows.Count > 0)
            {
                return 1;
            }
            return 0;

        }
    }
}

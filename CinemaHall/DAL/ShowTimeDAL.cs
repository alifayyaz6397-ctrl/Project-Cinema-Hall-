using CinemaHall.UI;
using Google.Protobuf.Reflection;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaHall.DAL
{
    internal class ShowTimeDAL
    {
        Database db = new Database();
        public int ValidateShowTime(int movieId,int hallId,int slotId,DateTime showDate)
        {
            string query = "SELECT COUNT(*) FROM (SELECT 1) AS dummy WHERE EXISTS (SELECT 1 FROM movies WHERE MovieID = @movieId) AND EXISTS (SELECT 1 FROM halls WHERE HallID = @hallId) AND NOT EXISTS (SELECT 1 FROM showtimes WHERE HallID = @hallId AND ShowDate = @showDate AND SlotID = @slotId)";
            MySqlParameter[] parameter = { new MySqlParameter("@movieId", movieId), new MySqlParameter("@HallId", hallId), new MySqlParameter("@slotId", slotId), new MySqlParameter("@showDate", showDate.ToString("yyyy-MM-dd")) };
            return Convert.ToInt32(db.ExecuteScalar(query, parameter));
        }
        public int addShowTime(int movieId, int hallId, int slotId, DateTime showDate,int normalPrice,int premiumPrice)
        {
            string query = "Insert Into showtimes (MovieID,HallID,ShowDate,NormalPrice,VipPrice,SlotID) Values (@m,@h,@d,@n,@v,@s)";
            MySqlParameter[] parameter = { new MySqlParameter("@m", movieId), new MySqlParameter("@h", hallId), new MySqlParameter("@s", slotId), new MySqlParameter("@d", showDate.ToString("yyyy-MM-dd")), new MySqlParameter("@n", normalPrice),new MySqlParameter("@v", premiumPrice) };
            return db.ExecuteNonQuery(query, parameter);
        }
        public DataTable showShowTime()
        {
            string query = "Select ShowtimeID, Title,HallName,ShowDate,SlotName,StartTime,EndTime,NormalPrice,VipPrice From showtimes ST Join slots S ON S.SlotID=ST.SlotID JOIN movies M ON M.MovieID=ST.MovieID JOIN halls H ON H.HallID=St.HallID ";
            return db.ExecuteQuery(query);
        }
        public int deleteShow(int id)
        {
            string query = "Delete From showtimes Where ShowtimeID=@s";
            MySqlParameter[] parameter = { new MySqlParameter("@s",id ) };
            return db.ExecuteNonQuery(query, parameter);
        }
        public int updatePrice(int id,int option,int price)
        {
            string query = "";
            if (option == 1)
            {
                 query = "Update showtimes set VipPrice=@p where ShowtimeID=@id";
              
            }
            else if (option == 2)
            {
                 query = "Update showtimes set NormalPrice=@p where ShowtimeID=@id";
              
            }
            MySqlParameter[] parameter = { new MySqlParameter("@p", price) ,
                    new MySqlParameter("@id", id)};
            return db.ExecuteNonQuery(query, parameter);

            
        }
        public object FindMovie(int Movieid)
        {
            string query = "Select 1 From movies Where MovieID=@id";
            MySqlParameter[] parameters = { new MySqlParameter("@id", Movieid) };
            return db.ExecuteScalar(query, parameters);
        }
        public int UpdateMovie(int id,  int movieId)
        {

            string query = $"Update showtimes SET MovieID=@movieID Where ShowtimeID= @id";
            MySqlParameter[] parameters = { new MySqlParameter("@id", id), new MySqlParameter("@movieID", movieId) };
            return db.ExecuteNonQuery(query, parameters);
        }
        public int searchHall(int HallId)
        {
            string query = "SELECT 1 from halls where HallID = @id";
            MySqlParameter[] parameters = { new MySqlParameter("@id", HallId) };
            return Convert.ToInt32(db.ExecuteScalar(query, parameters));
        }
        public int checkHallFree(int id ,int HallID) 
        {
            string query = "SELECT COUNT(*) FROM showtimes AS other JOIN showtimes AS current ON current.ShowtimeID = @id WHERE other.HallID = @newHallID AND other.ShowDate = current.ShowDate AND other.SlotID = current.SlotID AND other.ShowtimeID != @id;";
            MySqlParameter[] parameters = { new MySqlParameter("@newHallID",HallID ), new MySqlParameter("@id", id) };
            return Convert.ToInt32(db.ExecuteScalar(query, parameters));
        }
        public int checkDateFree(int id, DateTime newDate)
        {
            string query = "SELECT COUNT(*) FROM showtimes AS other JOIN showtimes AS current ON current.ShowtimeID = @id WHERE other.HallID = current.HallID AND other.ShowDate = @ShowDate AND other.SlotID = current.SlotID AND other.ShowtimeID != @id;";
            MySqlParameter[] parameters = { new MySqlParameter("@ShowDate", newDate), new MySqlParameter("@id", id) };
            return Convert.ToInt32(db.ExecuteScalar(query, parameters));
        }
        public int checkSlotFree(int id,int slotID)
        {
            string query = "SELECT COUNT(*) FROM showtimes AS other JOIN showtimes AS current ON current.ShowtimeID = @id WHERE other.HallID = current.HallID AND other.ShowDate = current.ShowDate AND other.SlotID = @SlotID AND other.ShowtimeID != @id;";
            MySqlParameter[] parameters = { new MySqlParameter("@SlotID", slotID), new MySqlParameter("@id", id) };
            return Convert.ToInt32(db.ExecuteScalar(query, parameters));
        }
        public int updateDate(int id,DateTime date)
        {
            string query = "Update showtimes Set ShowDate=@d where showtimeID=@id";
            MySqlParameter[] parameters = { new MySqlParameter("@id", id), new MySqlParameter("@d", date) };
            return db.ExecuteNonQuery(query, parameters);
        }
        public int updateSlot(int id, int slotID)
        {
            string query = "Update showtimes Set SlotID=@d where showtimeID=@id";
            MySqlParameter[] parameters = { new MySqlParameter("@id", id), new MySqlParameter("@d", slotID) };
            return db.ExecuteNonQuery(query, parameters);
        }
        public int updateHall(int id, int HallID)
        {
            string query = "Update showtimes Set HallID=@d where showtimeID=@id";
            MySqlParameter[] parameters = { new MySqlParameter("@id", id), new MySqlParameter("@d", HallID) };
            return db.ExecuteNonQuery(query, parameters);
        }
    }
}

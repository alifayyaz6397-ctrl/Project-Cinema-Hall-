using CinemaHall.UI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CinemaHall.DAL
{
    internal class HallDAL
    {
        Database db = new Database(); 
        public int AddHall(string name, int rows, int columns, int type)
        {
            string query = "Insert Into halls (HallName,TotalRows,TotalCols,TypeID) Values(@hall,@rows,@cols,@type)";
            MySqlParameter[] parameters = { new MySqlParameter("@hall", name), new MySqlParameter("@rows", rows), new MySqlParameter("@cols", columns), new MySqlParameter("@type", type) };
            return Convert.ToInt32(db.ExecuteScalar(query, parameters));
        }
        public void addPremiumSeats(int LastID, List<int> seats)
        {
           

            for (int i = 0; i < seats.Count; i++)
            {
                string query2 = $"Insert Into hallvipseats (HallID,SeatNumber) Values(@id,@seat)";
                MySqlParameter[] parameter2 = { new MySqlParameter("@id", LastID), new MySqlParameter ( "@seat", seats[i])};
                db.ExecuteNonQuery(query2, parameter2);
            }
        }
        public DataTable showHalls()
        {
            string query = "SELECT Temp.*,(TotalSeats - PremiumSeats) AS NormalSeats FROM (SELECT H.HallID, H.HallName, (H.TotalCols * H.TotalRows) AS TotalSeats, (SELECT COUNT(SeatNumber) FROM hallvipseats HV WHERE HV.HallID = H.HallID) AS PremiumSeats, HT.TypeName, HT.BaseSurcharge FROM halls H JOIN halltypes HT ON H.TypeID = HT.TypeID) AS Temp;";
            return db.ExecuteQuery(query);
        }
        public DataTable  HallSearch(int id)
        {
            string query = "SELECT H.TotalRows, H.TotalCols, HV.SeatNumber FROM halls H LEFT JOIN hallvipseats HV ON H.HallID = HV.HallID WHERE H.HallID = @id";
            MySqlParameter[] parameters = { new MySqlParameter("@id", id) };
            return db.ExecuteQuery(query,parameters);
            
        }
        public int deleteHall(int id)
        {
            string query = "Delete From halls H Where H.HallID=@id";
            MySqlParameter[] parameter = { new MySqlParameter("id", id) };
            return db.ExecuteNonQuery(query, parameter);
        }
        public int updateHall(int id,int option,string field=null,int typeId=0,List<int>seats=null)
        {
            if (option == 1)
            {
                string query = "Update halls Set HallName=@field Where HallID=@id";
                MySqlParameter[] parameter = { new MySqlParameter("field", field), new MySqlParameter("@id", id) };
                return db.ExecuteNonQuery(query, parameter);
            }
            else if(option == 2)
            {
                string query = "Update halls Set TypeID=@typeId Where HallID=@id";
                MySqlParameter[] parameter = { new MySqlParameter("typeId", typeId), new MySqlParameter("@id", id) };
                return db.ExecuteNonQuery(query, parameter);
            }
            else if (option == 3)
            {
                string query = "DELETE FROM hallvipseats WHERE HallID = @id";
                MySqlParameter[] parameters = { new MySqlParameter("@id", id) };

                db.ExecuteNonQuery(query, parameters);

                for (int i = 0; i < seats.Count; i++)
                {
                    string query2 = $"Insert Into hallvipseats (HallID,SeatNumber) Values(@id,@seat)";
                    MySqlParameter[] parameter2 = { new MySqlParameter("@id", id), new MySqlParameter("@seat", seats[i]) };
                    db.ExecuteNonQuery(query2, parameter2);
                }

            }
            return 0;

        }
        public DataTable bookSeats()
        {
            string query = "select seatnumber from bookings";
            return db.ExecuteQuery(query);

        }
    }
}

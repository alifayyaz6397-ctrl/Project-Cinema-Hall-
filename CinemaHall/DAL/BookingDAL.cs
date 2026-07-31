using MySql.Data.MySqlClient;
using CinemaHall.BAL;
using System;
using System.Collections.Generic;
using System.Data;

namespace CinemaHall.DAL
{
    internal class BookingDAL
    {
        Database db = new Database();

        public DataTable getShowDetails(int showId)
        {
            string query = "select S.HallID,TotalCols,VipPrice,NormalPrice,BaseSurcharge from showtimes S join halls H  on H.HallID=S.HallID join halltypes HT on HT.TypeID=H.TypeID where ShowtimeID=@id";

            MySqlParameter[] parameter = { new MySqlParameter("@id", showId) };
            return db.ExecuteQuery(query, parameter);
        }

        // Now takes the actual signed-in user's id instead of a hardcoded value --
        // previously every transaction was recorded under UserID 2 regardless of who booked.
        public int addTransction(decimal totalAmount, int userId)
        {
            string query = "Insert into transactions (userID,totalAmount) values (@u,@t);Select LAST_INSERT_ID();";
            MySqlParameter[] parameter = { new MySqlParameter("@u", userId), new MySqlParameter("@t", totalAmount) };
            return Convert.ToInt32(db.ExecuteScalar(query, parameter));
        }

        // Same fix here: userId is now a real parameter (was hardcoded to 2),
        // and the seat-number parameter name typo ("n" -> "@n") is fixed, since
        // it previously would have thrown a "parameter not found" error at runtime.
        public bool addBooking(List<int> seats, List<int> premiums, decimal vipPrice, decimal normalPrice, int transactionId, int showId, int userId)
        {
            bool isInserted = true;
            foreach (int s in seats)
            {
                bool check = premiums.Contains(s);
                string seatType = check ? "VIP" : "Normal";
                decimal price = check ? vipPrice : normalPrice;
                string query = "Insert into bookings (UserID,ShowtimeID,SeatNumber,SeatType,PricePaid,Status,TransactionID) values (@u,@s,@n,@v,@p,'active',@t)";

                MySqlParameter[] parameter = {
                    new MySqlParameter("@u", userId),
                    new MySqlParameter("@s", showId),
                    new MySqlParameter("@n", s),
                    new MySqlParameter("@t", transactionId),
                    new MySqlParameter("@p", price),
                    new MySqlParameter("@v", seatType)
                };
                int rows = db.ExecuteNonQuery(query, parameter);
                if (rows <= 0) isInserted = false;
            }
            return isInserted;
        }

        public DataTable getBookings(int id = 0, bool isAdmin = false, int tempId = 0)
        {
            // Base query using your JOIN pattern
            string query = @"SELECT B.BookingID, U.Username, M.Title, H.HallName, B.SeatNumber, B.SeatType, B.PricePaid, S.ShowDate,B.Status 
                     FROM bookings B 
                     JOIN users U ON B.UserID = U.UserID
                     JOIN showtimes S ON B.ShowtimeID = S.ShowtimeID 
                     JOIN movies M ON S.MovieID = M.MovieID 
                     JOIN halls H ON S.HallID = H.HallID";

            if (isAdmin && id != 0) // Search by Booking ID (Admin only)
            {
                query += " WHERE B.BookingID = @id ";
                MySqlParameter[] p = { new MySqlParameter("@id", id) };
                return db.ExecuteQuery(query, p);
            }
            if (!isAdmin && tempId != 0) // View Personal Bookings (User only)
            {
                query += " WHERE B.UserID = @u ";
                MySqlParameter[] p = { new MySqlParameter("@u", tempId) };
                return db.ExecuteQuery(query, p);
            }
            if (!isAdmin) // View Personal Bookings (User only)
            {
                query += " WHERE B.UserID = @u ";
                MySqlParameter[] p = { new MySqlParameter("@u", UserBAL.userId) };
                return db.ExecuteQuery(query, p);
            }

            return db.ExecuteQuery(query); // View All (Admin only)
        }

        public int cancelBooking(int bId, int uId, bool isAdmin)
        {
            string query = "";
            MySqlParameter[] parameters;

            if (isAdmin)
            {
                // Admin can cancel any booking
                query = "UPDATE bookings SET Status = 'cancelled' WHERE BookingID = @bid";
                parameters = new MySqlParameter[] {
                    new MySqlParameter("@bid", bId)
                };
            }
            else
            {
                // User can only cancel if the booking belongs to them
                query = "UPDATE bookings SET Status = 'cancelled' WHERE BookingID = @bid AND UserID = @uid";
                parameters = new MySqlParameter[] {
                    new MySqlParameter("@bid", bId),
                    new MySqlParameter("@uid", uId)
                };
            }

            return db.ExecuteNonQuery(query, parameters);
        }
    }
}

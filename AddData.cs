using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMLShop
{
    public static class AddData
    {
        public static void AddUserData(User user, List<Product> products, Order order, OrderData orderData, SqliteConnection connection) 
        {
            User _user = user;
            List<Product> _products = products;
            Order _order = order;
            OrderData _orderData = orderData;  

            //проверяем наличие пользователя в БД, добавляем нового при необходимости
            string sqlFindUser = "SELECT _id " +
                "FROM Users " +
                $"WHERE Name = '{_user.Name}' AND Email = '{_user.Email}';";

            SqliteCommand command = new SqliteCommand(sqlFindUser, connection);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read())   // построчно считываем данные
                    {
                        _user.Id = reader.GetInt32(0);
                    }
                }
                else
                {
                    //создание нового пользователя - если функционал необходим - реализовать получение ИД записи
                    reader.Close();
                    string sqlAddUser = $"INSERT INTO Users (Name, Email) VALUES ('{_user.Name}', '{_user.Email}')";
                    command.CommandText = sqlAddUser ;
                    command.ExecuteNonQuery();
                }
            }

            string sqlOrderQuery = $"INSERT INTO Orders (Number, Regdate, UserId) VALUES ('{_order.Number}', '{_order.Regdate}', '{_user.Id}'); ";
            command.CommandText = sqlOrderQuery;
            command.ExecuteNonQuery();

            string sqlProductQuery;
            string sqlOrderDataQuery;
            foreach (Product product in _products)
            {
                sqlProductQuery = $"SELECT _id FROM Products WHERE Name = '{product.Name}'";
                command.CommandText = sqlProductQuery;
                Int64 id = (Int64)command.ExecuteScalar();
                sqlOrderDataQuery = $"INSERT INTO OrderData (OrderNumber, ProductId, ProductQuantity, Sum) VALUES ('{_order.Number}', '{id}', '{product.Quantity}', '{product.Price}')";
                command.CommandText = sqlOrderDataQuery;
                command.ExecuteNonQuery();
            }
        }
    }
}

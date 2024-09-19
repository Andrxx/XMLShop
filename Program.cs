using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using XMLShop;
// See https://aka.ms/new-console-template for more information


class Program
{
    static void Main(string[] args)
    {
        using (var connection = new SqliteConnection("Data Source=usersdata.db"))
        {
            Console.WriteLine("задайте адрес файла");
            string str = Console.ReadLine();
            if (string.IsNullOrEmpty(str)) str = "C:\\repository\\XMLShop\\Example.xml";
            connection.Open();
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;

            CreateDB(command);

            //расскоменитровать перед первым запуском приложения для коррректной работы
            AddSomeData(command); // добавление тестовых данных для работы приложения, совпадает с примером из ТЗ

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(str);

            XmlElement? xRoot = xDoc.DocumentElement;
            if (xRoot != null)
            {
                // обход всех ордеров в корневом элементе
                foreach (XmlElement xOrder in xRoot)
                {
                    User _user = new User();
                    Order _order = new Order();
                    List<Product> _products = new List<Product>();
                    OrderData _orderData = new OrderData();

                    //парсим xml
                    foreach (XmlNode xElement in xOrder.ChildNodes) 
                    {
                        if (xElement.Name == "no") 
                        {
                            _order.Number = Int32.Parse(xElement.InnerText);
                        }
                        if (xElement.Name == "reg_date")
                        {
                            _order.Regdate = xElement.InnerText;
                        }
                        if (xElement.Name == "reg_date")
                        {
                            _order.Regdate = xElement.InnerText;
                        }
                        //получаем список продуктов
                        if(xElement.Name == "product")
                        {
                            Product _product = new Product();
                            foreach (XmlNode xProduct in xElement.ChildNodes)
                            {                               
                                if(xProduct.Name == "name") _product.Name = xProduct.InnerText;
                                if (xProduct.Name == "price") _product.Price = float.Parse(xProduct.InnerText, CultureInfo.InvariantCulture);
                                if (xProduct.Name == "quantity") _product.Quantity = Int32.Parse(xProduct.InnerText);
                            }
                            _products.Add(_product);
                        }
                        if (xElement.Name == "user")
                        {
                            foreach (XmlNode xUser in xElement.ChildNodes)
                            {
                                if (xUser.Name == "fio") _user.Name = xUser.InnerText;
                                if (xUser.Name == "email") _user.Email = xUser.InnerText;
                            }
                        }
                    }

                    //после парсинга данных запрос к БД
                    AddData.AddUserData(_user, _products, _order, _orderData, connection);
                }
            }



            Console.WriteLine("Выполнено");
        }
        Console.Read();
    }


    //создаем структуру БД, если ее еще нет
    public static void CreateDB(SqliteCommand command)
    {
        command.CommandText = "CREATE TABLE IF NOT EXISTS Users(_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, Name TEXT NOT NULL, " +
                "Email TEXT, Phone INTEGER)";
        command.ExecuteNonQuery();
        command.CommandText = "CREATE TABLE IF NOT EXISTS Orders(_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, Number INTEGER NOT NULL, " +
            "Regdate TEXT NOT NULL, UserId INTEGER NOT NULL)";
        command.ExecuteNonQuery();
        command.CommandText = "CREATE TABLE IF NOT EXISTS Products(_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, Name TEXT NOT NULL, " +
                "Description TEXT, Quantity INTEGER NOT NULL, Price REAL NOT NULL)";
        command.ExecuteNonQuery();
        command.CommandText = "CREATE TABLE IF NOT EXISTS OrderData(OrderNumber INTEGER NOT NULL, " +
                "ProductId INTEGER NOT NULL, ProductQuantity INTEGER NOT NULL, Sum INTEGER NOT NULL)";
        command.ExecuteNonQuery();
    }

    //Добавляем тестовые данные
    public static void AddSomeData(SqliteCommand command)
    {
        command.CommandText = "INSERT INTO Users (Name, Email, Phone) VALUES ('Иванов Иван Иванович', 'abc@email.com', '345433445')";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO Users (Name, Email, Phone) VALUES ('Петров Виктор Семенович', 'xyz@email.com', '345433445')";
        command.ExecuteNonQuery();
        //command.CommandText = "INSERT INTO Orders (Number, Regdate, UserId) VALUES ('1', '23.12.2023', '3')";
        //command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO Products (Name, Description, Quantity, Price) VALUES ('LG 1755', 'phone', '5', '12000.75')";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO Products (Name, Description, Quantity, Price) VALUES ('Xiomi 12X', 'phone', '15', '42000.75')";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO Products (Name, Description, Quantity, Price) VALUES ('Noname 14232', 'phone', '50', '1.7')";
        command.ExecuteNonQuery();
        command.CommandText = "INSERT INTO Products (Name, Description, Quantity, Price) VALUES ('Noname 222', 'phone', '100', '3.14')";
        command.ExecuteNonQuery();

        //command.CommandText = "INSERT INTO OrderData (OrderId, ProductId, ProductQuantity, Sum) VALUES ('1', '2', '1', '1000.56')";
        //command.ExecuteNonQuery();
    }
}




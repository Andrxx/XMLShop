using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//класс-помойка для сущностей БД
namespace XMLShop
{
 
    public class User
    {
        public int Id;
        public string? Name;
        public string? Email;
        public int Phone;
        public string? Addres;
    }

    public class Order
    {
        public int Id;
        public int Number;
        public string? Regdate;
        public int UserId;
        public double Sum;      //не отслеживаемое в БД поле
    }

    public class Product
    {
        public int Id;
        public string? Name;
        public string? Description;
        public double Price;
        public int Quantity;
        public int OrderId;
    }

    public class OrderData
    {
        public int OrderId;
        public int ProductId;
        public int ProductQuantity;
        public double Sum;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            //ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            //var Name = ClaimsPrincipal.Current.Identity.Name;
            //var Name1 = User.Identity.Name;

            //var userName = principal.Claims.Where(c => c.Type == "sub").Single().Value;

            return Ok(Order.CreateOrders());
        }

    }


    #region Helpers

    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string ShipperCity { get; set; }
        public Boolean IsShipped { get; set; }


        public static List<Order> CreateOrders()
        {
            List<Order> OrderList = new List<Order> 
            {
                new Order {OrderID = 10248, CustomerName = "Almin Halilovic", ShipperCity = "Sarajevo", IsShipped = true },
                new Order {OrderID = 10249, CustomerName = "Armin Klacar", ShipperCity = "Sarajevo", IsShipped = false},
                new Order {OrderID = 10250,CustomerName = "Ensar Muratovic", ShipperCity = "Sarajevo", IsShipped = false },
                new Order {OrderID = 10251,CustomerName = "Mirhat Babic", ShipperCity = "Sarajevo", IsShipped = false},
                new Order {OrderID = 10252,CustomerName = "Benjamin Beganovic", ShipperCity = "Sarajevo", IsShipped = true},
                new Order {OrderID = 10252,CustomerName = "Zana Tatar", ShipperCity = "Kuwait", IsShipped = true},
                new Order {OrderID = 10252,CustomerName = "Azra Mahmutovic", ShipperCity = "Kuwait", IsShipped = true}
            };

            return OrderList;
        }
    }

    #endregion
}

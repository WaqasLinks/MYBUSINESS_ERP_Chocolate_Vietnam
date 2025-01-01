using Microsoft.AspNet.SignalR;
using MYBUSINESS.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MYBUSINESS.HubConnection
{
    public class PhevaHub : Hub
    {
        // Broadcast the updated product-customer list (array) to all connected clients
        public void BroadcastProductUpdate(string productsJSON)
        {
            var products = new List<CustomerDetailsViewModel>();
            products = JsonConvert.DeserializeObject<List<CustomerDetailsViewModel>>(productsJSON);
            Clients.All.BroadcastProductUpdate(products);
        } 
        public void BroadcastPaymentDetails(string paymenDetails)
        {
            var payments = new List<CustomerDetailsViewModel>();
            payments = JsonConvert.DeserializeObject<List<CustomerDetailsViewModel>>(paymenDetails);
            Clients.All.BroadcastPaymentDetails(payments);
        }
        public void BroadcastProductUpdates()
        {
            string productsJSON = "XCVFGHHDSADSAD";
            Clients.Caller.BroadcastProductUpdates(productsJSON);
        }
    }
}

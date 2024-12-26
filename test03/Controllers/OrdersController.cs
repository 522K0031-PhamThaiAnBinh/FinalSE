using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using test03.Models;

namespace test03.Controllers
{
    public class OrdersController : Controller
    {
        private test01Entities db = new test01Entities();



        // GET: Orders
        public ActionResult Index()
        {
            // Retrieve orders from the database, including Reservation data
            var orders = db.Orders.Include(o => o.Reservations).ToList();
            return View(orders);
        }


        // GET: Orders/Create
        public ActionResult Create()
        {
            // Use ReservationID as both value and display text in the dropdown
            ViewBag.ReservationID = new SelectList(db.Reservations, "ReservationID", "ReservationID");

            // ViewBag.MenuItems to load menu items into the dropdown
            ViewBag.MenuItems = new SelectList(db.MenuItems, "MenuItemID", "Name");

            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,ReservationID,TotalAmount,OrderDate,Status, SelectedMenuItems")] Orders orders, List<OrderDetails> selectedMenuItems)
        {
            if (ModelState.IsValid)
            {
                // Set OrderDate to current date and time
                orders.OrderDate = DateTime.Now;

                // Ensure only available items are included in the order
                var unavailableItems = selectedMenuItems.Where(item =>
                    !db.MenuItems.Any(m => m.MenuItemID == item.MenuItemID && m.IsAvailable.HasValue && m.IsAvailable.Value == true)).ToList();

                if (unavailableItems.Any())
                {
                    // If there are unavailable items, display an error message
                    TempData["ErrorMessage"] = "Some of the items you selected are currently unavailable.";
                    return RedirectToAction("Create");  // Redirect back to the Create view
                }

                // Calculate TotalAmount by adding up the total of selected items
                orders.TotalAmount = selectedMenuItems.Sum(item => item.Quantity * db.MenuItems
                    .Where(m => m.MenuItemID == item.MenuItemID)
                    .FirstOrDefault().Price);

                // Add order to the database
                db.Orders.Add(orders);
                db.SaveChanges();

                // Add the order details (items) to the OrderDetails table
                foreach (var item in selectedMenuItems)
                {
                    item.OrderID = orders.OrderID;  // Set the OrderID for each order detail
                    db.OrderDetails.Add(item);
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // If the model is invalid, or after redirecting back due to unavailable items, reload dropdowns
            ViewBag.ReservationID = new SelectList(db.Reservations, "ReservationID", "Status", orders.ReservationID);
            ViewBag.MenuItems = new SelectList(db.MenuItems, "MenuItemID", "Name");

            return View(orders);
        }


    }
}
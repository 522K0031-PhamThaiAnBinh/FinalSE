using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;  // Add this using statement
using System.Net;
using System.Web;
using System.Web.Mvc;
using test03.Models;

namespace test03.Controllers
{
    public class OrderDetailsController : Controller
    {
        private test01Entities db = new test01Entities();

        // GET: OrderDetails
        public ActionResult Index()
        {
            var orderDetails = db.OrderDetails.Include(o => o.MenuItems).Include(o => o.Orders);
            return View(orderDetails.ToList());  // ToList() now works after adding the using statement
        }

        // GET: OrderDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Fetch the Order by OrderID, including OrderDetails and related MenuItems
            var order = db.Orders.Include(o => o.OrderDetails.Select(od => od.MenuItems))  // Include related MenuItems for OrderDetails
                                 .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);  // Pass the Order model to the view
        }


        // GET: OrderDetails/Create for an existing Order
        public ActionResult Create(int orderId)
        {
            ViewBag.OrderID = orderId; // Pass the OrderID to the view
            ViewBag.MenuItemID = new SelectList(db.MenuItems, "MenuItemID", "Name");
            return View();
        }

        // POST: OrderDetails/Create for an existing Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int orderId, List<int> menuItemIds, List<int> quantities)
        {
            if (ModelState.IsValid)
            {
                for (int i = 0; i < menuItemIds.Count; i++)
                {
                    var orderDetails = new OrderDetails
                    {
                        OrderID = orderId,
                        MenuItemID = menuItemIds[i],
                        Quantity = quantities[i],
                        SubTotal = db.MenuItems.Where(m => m.MenuItemID == menuItemIds[i]).FirstOrDefault().Price * quantities[i]  // Where() works now
                    };

                    db.OrderDetails.Add(orderDetails);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // If validation fails, return to the view with the error.
            return View();
        }

        // GET: OrderDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetails orderDetails = db.OrderDetails.Find(id);
            if (orderDetails == null)
            {
                return HttpNotFound();
            }
            ViewBag.MenuItemID = new SelectList(db.MenuItems, "MenuItemID", "Name", orderDetails.MenuItemID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "Status", orderDetails.OrderID);
            return View(orderDetails);
        }

        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderDetailID,OrderID,MenuItemID,Quantity,SubTotal")] OrderDetails orderDetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderDetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MenuItemID = new SelectList(db.MenuItems, "MenuItemID", "Name", orderDetails.MenuItemID);
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "Status", orderDetails.OrderID);
            return View(orderDetails);
        }

        // GET: OrderDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetails orderDetails = db.OrderDetails.Find(id);
            if (orderDetails == null)
            {
                return HttpNotFound();
            }
            return View(orderDetails);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderDetails orderDetails = db.OrderDetails.Find(id);
            db.OrderDetails.Remove(orderDetails);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

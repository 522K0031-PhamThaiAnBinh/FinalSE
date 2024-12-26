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
    public class CustomersController : Controller
    {
        private test01Entities db = new test01Entities();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = db.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,FirstName,LastName,Email,PhoneNumber")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                customers.CreatedAt = DateTime.Now; // Automatically set the current date and time
                db.Customers.Add(customers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customers);
        }





        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = db.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,FirstName,LastName,Email,PhoneNumber,CreatedAt")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customers);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = db.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Find the customer
            var customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            // Delete related payments first, which are linked to orders of the customer
            var relatedPayments = db.Payments
                                    .Where(p => p.OrderID.HasValue && db.Orders
                                                                   .Any(o => o.OrderID == p.OrderID && o.ReservationID.HasValue && db.Reservations
                                                                                                              .Any(r => r.ReservationID == o.ReservationID && r.CustomerID == customer.CustomerID)))
                                    .ToList();
            db.Payments.RemoveRange(relatedPayments);

            // Delete related order details first, which are linked to orders of the customer
            var relatedOrderDetails = db.OrderDetails
                                        .Where(od => od.OrderID.HasValue && db.Orders
                                                                          .Any(o => o.OrderID == od.OrderID && o.ReservationID.HasValue && db.Reservations
                                                                                                                       .Any(r => r.ReservationID == o.ReservationID && r.CustomerID == customer.CustomerID)))
                                        .ToList();
            db.OrderDetails.RemoveRange(relatedOrderDetails);

            // Delete related orders, which are linked to the customer's reservations
            var relatedOrders = db.Orders
                                  .Where(o => o.ReservationID.HasValue && db.Reservations
                                                                         .Any(r => r.ReservationID == o.ReservationID && r.CustomerID == customer.CustomerID))
                                  .ToList();
            db.Orders.RemoveRange(relatedOrders);

            // Delete related reservations for this customer
            var relatedReservations = db.Reservations
                                        .Where(r => r.CustomerID == customer.CustomerID)
                                        .ToList();
            db.Reservations.RemoveRange(relatedReservations);

            // Finally, delete the customer
            db.Customers.Remove(customer);

            // Save all changes to the database
            db.SaveChanges();

            // Redirect to the customer list
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

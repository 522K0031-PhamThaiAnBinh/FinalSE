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
    public class PaymentsController : Controller
    {
        private test01Entities db = new test01Entities();

        // GET: Payments
        public ActionResult Index()
        {
            var payments = db.Payments.Include(p => p.Orders);
            return View(payments.ToList());
        }

        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            return View(payments);
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            // Use OrderID as both the value and display text
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "OrderID");
            return View();
        }


        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PaymentID,OrderID,Amount,PaymentMethod,PaymentDate,Status")] Payments payments)
        {
            if (ModelState.IsValid)
            {
                // The Amount is now updated by JavaScript before submitting the form
                payments.PaymentDate = DateTime.Now; // You can set the payment date here, if needed.
                db.Payments.Add(payments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // If validation fails, repopulate the OrderID dropdown and return the view
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "Status", payments.OrderID);
            return View(payments);
        }


        // GET: Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "Status", payments.OrderID);
            return View(payments);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PaymentID,OrderID,Amount,PaymentMethod,PaymentDate,Status")] Payments payments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderID = new SelectList(db.Orders, "OrderID", "Status", payments.OrderID);
            return View(payments);
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            return View(payments);
        }
        public JsonResult GetOrderTotal(int orderId)
        {
            var order = db.Orders
                          .Where(o => o.OrderID == orderId)
                          .FirstOrDefault();

            if (order != null)
            {
                // Calculate the total amount based on order details
                var totalAmount = db.OrderDetails
                                    .Where(od => od.OrderID == orderId)
                                    .Sum(od => od.SubTotal); // or adjust as needed based on your logic

                return Json(new { totalAmount = totalAmount }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { totalAmount = 0 }, JsonRequestBehavior.AllowGet);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payments payments = db.Payments.Find(id);
            db.Payments.Remove(payments);
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
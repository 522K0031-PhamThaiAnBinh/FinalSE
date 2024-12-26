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
    public class ReservationsController : Controller
    {
        private test01Entities db = new test01Entities();

        // GET: Reservations
        public ActionResult Index()
        {
            var reservations = db.Reservations.Include(r => r.Customers);
            return View(reservations.ToList());
        }

        // GET: Reservations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservations reservations = db.Reservations.Find(id);
            if (reservations == null)
            {
                return HttpNotFound();
            }
            return View(reservations);
        }

        // GET: Reservations/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,ReservationDate,ReservationTime,NumberOfGuests,TableNumber,Status,SpecialInstructions")] Reservations reservation)
        {
            if (ModelState.IsValid)
            {
                // Set the CreatedAt field to the current date and time
                reservation.CreatedAt = DateTime.Now;

                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FullName", reservation.CustomerID);
            return View(reservation);
        }


        // GET: Reservations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservations reservations = db.Reservations.Find(id);
            if (reservations == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", reservations.CustomerID);
            return View(reservations);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReservationID,CustomerID,ReservationDate,ReservationTime,NumberOfGuests,TableNumber,Status,SpecialInstructions,CreatedAt")] Reservations reservations)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", reservations.CustomerID);
            return View(reservations);
        }

        // GET: Reservations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservations reservations = db.Reservations.Find(id);
            if (reservations == null)
            {
                return HttpNotFound();
            }
            return View(reservations);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservations reservations = db.Reservations.Find(id);
            db.Reservations.Remove(reservations);
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

        [HttpPost]
        public ActionResult CreateReservation(string firstName, string lastName, string Phone, string Email, DateTime ReservationDate, TimeSpan ReservationTime, int NumberOfGuests)
        {
            try
            {
                // Step 1: Create a new customer
                var customer = new Customers
                {
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = Phone,
                    Email = Email,
                    CreatedAt = DateTime.Now
                };

                db.Customers.Add(customer);
                db.SaveChanges();

                // Step 2: Initialize tableNumber and check for availability
                Random rand = new Random();
                int tableNumber = 0;  // Initialize tableNumber with a default value
                bool isTableAvailable = false;

                // Try to find an available table
                while (!isTableAvailable)
                {
                    tableNumber = rand.Next(1, 21);  // You can adjust the range based on your tables

                    // Check if the table is already reserved for the same date and time
                    isTableAvailable = !db.Reservations.Any(r => r.ReservationDate == ReservationDate && r.ReservationTime == ReservationTime && r.TableNumber == tableNumber);
                }

                // Step 3: Create a reservation with the new CustomerID and the random table number
                var reservation = new Reservations
                {
                    CustomerID = customer.CustomerID, // Link to the new customer
                    ReservationDate = ReservationDate,
                    ReservationTime = ReservationTime,
                    NumberOfGuests = NumberOfGuests,
                    TableNumber = tableNumber,  // Assign the random table number
                    Status = "Confirmed",
                    CreatedAt = DateTime.Now
                };

                db.Reservations.Add(reservation);
                db.SaveChanges();

                // Step 4: Return success message and the assigned table number
                return Json(new { success = true, message = "Reservation created successfully!", tableNumber = tableNumber });
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Log detailed validation errors
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    .ToList();

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = $"Validation errors: {fullErrorMessage}";

                // Return detailed error
                return Json(new { success = false, message = exceptionMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult GetAvailableMenuItems()
        {
            var menuItems = db.MenuItems
                .Where(m => m.IsAvailable == true) // Handle nullable bool explicitly
                .Select(m => new
                {
                    m.MenuItemID,
                    m.Name,
                    m.Price,
                    m.Category
                })
                .ToList();

            return Json(menuItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateReservationWithOrder(string firstName, string lastName, string Phone, string Email, DateTime ReservationDate, TimeSpan ReservationTime, int NumberOfGuests, List<int> MenuItemIDs, List<int> Quantities, string SpecialInstructions)
        {
            // Debugging: Check if SpecialInstructions is received
            Console.WriteLine("Special Instructions: " + SpecialInstructions);

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Step 1: Create Customer
                    var customer = new Customers
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        PhoneNumber = Phone,
                        Email = Email,
                        CreatedAt = DateTime.Now
                    };
                    db.Customers.Add(customer);
                    db.SaveChanges();

                    // Step 2: Assign Table Number
                    Random rand = new Random();
                    int tableNumber = 0;
                    bool isTableAvailable = false;
                    while (!isTableAvailable)
                    {
                        tableNumber = rand.Next(1, 21);
                        isTableAvailable = !db.Reservations.Any(r => r.ReservationDate == ReservationDate && r.ReservationTime == ReservationTime && r.TableNumber == tableNumber);
                    }

                    // Step 3: Create Reservation (Include Special Instructions)
                    var reservation = new Reservations
                    {
                        CustomerID = customer.CustomerID,
                        ReservationDate = ReservationDate,
                        ReservationTime = ReservationTime,
                        NumberOfGuests = NumberOfGuests,
                        TableNumber = tableNumber,
                        Status = "Confirmed",
                        SpecialInstructions = SpecialInstructions,  // Ensure this is assigned
                        CreatedAt = DateTime.Now
                    };
                    db.Reservations.Add(reservation);
                    db.SaveChanges();

                    // Step 4: Create Order and OrderDetails (as you have already implemented)

                    transaction.Commit();

                    return Json(new { success = true, message = "Reservation and Pre-order created successfully!", tableNumber = tableNumber });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        [HttpPost]
        public ActionResult CreateReservationWithPayment(PaymentViewModel paymentData)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Step 1: Create Reservation
                    var reservation = new Reservations
                    {
                        CustomerID = paymentData.CustomerID,
                        ReservationDate = paymentData.ReservationDate,
                        ReservationTime = paymentData.ReservationDate.TimeOfDay,
                        NumberOfGuests = paymentData.NumberOfGuests,
                        TableNumber = paymentData.TableNumber,
                        Status = "Pending Payment",
                        CreatedAt = DateTime.Now
                    };
                    db.Reservations.Add(reservation);
                    db.SaveChanges();

                    // Step 2: Create Order (linked to the reservation)
                    var order = new Orders
                    {
                        ReservationID = reservation.ReservationID,
                        TotalAmount = paymentData.PaymentAmount
                    };
                    db.Orders.Add(order);
                    db.SaveChanges();

                    // Step 3: Create Payment (linked to the order)
                    var payment = new Payments
                    {
                        OrderID = order.OrderID,
                        Amount = paymentData.PaymentAmount,
                        PaymentMethod = paymentData.PaymentMethod,
                        Status = "Completed",
                        PaymentDate = DateTime.Now
                    };
                    db.Payments.Add(payment);
                    db.SaveChanges();

                    // Step 4: Update Reservation Status
                    reservation.Status = "Confirmed";
                    db.Entry(reservation).State = EntityState.Modified;
                    db.SaveChanges();

                    transaction.Commit();

                    return Json(new { success = true, message = "Reservation and payment completed successfully!", tableNumber = reservation.TableNumber });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

    }
}

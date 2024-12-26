using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using test03.Models;

namespace test03.Controllers
{
    public class MenuItemsController : Controller
    {
        private test01Entities db = new test01Entities();

        // GET: MenuItems
        public ActionResult Index()
        {
            try
            {
                // Test direct database access
                var menuItems = db.MenuItems.ToList(); // Should fetch all rows from the MenuItems table
                return View(menuItems); // Pass the data to the view
            }
            catch (Exception ex)
            {
                // Log the error to debug
                return Content("Error: " + ex.Message); // Display the error message
            }
        }


        // GET: MenuItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItems menuItems = db.MenuItems.Find(id);
            if (menuItems == null)
            {
                return HttpNotFound();
            }
            return View(menuItems);
        }

        // GET: MenuItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MenuItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MenuItems menuItem, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Ensure the directory exists
                    string directoryPath = Server.MapPath("~/Content/Images/MenuItems");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath); // Create directory if it doesn't exist
                    }

                    // Save the image to the directory and get the file name
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(directoryPath, fileName);
                    ImageFile.SaveAs(path);

                    // Assign the image file name to the model
                    menuItem.ImageFileName = fileName;
                }

                db.MenuItems.Add(menuItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menuItem);
        }


        // GET: MenuItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItems menuItems = db.MenuItems.Find(id);
            if (menuItems == null)
            {
                return HttpNotFound();
            }
            return View(menuItems);
        }

        // POST: MenuItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MenuItems menuItem, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Ensure the directory exists
                    string directoryPath = Server.MapPath("~/Content/Images/MenuItems");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath); // Create directory if it doesn't exist
                    }

                    // Save the image to the directory and get the file name
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(directoryPath, fileName);
                    ImageFile.SaveAs(path);

                    // Update the image file name
                    menuItem.ImageFileName = fileName;
                }

                db.Entry(menuItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menuItem);
        }


        // GET: MenuItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuItems menuItems = db.MenuItems.Find(id);
            if (menuItems == null)
            {
                return HttpNotFound();
            }
            return View(menuItems);
        }

        // POST: MenuItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Find the menu item to be deleted
            MenuItems menuItems = db.MenuItems.Find(id);

            if (menuItems == null)
            {
                return HttpNotFound();
            }

            // Delete related OrderDetails that reference this MenuItem
            var relatedOrderDetails = db.OrderDetails
                                        .Where(od => od.MenuItemID == menuItems.MenuItemID)
                                        .ToList();
            db.OrderDetails.RemoveRange(relatedOrderDetails);

            // Now delete the MenuItem itself
            db.MenuItems.Remove(menuItems);

            // Save changes to the database
            db.SaveChanges();

            // Redirect to the Index view
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace cis237assignment6.Models
{
    public class BeveragesController : Controller
    {
        private BeverageJMartinEntities db = new BeverageJMartinEntities();

        // GET: Beverages
        public ActionResult Index()
        {
            //Setup a variable to hold the Cars data set
            DbSet<Beverage> BeverageToSearch = db.Beverages;
            //Strings to hold the user data that might be in the session and to be the default values if user has not entered any.
            string filterName = " ";
            string filterPack = " ";
            string filterMinPrice = "";
            string filterMaxPrice = "";

            decimal minPrice = 0;
            decimal maxPrice = 1000000;
            
            //Check to see if there is a value in the session, and if there is assign it to the variable setup to
            // hold the value. Take the value and place into the ViewBag for transfer back to the sorted list.
            if(Session["name"]!= null && !string.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["name"];
                ViewBag.filterName = filterName;
            }

            if (Session["pack"] != null && !string.IsNullOrWhiteSpace((string)Session["pack"]))
            {
                filterPack = (string)Session["pack"];
                ViewBag.filterPack = filterPack;
            }

            if (Session["minPrice"] != null && !string.IsNullOrWhiteSpace((string)Session["minPrice"]))
            {
                filterMinPrice = (string)Session["minPrice"];
                try
                {
                    minPrice = Decimal.Parse(filterMinPrice);
                    ViewBag.filterMinPriceError = "";
                    ViewBag.filterMinePrice = filterMinPrice;
                }catch(Exception e)
                {
                    ViewBag.filterMinPriceError = "ERROR: Not a valid minumum price. Enter a number.";
                    ViewBag.filterMinPrice = "";
                }
            }

            if (Session["maxPrice"] != null && !string.IsNullOrWhiteSpace((string)Session["maxPrice"]))
            {
                filterMaxPrice = (string)Session["maxPrice"];
                filterMinPrice = (string)Session["minPrice"];
                try
                {
                    maxPrice = Decimal.Parse(filterMaxPrice);
                    ViewBag.filterMaxPriceError = "";
                    ViewBag.filterMaxPrice = filterMaxPrice;
                }
                catch (Exception e)
                {
                    ViewBag.filterMaxPriceError = "ERROR: Not a valid maximum price. Enter a number.";
                    ViewBag.filterMaxPrice = "";
                }
            }

            //Do the filter for all of the fields. Since the default value is "" it will function.
            IEnumerable<Beverage> filtered = BeverageToSearch.Where(bev => bev.price >= minPrice &&
                                                                           bev.price <= maxPrice &&
                                                                           bev.name.ToUpper().Contains(filterName.ToUpper())&&
                                                                           bev.pack.ToLower().Contains(filterPack.ToLower()));

            return View(filtered);
        }

        // GET: Beverages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: Beverages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                
                db.Beverages.Add(beverage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: Beverages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //This is the filter method. It will take in the data submitted from the form and store it in the session.
        //Add the HttpPost to make sure is is a post.
        //Add the ValidateAntiForgeryToken to make sure the person submiting the form got the form form the server.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter()
        {
            //Get the form data that was sent out of the Request object.
            //The string that is used as a key to get the data matches the
            //name property of the form control.
            String name = Request.Form.Get("name");
            String pack = Request.Form.Get("pack");
            String minPrice = Request.Form.Get("minPrice");
            String maxPrice = Request.Form.Get("maxPrice");

            //Store the form data into the session so that icn be retrieved to filter the data.
            Session["name"] = name;
            Session["pack"] = pack;
            Session["minPrice"] = minPrice;
            Session["maxPrice"] = maxPrice;

            //Redierct the user to the (Beverages)index page where the filting will be done.
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

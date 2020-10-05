using CongressoApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CongressoApp.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private static ApplicationUserManager _userManager;

        private CongressoContext db = new CongressoContext();

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: SysAdmin/Users
        public ActionResult Index()
        {
            List<UsersIndexViewModel> model = new List<UsersIndexViewModel>();
            List<ApplicationUser> users;
            try
            {
                //users = UserManager.Users.Where(u => !u.SysAdmin).OrderBy(u => u.Email).ToList();
                users = UserManager.Users.ToList();
                
                foreach (var u in users)
                {
                    model.Add(
                        new UsersIndexViewModel()
                        {
                            User = u,
                            Approver = users.Find(a => a.Id.Equals(u.UserApproverId))
                        }
                    );
                }

            }
            catch (Exception ex)
            {
                return View("Error");
            }

            return View(model);
        }

        // GET: SysAdmin/Users/Edit/5
        public ActionResult Edit(string id)
        {
            UsersViewModel model = new UsersViewModel();
            if (id == null)
            {
                return View("Index");
            }

            try
            {
                model.User = UserManager.Users.Where(u => u.Id.Equals(id)).First();
                model.UsersList = UserManager.Users.Where(l => l.Id != id && !l.SysAdmin && (int)l.UserCategory > (int)model.User.UserCategory).Select(
                         u => new SelectListItem
                         {
                             Text = u.Email,
                             Value = u.Id,
                             Selected = u.Id.Equals(model.User.UserApproverId)
                         }
                        );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (model.User == null)
            {
                return View("Index");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsersViewModel model)
        {
            
            if (ModelState.IsValid)
            {

                ApplicationUser user = db.Users.Find(model.User.Id);
                user.UserFirstName = model.User.UserFirstName;
                user.UserLastName = model.User.UserLastName;
                user.Email = model.User.Email;
                user.PhoneNumber = model.User.PhoneNumber;
                user.UserSignature = model.User.UserSignature;
                user.SysAdmin = model.User.SysAdmin;
                user.EmailConfirmed = model.User.EmailConfirmed;
                user.UserCategory = model.User.UserCategory;
                user.UserApproverId = model.User.UserApproverId;

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

    }
}
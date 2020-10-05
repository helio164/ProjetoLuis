using CongressoApp.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CongressoApp.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private CongressoContext _context;


        ////////////////////////////////////////////////////////////
        ///                     Dashboard
        ////////////////////////////////////////////////////////////

        // GET: Admin/Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }


        ////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////
        ///                     Users
        ////////////////////////////////////////////////////////////
        // GET: Admin/Users
        public ActionResult Users()
        {
            var users = _userManager.Users.Where(u => !u.SysAdmin).OrderBy(u => u.Email).ToList();

            return View(users);
        }

        // GET: SysAdmin/Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Index");
            }

            var user = _context.Users.Find(id);
            if (user == null)
            {
                return View("Index");
            }
            return View(user);
        }


        ////////////////////////////////////////////////////////////
       


    }
}
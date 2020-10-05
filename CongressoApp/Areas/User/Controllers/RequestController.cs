using CongressoApp.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CongressoApp.Areas.User.Controllers
{
    
    public class RequestController : Controller
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

        // GET: User/Request
        public ActionResult Index()
        {
            var LoggedUser = db.Users.Where(u => u.Email.Equals(User.Identity.Name)).First();
            var requests = db.Requests.Where(x => !x.Deleted && (x.CreatedBy.Id == LoggedUser.Id || x.ApproverLevel1.Id == LoggedUser.Id || x.ApproverLevel2.Id == LoggedUser.Id || x.ApproverLevel3.Id == LoggedUser.Id || x.ApproverLevel4.Id == LoggedUser.Id)).ToList();
            RequestsIndexViewModel model = new RequestsIndexViewModel
            {
                Accepted = requests.Where(x => x.StatusLevel4 == 0).OrderByDescending(x => x.Date).ToList(),
                Level4 = requests.Where(x => x.StatusLevel4 != 0).OrderByDescending(x => x.Date).ToList(),
                Level3 = requests.Where(x => x.StatusLevel3 != 0).OrderByDescending(x => x.Date).ToList(),
                Level2 = requests.Where(x => x.StatusLevel2 != 0).OrderByDescending(x => x.Date).ToList(),
                Level1 = requests.Where(x => x.StatusLevel1 != 0).OrderByDescending(x => x.Date).ToList()
            };

            return View(model);
        }

        // GET: User/Request/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Request/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Request/Create
        [HttpPost]
        public async Task<ActionResult> Create(Request request)
        {
            try
            {
                // TODO: Add insert logic here
                    var LoggedUser = db.Users.Where(u => u.Email.Equals(User.Identity.Name)).First();
                    request.CreatedBy = (ApplicationUser)LoggedUser;
                    request.CreatedAt = new DateTime();
                    request.Date = new DateTime();
                    var Approver = db.Users.Where(u => u.Id.Equals(LoggedUser.UserApproverId)).FirstOrDefault();
                switch (request.CreatedBy.UserCategory)
                {
                    case UserCategory.ADM:
                        request.StatusLevel4 = ApproveStatus.WAITING;
                        request.ApproverLevel4 = Approver;
                        request.CurrentLevel = (int)UserCategory.ADM;
                        request.StatusLevel3 = ApproveStatus.APPROVED;
                        request.ApproverLevel3 = request.CreatedBy;
                        request.StatusLevel2 = ApproveStatus.APPROVED;
                        request.ApproverLevel2 = request.CreatedBy;
                        request.StatusLevel1 = ApproveStatus.APPROVED;
                        request.ApproverLevel1 = request.CreatedBy;
                        break;
                    case UserCategory.DM:
                        request.StatusLevel3 = ApproveStatus.WAITING;
                        request.ApproverLevel3 = Approver;
                        request.CurrentLevel = (int)UserCategory.DM;
                        request.StatusLevel2 = ApproveStatus.APPROVED;
                        request.ApproverLevel2 = request.CreatedBy;
                        request.StatusLevel1 = ApproveStatus.APPROVED;
                        request.ApproverLevel1 = request.CreatedBy;
                        break;
                    case UserCategory.GP:
                        request.StatusLevel2 = ApproveStatus.WAITING;
                        request.ApproverLevel2 = Approver;
                        request.CurrentLevel = (int)UserCategory.GP;
                        request.StatusLevel1 = ApproveStatus.APPROVED;
                        request.ApproverLevel1 = request.CreatedBy;
                        break;
                    case UserCategory.GRV:
                    case UserCategory.DEL:
                    default:
                        request.StatusLevel1 = ApproveStatus.WAITING;
                        request.ApproverLevel1 = Approver;
                        request.CurrentLevel = (int)UserCategory.GRV;
                        break;

                }
                    //db.Professionals.Add(request.Professionals[0]);
                    db.Requests.Add(request);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");

            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: User/Request/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Request/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Request/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Request/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

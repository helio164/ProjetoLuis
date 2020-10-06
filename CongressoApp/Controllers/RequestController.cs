using CongressoApp.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CongressoApp.Controllers
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
            List<Request> requests;
            if (LoggedUser.UserCategory== UserCategory.ADM || LoggedUser.UserCategory == UserCategory.DM || LoggedUser.UserCategory == UserCategory.SA)
            {
                requests = db.Requests.Include("CreatedBy").Where(x => !x.Deleted && x.Active).ToList();
            }
            else
            {
                requests = db.Requests.Include("CreatedBy").Where(x => !x.Deleted && x.Active && (x.CreatedBy.Id == LoggedUser.Id || x.ApproverLevel1.Id == LoggedUser.Id || x.ApproverLevel2.Id == LoggedUser.Id || x.ApproverLevel3.Id == LoggedUser.Id || x.ApproverLevel4.Id == LoggedUser.Id)).ToList();
            }
            RequestsIndexViewModel model = new RequestsIndexViewModel
            {
                Accepted = requests.Where(x => x.CurrentLevel == 5).OrderByDescending(x => x.Date).ToList(),
                Level4 = requests.Where(x => x.CurrentLevel == 4).OrderByDescending(x => x.Date).ToList(),
                Level3 = requests.Where(x => x.CurrentLevel == 3).OrderByDescending(x => x.Date).ToList(),
                Level2 = requests.Where(x => x.CurrentLevel == 2).OrderByDescending(x => x.Date).ToList(),
                Level1 = requests.Where(x => x.CurrentLevel == 1).OrderByDescending(x => x.Date).ToList()
            };

            return View(model);
        }

        // GET: User/Request/Details/5
        public ActionResult Details(int id)
        {
            var model = new RequestViewModel();
                model.Request = db.Requests.Include("Professionals").Include("Documents").Include("CreatedBy").Include("ApproverLevel1").Include("ApproverLevel2").Include("ApproverLevel3").Include("ApproverLevel4").Where(r => r.Id == id).FirstOrDefault();
            List<DocumentViewModel> files = new List<DocumentViewModel>();
            //Fetch all files in the Folder (Directory).
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\Documentos\\{model.Request.Id}";
            string[] filePaths;
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                filePaths = Directory.GetFiles(path);
                //Copy File names to Model collection.
                foreach (string filePath in filePaths)
                {
                    files.Add(new DocumentViewModel { Name = Path.GetFileName(filePath), Path = filePath });
                }
            }
                model.Documents = files;
            return View(model);
        }

        // GET: User/Request/Create
        public ActionResult Create()
        {
            RequestViewModel model = new RequestViewModel();
            model.ProfessionalsList = db.Professionals.Select(
                         u => new SelectListItem
                         {
                             Text = u.Email,
                             Value = u.Id.ToString()
                         }
                        );

            return View(model);
        }

        // POST: User/Request/Create
        [HttpPost]
        public async Task<ActionResult> Create(RequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                Request request = model.Request;
                // TODO: Add insert logic here
                var LoggedUser = db.Users.Where(u => u.Email.Equals(User.Identity.Name)).First();
                request.CreatedBy = (ApplicationUser)LoggedUser;
                request.CreatedAt = DateTime.Now;
                request.Active = true;
                request.Date = request.Date = DateTime.Parse(model.RequestDate);

                var pro = db.Professionals.Where(p => p.Id.ToString() == model.SelectedPro).FirstOrDefault();

                var Approver = db.Users.Where(u => u.Id.Equals(LoggedUser.UserApproverId)).FirstOrDefault();
                switch (request.CreatedBy.UserCategory)
                {
                    case UserCategory.ADM:
                    case UserCategory.DM:
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
                    case UserCategory.GP:
                        request.StatusLevel3 = ApproveStatus.WAITING;
                        request.ApproverLevel3 = Approver;
                        request.CurrentLevel = (int)UserCategory.DM;
                        request.StatusLevel2 = ApproveStatus.APPROVED;
                        request.ApproverLevel2 = request.CreatedBy;
                        request.StatusLevel1 = ApproveStatus.APPROVED;
                        request.ApproverLevel1 = request.CreatedBy;
                        break;
                    case UserCategory.GRV:
                        request.StatusLevel2 = ApproveStatus.WAITING;
                        request.ApproverLevel2 = Approver;
                        request.CurrentLevel = (int)UserCategory.GP;
                        request.StatusLevel1 = ApproveStatus.APPROVED;
                        request.ApproverLevel1 = request.CreatedBy;
                        break;
                    case UserCategory.DEL:
                    default:
                        request.StatusLevel1 = ApproveStatus.WAITING;
                        request.ApproverLevel1 = Approver;
                        request.CurrentLevel = (int)UserCategory.GRV;
                        break;

                }
                db.Requests.Add(request);
                await db.SaveChangesAsync();
                if (files != null)
                {
                    string path = $"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\Documentos\\{request.Id}\\";
                    var dir = new DirectoryInfo(path);

                    if (!dir.Exists)
                    {
                        Directory.CreateDirectory(path);
                    }

                    foreach (var file in files)
                    {
                        if (file!=null)
                        {
                            string filePath = Path.GetFileName(file.FileName);
                            file.SaveAs(Path.Combine(path, filePath));
                            //Here you can write code for save this information in your database if you want 
                        }
                    }
                }

                //return RedirectToAction("AddFiles");
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: User/Request/Approve/5
        public ActionResult Approve(int id)
        {
            var model = db.Requests.Include("CreatedBy").Include("ApproverLevel1").Include("ApproverLevel2").Include("ApproverLevel3").Include("ApproverLevel4").Where(r => r.Id == id).FirstOrDefault();

            return View(model);
        }

        // POST: User/Request/Approve/5
        [HttpPost]
        public async Task<ActionResult> Approve(Request model)
        {
            try
            {
                Request request = db.Requests.Include("CreatedBy").Include("ApproverLevel1").Include("ApproverLevel2").Include("ApproverLevel3").Include("ApproverLevel4").Where(r => r.Id == model.Id).FirstOrDefault();

                request.Level1Notes = model.Level1Notes!=null ? model.Level1Notes : request.Level1Notes;
                request.Level2Notes = model.Level2Notes!=null ? model.Level2Notes : request.Level2Notes;
                request.Level3Notes = model.Level3Notes!=null ? model.Level3Notes : request.Level3Notes;
                request.Level4Notes = model.Level4Notes!=null ? model.Level4Notes : request.Level4Notes;

                var LoggedUser = db.Users.Where(u => u.Email.Equals(User.Identity.Name)).First();
                var Approver = db.Users.Where(u => u.Id.Equals(LoggedUser.UserApproverId)).FirstOrDefault();

                ApproveStatus? newStatus = null;
                if (model.StatusLevel1 != null || model.StatusLevel2 != null || model.StatusLevel3 != null || model.StatusLevel4 != null)
                {
                    newStatus = model.StatusLevel1 != null ? model.StatusLevel1 : model.StatusLevel2 != null ? model.StatusLevel2 : model.StatusLevel3 != null ? model.StatusLevel3 : model.StatusLevel4 != null? model.StatusLevel4 : newStatus;
                }

                if (newStatus != null && (newStatus == ApproveStatus.MISSINGINFO || newStatus == ApproveStatus.DISAPPROVED))
                {
                    switch (LoggedUser.UserCategory)
                    {
                        case UserCategory.ADM:
                            request.StatusLevel4 = newStatus;
                            request.Level4Notes = model.Level4Notes;
                            break;
                        case UserCategory.DM:
                            request.StatusLevel3 = newStatus;
                            request.Level3Notes = model.Level3Notes;
                            break;
                        case UserCategory.GP:
                            request.StatusLevel2 = newStatus;
                            request.Level2Notes = model.Level2Notes;
                            break;
                        case UserCategory.GRV:
                        case UserCategory.DEL:
                        default:
                            request.StatusLevel1 = newStatus;
                            request.Level1Notes = model.Level1Notes;
                            break;
                    }
                }

                if (newStatus != null && newStatus == ApproveStatus.APPROVED)
                {
                    switch (LoggedUser.UserCategory)
                    {
                        case UserCategory.ADM:
                            request.StatusLevel4 = ApproveStatus.APPROVED;
                            request.ApproverLevel4 = request.ApproverLevel4 != null ? request.ApproverLevel4 : LoggedUser;
                            request.CurrentLevel = 5;
                            request.StatusLevel3 = ApproveStatus.APPROVED;
                            request.ApproverLevel3 = request.ApproverLevel3 != null ? request.ApproverLevel3 : LoggedUser;
                            request.StatusLevel2 = ApproveStatus.APPROVED;
                            request.ApproverLevel2 = request.ApproverLevel2 != null ? request.ApproverLevel2 : LoggedUser;
                            request.StatusLevel1 = ApproveStatus.APPROVED;
                            request.ApproverLevel1 = request.ApproverLevel1 != null ? request.ApproverLevel1 : LoggedUser;
                            break;
                        case UserCategory.DM:
                            request.StatusLevel4 = ApproveStatus.WAITING;
                            request.ApproverLevel4 = Approver;
                            request.CurrentLevel = (int)UserCategory.ADM;
                            request.StatusLevel3 = ApproveStatus.APPROVED;
                            request.ApproverLevel3 = request.ApproverLevel3 != null ? request.ApproverLevel3 : LoggedUser;
                            request.StatusLevel2 = ApproveStatus.APPROVED;
                            request.ApproverLevel2 = request.ApproverLevel2 != null ? request.ApproverLevel2 : LoggedUser;
                            request.StatusLevel1 = ApproveStatus.APPROVED;
                            request.ApproverLevel1 = request.ApproverLevel1 != null ? request.ApproverLevel1 : LoggedUser;
                            break;
                        case UserCategory.GP:
                            request.StatusLevel3 = ApproveStatus.WAITING;
                            request.ApproverLevel3 = Approver;
                            request.CurrentLevel = (int)UserCategory.DM;
                            request.StatusLevel2 = ApproveStatus.APPROVED;
                            request.ApproverLevel2 = request.ApproverLevel2 != null ? request.ApproverLevel2 : LoggedUser;
                            request.StatusLevel1 = ApproveStatus.APPROVED;
                            request.ApproverLevel1 = request.ApproverLevel1 != null ? request.ApproverLevel1 : LoggedUser;
                            break;
                        case UserCategory.GRV:
                            request.StatusLevel2 = ApproveStatus.WAITING;
                            request.ApproverLevel2 = Approver;
                            request.CurrentLevel = (int)UserCategory.GP;
                            request.StatusLevel1 = ApproveStatus.APPROVED;
                            request.ApproverLevel1 = request.ApproverLevel1 != null ? request.ApproverLevel1 : LoggedUser;
                            break;
                        case UserCategory.DEL:
                        default:
                            request.StatusLevel1 = ApproveStatus.WAITING;
                            request.ApproverLevel1 = Approver;
                            request.CurrentLevel = (int)UserCategory.GRV;
                            break;
                    }
                }
                db.Entry(request).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Request/ApproveConfirmation/5
        public ActionResult ApproveConfirmation(int id)
        {
            return View();
        }

        // GET: User/Request/Edit/5
        public ActionResult Edit(int id)
        {
            var model = new RequestViewModel();
            model.Request = db.Requests.Include("CreatedBy").Include("ApproverLevel1").Include("ApproverLevel2").Include("ApproverLevel3").Include("ApproverLevel4").Where(r => r.Id == id).FirstOrDefault();
            model.RequestDate = model.Request.DateTimeFormat;
            List<DocumentViewModel> files = new List<DocumentViewModel>();
            //Fetch all files in the Folder (Directory).
            string path = $"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\Documentos\\{model.Request.Id}";
            string[] filePaths;
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                filePaths = Directory.GetFiles(path);
                //Copy File names to Model collection.
                foreach (string filePath in filePaths)
                {
                    files.Add(new DocumentViewModel { Name = Path.GetFileName(filePath), Path = filePath });
                }
            }
                model.Documents = files;

            return View(model);
        }

        public FileResult DownloadFile(string fileName, string filePath)
        {
            //Build the File Path.
            string path = filePath;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        // POST: User/Request/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(RequestViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            try
            {
                // TODO: Add update logic here
                Request request = db.Requests.Include("Professionals").Include("Documents").Include("CreatedBy").Include("ApproverLevel1").Include("ApproverLevel2").Include("ApproverLevel3").Include("ApproverLevel4").Where(r => r.Id == model.Request.Id).FirstOrDefault();
                request.Theme = model.Request.Theme;
                request.Location = model.Request.Location;
                request.Date = DateTime.Parse(model.RequestDate) != null ? DateTime.Parse(model.RequestDate) : model.Request.Date;
                request.Price = model.Request.Price;
                request.Notes = model.Request.Notes;
                request.Active = model.Request.Active;

                switch (request.CurrentLevel)
                {
                    case 1:
                        request.StatusLevel1 = ApproveStatus.WAITING;
                        break;
                    case 2:
                        request.StatusLevel2 = ApproveStatus.WAITING;
                        break;
                    case 3:
                        request.StatusLevel3 = ApproveStatus.WAITING;
                        break;
                    case 4:
                        request.StatusLevel4 = ApproveStatus.WAITING;
                        break;
                    default:
                        break;
                }

                db.Entry(request).State = EntityState.Modified;
                await db.SaveChangesAsync();

                if (files != null)
                {
                    string path = $"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\Documentos\\{request.Id}\\";
                    var dir = new DirectoryInfo(path);

                    if (!dir.Exists)
                    {
                        Directory.CreateDirectory(path);
                    }

                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            string filePath = Path.GetFileName(file.FileName);
                            file.SaveAs(Path.Combine(path, filePath));
                        }
                    }
                }

                return RedirectToAction("Details", new { id = model.Request.Id });
            }
            catch(Exception ex)
            {
                model.Documents = new List<DocumentViewModel>();
                return View(model);
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

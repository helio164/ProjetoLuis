using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CongressoApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public bool SysAdmin { get; set; }
        [Display(Name="Nome")]
        public string UserFirstName { get; set; }
        [Display(Name = "Apelido")]
        public string UserLastName { get; set; }
        [Display(Name = "Assinatura")]
        public string UserSignature { get; set; }
        [Display(Name = "Categoria")]
        public UserCategory UserCategory { get; set; }
        [Display(Name = "Aprovador")]
        public string UserApproverId { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class UsersIndexViewModel
    {
        public ApplicationUser User { get; set; }
        [Display(Name = "Aprovador")]
        public ApplicationUser Approver { get; set; }
    }

    public class UsersViewModel
    {
        public ApplicationUser User { get; set; }
        [Display(Name = "Aprovador")]
        public ApplicationUser Approver { get; set; }
        public IEnumerable<SelectListItem> UsersList { get; set; }
    }

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("CongressoContext", throwIfV1Schema: false)
    //    {
    //    }

    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }
    //}
}
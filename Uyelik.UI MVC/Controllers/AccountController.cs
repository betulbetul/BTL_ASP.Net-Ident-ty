﻿using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Uyelik.BLL.Account;
using Uyelik.Entity.Enums;
using Uyelik.Entity.IdentityModels;
using Uyelik.Entity.ViewModels;

namespace Uyelik.UI_MVC.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userManager = MembershipTools.NewUserManager();
            var checkUser = userManager.FindByName(model.UserName);
            if (checkUser != null)
            {
                ModelState.AddModelError(String.Empty, "Bu kullanıcı adı başka biri tarafından daha önceden alınmıştır!");
                return View(model);
            }
            var user = new ApplicationUser()
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                UserName = model.UserName,
            };

            var sonuc = userManager.Create(user, model.Password);
            if (sonuc.Succeeded)
            {
                if (userManager.Users.Count() == 1)
                {
                    userManager.AddToRole(user.Id, IdentityRoles.Admin.ToString());
                }
                else
                    userManager.AddToRole(user.Id, IdentityRoles.Passive.ToString());
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı kayıt işleminde hata oluştu!");
                return View(model);
            }
        }

        // GET: Account
        public ActionResult Login()
        {


            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userManager = MembershipTools.NewUserManager();
            var user = await userManager.FindAsync(model.UserName, model.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Böyle bir kullanıcı bulunamadı!");
                return View(model);
            }
            var authManager = HttpContext.GetOwinContext().Authentication;
            var userIdentity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            authManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = model.RememberMe

            }, userIdentity);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}
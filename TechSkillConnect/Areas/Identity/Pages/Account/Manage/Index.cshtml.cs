//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.
//#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;

namespace TechSkillConnect.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private Tutor _Tutor;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        //[BindProperty]
        //public InputModel Input { get; set; }
        [BindProperty] 
        public Tutor _tutor { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        //public class InputModel
        //{
        //    /// <summary>
        //    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        //    ///     directly from your code. This API may change or be removed in future releases.
        //    /// </summary>
        //    [Phone]
        //    [Display(Name = "Phone number")]
        //    public string PhoneNumber { get; set; }
        //}

        //private async Task LoadAsync(IdentityUser user)
        //{
        //    var userName = await _userManager.GetUserNameAsync(user);
        //    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        //    Username = userName;

        //    Input = new InputModel
        //    {
        //        PhoneNumber = phoneNumber
        //    };
        //}
        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == user.Id);

            // Assign values to the InputModel
            tutor = new Tutor
            {
                Tutor_firstname = tutor.Tutor_firstname,
                Tutor_lastname = tutor.Tutor_lastname,
                CountryOfBirth = tutor.CountryOfBirth,
                TutorEmail = email,
                Tutor_phone = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        //    public async Task<IActionResult> OnPostAsync()
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        if (user == null)
        //        {
        //            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            await LoadAsync(user);
        //            return Page();
        //        }

        //        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        //        if (Input.PhoneNumber != phoneNumber)
        //        {
        //            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
        //            if (!setPhoneResult.Succeeded)
        //            {
        //                StatusMessage = "Unexpected error when trying to set phone number.";
        //                return RedirectToPage();
        //            }
        //        }

        //        await _signInManager.RefreshSignInAsync(user);
        //        StatusMessage = "Your profile has been updated";
        //        return RedirectToPage();
        //    }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.IdentityID == user.Id);
            if (tutor != null)
            {
                tutor.Tutor_firstname = _tutor.Tutor_firstname;
                tutor.Tutor_lastname = _tutor.Tutor_lastname;
                tutor.CountryOfBirth = _tutor.CountryOfBirth;
            }

            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, _tutor.Tutor_phone);
            var setEmailResult = await _userManager.SetEmailAsync(user, _tutor.TutorEmail);

            if (!setPhoneResult.Succeeded || !setEmailResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number or email.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DepartmentAndSubDepartmentsMVC.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        #region Private Variables.
        #region Dependency
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<UserController> _logger;
        #endregion
        #endregion

        #region Constructor.
        public UserController(SignInManager<IdentityUser> signInManager, ILogger<UserController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }
        #endregion


        #region Public Methods.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return new RedirectResult("/Identity/Account/Login");
        }
        #endregion
    }
}

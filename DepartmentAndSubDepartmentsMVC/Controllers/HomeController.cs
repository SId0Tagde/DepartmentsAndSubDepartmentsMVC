using DepartmentAndSubDepartmentsMVC.Models;
using DepartmentAndSubDepartmentsMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DepartmentAndSubDepartmentsMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        #region Private Variables
        #region Dependency
        private readonly ILogger<HomeController> _logger;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        #endregion
        #endregion

        #region Constructor
        public HomeController(SignInManager<IdentityUser> signInManager,ILogger<HomeController> logger, IDepartmentRepository departmentRepository)
        {
            _signInManager = signInManager;
            _logger = logger;
            _departmentRepository = departmentRepository;
        }
        #endregion

        #region Public Methods.
        public async Task<ActionResult<List<Department>>> Index()
        {
            try
            {
                return View(await _departmentRepository.GetAllAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var department = await _departmentRepository.GetByIdWithHierarchyAsync(id);
                if (department == null)
                {
                    return NotFound();
                }

                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = new SelectList(await _departmentRepository.GetAllAsync(),
                "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var potentialParent = await _departmentRepository.GetByIdWithHierarchyAsync(Convert.ToInt32(department.ParentDepartmentId));
                    await _departmentRepository.AddAsync(department);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "");
                    return View(department);
                }
            }
            ViewBag.Departments = new SelectList(await _departmentRepository.GetAllAsync(), "Id", "Name",
                department.ParentDepartmentId);
            return View(department);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                ViewBag.Departments = new SelectList(await _departmentRepository.GetAllAsync(),
                "Id", "Name");
                var department = await _departmentRepository.GetByIdAsync(id);
                if (department == null) { return NotFound(); }
                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {   
                    var potentialParent = await _departmentRepository.GetByIdWithHierarchyAsync(Convert.ToInt32(department.ParentDepartmentId));
                    if (potentialParent?.Id == department.Id)
                    {
                        ModelState.AddModelError(nameof(department.ParentDepartmentId), "Circular parenting detected.");
                    }
                    if (!await _departmentRepository.HasCircularParenting(department, potentialParent?.ParentDepartment!))
                    {
                        await _departmentRepository.UpdateAsync(department);
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError(nameof(department.ParentDepartmentId), "Circular parenting detected.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "");
                    return View(department);
                }
            }
            ViewBag.Departments = new SelectList(await _departmentRepository.GetAllAsync(), "Id", "Name",
                department.ParentDepartmentId);
            return View(department);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(id);
                if (department == null)
                {
                    return NotFound();
                }
                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(id);

                if (department == null)
                {
                    return NotFound();
                }

                // Check if the department has any sub-departments
                bool hasSubDepartments;
                if (department.SubDepartments != null)
                {
                    hasSubDepartments = department.SubDepartments.Any();
                }
                else
                {
                    hasSubDepartments = false;
                }

                if (department.ParentDepartmentId == null && !hasSubDepartments)
                {
                    // Department has no parent and no sub-departments, safe to delete
                    await _departmentRepository.DeleteAsync(department.Id);
                }
                else if (department.ParentDepartmentId != null && !hasSubDepartments)
                {
                    // Department has a parent department
                    var parentDepartment = await _departmentRepository.GetByIdAsync(department.ParentDepartmentId.Value);
                    if (parentDepartment != null)
                    {
                        // Remove this department from the parent's sub-departments list
                        if (parentDepartment.SubDepartments != null)
                        {
                            parentDepartment.SubDepartments.Remove(department);
                            await _departmentRepository.UpdateAsync(parentDepartment);
                        }
                    }

                    // Delete the department
                    await _departmentRepository.DeleteAsync(department.Id);
                }
                else if (department.ParentDepartmentId != null && hasSubDepartments)
                {
                    // Department has a parent department
                    var dept = await _departmentRepository.GetByIdAsync(id);
                    // Remove the department from the parent department's sub-departments list
                    dept?.ParentDepartment?.SubDepartments?.Remove(department);

                    // Set the parent department to null for each sub-department
                    foreach (var subDepartment in department?.SubDepartments ?? new List<Department>())
                    {
                        subDepartment.ParentDepartment = null;
                    }
                    // Delete the department
                    if (dept != null)
                        await _departmentRepository.DeleteAsync(dept.Id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}

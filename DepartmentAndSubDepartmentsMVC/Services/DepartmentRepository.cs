using DepartmentAndSubDepartmentsMVC.Data;
using DepartmentAndSubDepartmentsMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace DepartmentAndSubDepartmentsMVC.Services
{
    public class DepartmentRepository : IDepartmentRepository
    {
        #region Private Variables.
        #region Dependency
        private readonly ApplicationDbContext _context;
        #endregion
        #endregion

        #region Constructor.
        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Private Methods.
        private async Task<Department?> GetByIdWithParentsAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.ParentDepartment)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        #endregion

        #region Public Methods
        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments.Include(d => d.SubDepartments)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Department>> GetParentDepartmentsAsync(int id)
        {
            var department = await _context.Departments
                .Include(d => d.ParentDepartment)
                .FirstOrDefaultAsync(d => d.Id == id);

            var parents = new List<Department>();
            while (department?.ParentDepartment != null)
            {
                parents.Add(department.ParentDepartment);
                department = await GetByIdWithParentsAsync(department.ParentDepartment.Id) ;
            }
            return parents;
        }

        public async Task<IEnumerable<Department>> GetSubDepartmentsAsync(int id)
        {
            return await _context.Departments
                  .Where(d => d.ParentDepartmentId == id)
                  .ToListAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Department?> GetByIdWithHierarchyAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.SubDepartments)
                .Include(d => d.ParentDepartment)
                .ThenInclude(p => p.ParentDepartment) // Ensure recursive loading of parent departments
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> HasCircularParenting(Department department, Department potentialParent)
        {
            // Base case: If potentialParent is null, or it's the department itself, return false
            if (potentialParent == null)
            {
                return false;
            }


            // Check if the potentialParent is the parent of the department
            var currentParent = potentialParent.ParentDepartment;
            while (currentParent != null)
            {
                if (currentParent.Id == department.Id)
                {
                    return true; // Circular reference found
                }
                //currentParent = currentParent.ParentDepartment;
                currentParent = await GetByIdWithParentsAsync(Convert.ToInt32(currentParent.ParentDepartmentId));
            }

            // Recursively check the potentialParent's parent
            if (potentialParent.ParentDepartment != null)
                return await HasCircularParenting(department, potentialParent.ParentDepartment);
            else
                return false;
        }
        #endregion
    }
}

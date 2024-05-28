using DepartmentAndSubDepartmentsMVC.Models;

namespace DepartmentAndSubDepartmentsMVC.Services
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
        Task<IEnumerable<Department>> GetSubDepartmentsAsync(int id);
        Task<IEnumerable<Department>> GetParentDepartmentsAsync(int id);
        Task<Department?> GetByIdWithHierarchyAsync(int id);

        Task<bool> HasCircularParenting(Department department, Department potentialParent);
    }
}

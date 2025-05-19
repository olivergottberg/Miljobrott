using Microsoft.EntityFrameworkCore;
using Miljöboven.Models;
using Miljoboven4.ViewModels;

// Repository med metoder för att hämta och uppdatera ärenden samt hämta ärenden till specifika roller

namespace Miljoboven4.Models
{
    public class EFEnvironmentRepository : IEnvironmentRepository
    {
        private readonly ApplicationDbContext context;
        private IHttpContextAccessor contextAcc;

        public EFEnvironmentRepository(ApplicationDbContext ctx, IHttpContextAccessor cont)
        {
            context = ctx;
            contextAcc = cont;
        }

        public IQueryable<Department> Departments => context.Departments;
        public IQueryable<Employee> Employees => context.Employees;
        public IQueryable<Errand> Errands =>
            context.Errands.Include(e => e.Samples).Include(e => e.Pictures);
        public IQueryable<ErrandStatus> ErrandStatuses => context.ErrandStatuses;
        public IQueryable<Picture> Pictures => context.Pictures;
        public IQueryable<Sample> Samples => context.Samples;
        public IQueryable<Sequence> Sequences => context.Sequences;

        // Hämtar all information om ett ärende från dess ID
        public Errand GetErrandDetail(int Id)
        {
            var errand = Errands.FirstOrDefault(e => e.ErrandId == Id);
            return errand;
        }

        // Sparar/uppdaterar ett ärende, ett nytt RefNumber skapas om inget finns
        public void SaveErrand(Errand errand) 
        {
            if(errand.ErrandId == 0)
            {
                var sequence = context.Sequences.FirstOrDefault(e => e.ID == 1);

                errand.RefNumber = $"{DateTime.Now.Year}-45-{sequence.CurrentValue}";
                errand.StatusId = "S_A";
                
                context.Errands.Add(errand);

                sequence.CurrentValue++;
            }
            context.SaveChanges();
        }

        // Hämtar alla ärenden som visas på startsidan när man loggar in, används för samordnare
        public IQueryable<ErrandViewModel> GetAllErrands ()
        {
            var errandList = from err in Errands
                             join stat in ErrandStatuses on err.StatusId equals stat.StatusID
                             join dep in Departments on err.DepartmentId equals dep.DepartmentID
                                into departmentErrand
                             from deptE in departmentErrand.DefaultIfEmpty()
                             join em in Employees on err.EmployeeId equals em.EmployeeId
                                into employeeErrand
                             from empE in employeeErrand.DefaultIfEmpty()
                             orderby err.RefNumber descending

                             select new ErrandViewModel
                             {
                                 DateOfObservation = err.DateOfObservation,
                                 ErrandId = err.ErrandId,
                                 RefNumber = err.RefNumber,
                                 TypeOfCrime = err.TypeOfCrime,
                                 StatusName = stat.StatusName,
                                 DepartmentName = (err.DepartmentId == null ? "Ej tillsatt" : deptE.DepartmentName),
                                 EmployeeName = (err.EmployeeId == null ? "Ej tilsatt" : empE.EmployeeName)
                             };
            return errandList;
        }

        // Hämtar ärenden som den inloggade handläggaren ska se
        public IQueryable<ErrandViewModel> GetInvestigatorErrands ()
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            userName = Employees.FirstOrDefault(e => e.EmployeeId == userName).EmployeeName;
            var investigatorErrands = GetAllErrands().Where(IHttpContextAccessor => IHttpContextAccessor.EmployeeName == userName);
            return investigatorErrands;
        }

        // Hämtar ärenden som den inloggade chefen ska se
        public IQueryable<ErrandViewModel> GetManagerErrands()
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            var userDepartmentId = Employees.FirstOrDefault(e => e.EmployeeId == userName).DepartmentId;
            var userDepartmentName = Departments.FirstOrDefault(d => d.DepartmentID == userDepartmentId).DepartmentName;
            var managerErrands = GetAllErrands().Where(errand => errand.DepartmentName == userDepartmentName);
            return managerErrands;
        }

        // Filtrerar de handläggare som en chef ska se, de handläggare som hör till chefens avdelning
        public IEnumerable<Employee> FilterEmployees()
        {
			var userName = contextAcc.HttpContext.User.Identity.Name;
            var manager = Employees.FirstOrDefault(e => e.EmployeeId == userName);
            var departmentId = manager.DepartmentId;
            var employeeList = Employees.Where(e => e.DepartmentId == departmentId);
            return employeeList;
		}
	}
}

namespace DemoApp.Common;

public interface IEmployee
{
    public string Name { get; init; }
    public Guid ID { get; init; }
    public string JobTitle { get; init; }
    public int Salary { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public record Employee(string Name,Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null, Manager? Manager = null) : IEmployee;
public record NewHire() : Employee("New Hire", Guid.NewGuid(), "New Hire", 0, DateTime.Now);

public record Manager(string Name, Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null, List<Employee>? DirectReports = default): IEmployee;
public record ManagerInTraining(string Name, Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null) : Manager(Name, ID, JobTitle, Salary, StartDate, EndDate);

public record HrPerson(string Name, Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null, List<Employee>? DirectReports = default): IEmployee;
public record HrPersonInTraining(string Name, Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null) : HrPerson(Name, ID, JobTitle, Salary, StartDate, EndDate);


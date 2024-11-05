using System.Security.Cryptography.X509Certificates;

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

public class JuniorProgrammer(string Name, Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null, Manager? Manager = null) : IEmployee
{
    /// <inheritdoc />
    public string Name { get; init; } = Name;

    /// <inheritdoc />
    public Guid ID { get; init; } = ID;

    /// <inheritdoc />
    public string JobTitle { get; init; } = JobTitle;

    /// <inheritdoc />
    public int Salary { get; init; } = Salary;

    /// <inheritdoc />
    public DateTime StartDate { get; init; } = StartDate;

    /// <inheritdoc />
    public DateTime? EndDate { get; init; } = EndDate;
}

public record Programmer(string Name, Guid ID, DateTime StartDate, string JobTitle ="programmer", int Salary = 1000, DateTime? EndDate = null, Manager? Manager = null) : IEmployee;
public record NewHire() : Programmer("New Hire", Guid.NewGuid(), DateTime.Now, "New Hire", 100);
public record Manager(string Name, Guid ID, DateTime StartDate, string JobTitle = "Manager", int Salary = 2000, DateTime? EndDate = null, List<Programmer>? DirectReports = default) : IEmployee;

public record ManagerInTraining(string Name, Guid ID, DateTime StartDate, string JobTitle = "Manager Training", int Salary = 1500, DateTime? EndDate = null) : Manager(Name, ID, StartDate, JobTitle, Salary, EndDate);

public record HrPerson(string Name, Guid ID, DateTime StartDate, string JobTitle = "HR", int Salary = 2000, DateTime? EndDate = null, List<Programmer>? DirectReports = default) : IEmployee;

public record HrPersonInTraining(string Name, Guid ID, DateTime StartDate, string JobTitle = "HR Training", int Salary = 1000, DateTime? EndDate = null) : HrPerson(Name, ID, StartDate, JobTitle, Salary, EndDate);
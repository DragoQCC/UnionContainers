using Bogus;
namespace UnionContainers.Benchmarks;

public record ConsultantDTO
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}

public record CSuiteDTO(string Name, Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null, List<DirectorDTO>? DirectReports = default);
public record DirectorDTO(string Name, Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null, List<ManagerDTO>? DirectReports = default);
public record ManagerDTO(string Name, Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null, List<ConsultantDTO>? DirectReports = default);

public record HrPersonDTO(string Name, Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null, List<ConsultantDTO>? DirectReports = default);

public record ProgrammerDTO(string Name, Guid ID, string JobTitle, int Salary, DateTime StartDate, DateTime? EndDate = null, List<string>? Projects = default);


public static class EmployeeDTOMethods
{
    public static List<ConsultantDTO> Consultants { get; set; } = new();

    public static void GenerateConsultingList(int genCount)
    {
        var faker = new Faker<ConsultantDTO>()
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Age, f => f.Random.Number(20, 60))
            .RuleFor(c => c.Address, f => f.Address.FullAddress())
            .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(c => c.Email, f => f.Internet.Email());
        
        Consultants = faker.Generate(genCount);
    }
    
   
}
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using LanguageExt;
using OneOf;
using OneOf.Types;

namespace UnionContainers.Benchmarks;

[MemoryDiagnoser] 
[SimpleJob(RuntimeMoniker.Net80)]
[HideColumns(new []{"Job", "Error", "StdDev","Median", "RatioSD","Gen1","Gen2"})]
public class Benchmarks
{
    
    [GlobalSetup]
    public void Setup()
    {
        EmployeeDTOMethods.GenerateConsultingList(1000);
    }

    [Benchmark]
    public async Task<ConsultantDTO> TryGetConsultant()
    {
        try
        {
            var consultant = EmployeeDTOMethods.Consultants.FirstOrDefault(c => c.Name == "John");
            return consultant ?? new ConsultantDTO {Name = "No Consultant", Age = 0, Address = "No Address", PhoneNumber = "No Phone Number", Email = "No Email"};
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [Benchmark]
    public async Task<ConsultantDTO> TryGetConsultantOneOf()
    {
        var consultant = EmployeeDTOMethods.Consultants.FirstOrDefault(c => c.Name == "John");
        OneOf<ConsultantDTO?,None> oneof = consultant;
        return oneof.Match(
            c => c ?? new ConsultantDTO {Name = "Null Consultant", Age = 0, Address = "No Address", PhoneNumber = "No Phone Number", Email = "No Email"},
            None => new ConsultantDTO {Name = "No Consultant", Age = 0, Address = "No Address", PhoneNumber = "No Phone Number", Email = "No Email"});
    }
    
    [Benchmark]
    public async Task<ConsultantDTO> TryGetConsultantLangExt()
    {
        var consultant = EmployeeDTOMethods.Consultants.FirstOrDefault(c => c.Name == "John");
        Option<ConsultantDTO> optional = Prelude.Optional(consultant);
        return optional.Match(
            Some: c => c,
            None: () => new ConsultantDTO {Name = "No Consultant", Age = 0, Address = "No Address", PhoneNumber = "No Phone Number", Email = "No Email"});
    }
    
    [Benchmark]
    public async Task<ConsultantDTO> TryGetConsultantUnionCont()
    {
        var consultant = EmployeeDTOMethods.Consultants.FirstOrDefault(c => c.Name == "John");
        UnionContainer<ConsultantDTO> container = new(consultant); 
        return container.Match(
            onResult: c => c, 
            onNoResult: () => new ConsultantDTO {Name = "No Consultant", Age = 0, Address = "No Address", PhoneNumber = "No Phone Number", Email = "No Email"});
    }
    
    
}
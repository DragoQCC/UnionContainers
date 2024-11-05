using System.Net;
using DemoApp.Common;
using FluentAssertions;
using HelpfulTypesAndExtensions;

namespace UnionContainers.Tests.Unit.FromMethods;

public class UnionContainers_FromNonUserMethods
{
    [Test]
    public async Task NonUserMethodToContainer()
    {
        //arrange
        var client = new HttpClient();
        
        bool isException = false;
        bool isResult = false;
        bool isError = false;
        bool isNoResult = false;
        
        //act
        var container = await UnionContainer<HttpResponseMessage>.MethodToContainer(() => client.GetAsync("http://127.0.0.1:8080/"));
        
        container.Match
        (
            response =>
            {
                isResult = true;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    container.AddError($"Failed with error message {response.StatusCode}");
                }
            }, 
            () => isNoResult = true, 
            errors => isError = true, 
            exception => isException = true
        );
        
        //assert
        isResult.Should().BeFalse();
        isError.Should().BeFalse();
        isNoResult.Should().BeFalse();
        
        isException.Should().BeTrue();
    }
    
    
    
    [Test]
    public async Task NonUserMethodToContainerReturnMatch()
    {
        //arrange
        var client = new HttpClient();
        
        bool isException = false;
        bool isResult = false;
        bool isError = false;
        bool isNoResult = false;
        
        //act
        var httpRequestUnionContainer = await UnionContainer<HttpResponseMessage>.MethodToContainer(() => client.GetAsync("http://127.0.0.1:8080/"));
        
        HttpResponseMessage message = httpRequestUnionContainer.Match
        (
            response =>
            {
                isResult = true;
                return response;
            }, 
            () =>
            {
                isNoResult = true; 
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }, 
            errors =>
            {
                isError = true;
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }, 
            exception =>
            {
                isException = true;
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        );
        
        //assert
        isResult.Should().BeFalse();
        isError.Should().BeFalse();
        isNoResult.Should().BeFalse();
        message.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        isException.Should().BeTrue();
    }
}
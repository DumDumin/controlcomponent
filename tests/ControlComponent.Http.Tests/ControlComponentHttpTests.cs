using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace ControlComponent.Http.Tests
{
    public class ControlComponentHttpTests
    {
        Mock<HttpMessageHandler> httpMessageHandlerMock;
        HttpClient client;

        [SetUp]
        public void Setup()
        {
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            client = new HttpClient(httpMessageHandlerMock.Object);

            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                // TODO charset=windows-1252
                Content = new StringContent("STOPPED;", Encoding.UTF8, "text/plain")
            };

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7511/getVar?format=plain&path=/TechUnits/GSE/PA001/EXST.value");

            httpMessageHandlerMock.Protected().Setup<HttpResponseMessage>(
                "Send",
                ItExpr.Is<HttpRequestMessage>(h => h.RequestUri == request.RequestUri),
                ItExpr.IsAny<CancellationToken>()
            ).Returns(response).Verifiable();
        }

        [Test]
        public void Test1()
        {
            // curl -v "http://localhost:7511/setVar?path=/TechUnits/GSE/PA001.CMD&newvalue=Operator;OCCUPY;&format=plain"
            // ;

            // curl -v "http://localhost:7511/getVar?format=plain&path=/TechUnits/GSE/PA001/EXST.value"
            // IDLE;

            ControlComponentHttp cc = new ControlComponentHttp(client);
            
            Assert.AreEqual(ExecutionState.STOPPED, cc.EXST);
        }
    }
}
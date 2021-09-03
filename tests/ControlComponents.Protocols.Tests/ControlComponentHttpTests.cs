using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ControlComponents.Core;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace ControlComponents.Protocols.Tests
{
    public class ControlComponentHttpTests
    {
        Mock<HttpMessageHandler> httpMessageHandlerMock;
        HttpClient client;
        string baseUrl = "http://localhost:7511";
        string CC = "CC";
        string SENDER = "SENDER";

        [SetUp]
        public void Setup()
        {
            // Create a mockable http client
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            client = new HttpClient(httpMessageHandlerMock.Object);


        }

        private void ConfigureHttpClient(HttpResponseMessage response, HttpRequestMessage request)
        {
            httpMessageHandlerMock.Protected().Setup<HttpResponseMessage>(
                "Send",
                ItExpr.Is<HttpRequestMessage>(h => h.RequestUri == request.RequestUri),
                ItExpr.IsAny<CancellationToken>()
            ).Returns(response).Verifiable();
        }

        [Test]
        public void Test1()
        {
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("", Encoding.UTF8, "text/plain")
            };

            string path = $"/controlcomponent/{CC}/SENDER/OPERATIONS/RESET";
            var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + path);
            ConfigureHttpClient(response, request);

            ControlComponentHttp cc = new ControlComponentHttp(CC, baseUrl, client);
            cc.Reset(SENDER);

            cc.EXST.Should().Be(ExecutionState.RESETTING);
        }

        [Test]
        public void Test2()
        {
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("", Encoding.UTF8, "text/plain")
            };

            string path = $"/controlcomponent/{CC}/SENDER/OPERATIONS/RESET";
            var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + path);
            ConfigureHttpClient(response, request);

            ControlComponentHttp cc = new ControlComponentHttp(CC, baseUrl, client);
            cc.Reset(SENDER);

            cc.EXST.Should().Be(ExecutionState.RESETTING);
        }
    }
}
using System;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
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
        // IWebSocketFactory webSocketFactory;
        string baseUrl = "http://localhost:7511";
        string CC = "CC";
        string SENDER = "SENDER";

        [SetUp]
        public void Setup()
        {
            // Create a mockable http client
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            client = new HttpClient(httpMessageHandlerMock.Object);
            // webSocketFactory = new WebSocketFactory();    
        }

        private void ConfigureHttpClient(HttpResponseMessage response, HttpRequestMessage request)
        {
            httpMessageHandlerMock.Protected().Setup<HttpResponseMessage>(
                "Send",
                ItExpr.Is<HttpRequestMessage>(h => h.RequestUri == request.RequestUri),
                ItExpr.IsAny<CancellationToken>()
            ).Returns(response).Verifiable();
        }

        internal class WebSocketFactory : IWebSocketFactory
        {
            private readonly IWebSocket ws;

            public WebSocketFactory(IWebSocket ws)
            {
                this.ws = ws;
            }

            public IWebSocket CreateWebSocket()
            {
                return ws;
            }
        }

        [Test]
        public void When_Create_Then_Created()
        {
            Mock<IWebSocket> socket = new Mock<IWebSocket>();
            socket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.Delay(10).ContinueWith(t => new WebSocketReceiveResult(10, WebSocketMessageType.Text, true)));

            IWebSocketFactory webSocketFactory = new WebSocketFactory(socket.Object);
            ControlComponentHttp sut = new ControlComponentHttp(CC, baseUrl, client, webSocketFactory);
        }

        [Test]
        // TODO sometimes test fails, might be a timing issue
        public async Task When_Created_Then_SubscribedToEXST()
        {
            Mock<IWebSocket> socket = new Mock<IWebSocket>();
            var info = new ControlComponentInfo(ExecutionState.RESETTING, "NONE");
            var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<ControlComponentInfo>(info);

            socket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new WebSocketReceiveResult(0, WebSocketMessageType.Text, true)));
            IWebSocketFactory webSocketFactory = new WebSocketFactory(socket.Object);
            ControlComponentHttp sut = new ControlComponentHttp(CC, baseUrl, client, webSocketFactory);

            int i = 0;
            CancellationTokenSource t = new CancellationTokenSource();
            sut.OccupierChanged += (object sender, OccupationEventArgs e) => { i++; t.Cancel(); };
            sut.ExecutionStateChanged += (object sender, ExecutionStateEventArgs e) => { i++; t.Cancel(); };

            // activate the websocket after subscribing to events
            socket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                    .Callback((ArraySegment<byte> buffer, CancellationToken token) =>
                    {
                        bytes.CopyTo(buffer.Array, 0);
                    })
                    .Returns(Task.FromResult(new WebSocketReceiveResult(bytes.Length, WebSocketMessageType.Text, true)));
            
            // Wait for a message of the websocket
            await Task.Delay(5000, t.Token).ContinueWith(t => Task.Delay(1));
            i.Should().Be(1);
        }

        [Test]
        public async Task When_Created_Then_SubscribedToOccupierChanged()
        {
            Mock<IWebSocket> socket = new Mock<IWebSocket>();
            var info = new ControlComponentInfo(ExecutionState.STOPPED, SENDER);
            var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<ControlComponentInfo>(info);

            socket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new WebSocketReceiveResult(0, WebSocketMessageType.Text, true)));
            IWebSocketFactory webSocketFactory = new WebSocketFactory(socket.Object);
            ControlComponentHttp sut = new ControlComponentHttp(CC, baseUrl, client, webSocketFactory);

            int i = 0;
            CancellationTokenSource t = new CancellationTokenSource();
            sut.OccupierChanged += (object sender, OccupationEventArgs e) => { i++; t.Cancel(); };
            sut.ExecutionStateChanged += (object sender, ExecutionStateEventArgs e) => { i++; t.Cancel(); };

            // activate the websocket after subscribing to events
            socket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                    .Callback((ArraySegment<byte> buffer, CancellationToken token) =>
                    {
                        bytes.CopyTo(buffer.Array, 0);
                    })
                    .Returns(Task.FromResult(new WebSocketReceiveResult(bytes.Length, WebSocketMessageType.Text, true)));
            
            // Wait for a message of the websocket
            await Task.Delay(1000, t.Token).ContinueWith(t => Task.Delay(1));
            i.Should().Be(1);
        }

        [Test]
        public void When_Reset_Then_Resetting()
        {
            Mock<IWebSocket> socket = new Mock<IWebSocket>();
            var info = new ControlComponentInfo(ExecutionState.RESETTING, SENDER);
            var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<ControlComponentInfo>(info);
            // activate the websocket after subscribing to events
            socket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                    .Callback((ArraySegment<byte> buffer, CancellationToken token) =>
                    {
                        bytes.CopyTo(buffer.Array, 0);
                    })
                    .Returns(Task.FromResult(new WebSocketReceiveResult(bytes.Length, WebSocketMessageType.Text, true)));

            IWebSocketFactory webSocketFactory = new WebSocketFactory(socket.Object);

            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("", Encoding.UTF8, "text/plain")
            };

            string path = $"/controlcomponent/{CC}/{SENDER}/OPERATIONS/RESET";
            var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + path);
            ConfigureHttpClient(response, request);

            ControlComponentHttp cc = new ControlComponentHttp(CC, baseUrl, client, webSocketFactory);
            cc.Reset(SENDER);

            cc.EXST.Should().Be(ExecutionState.RESETTING);
        }
    }
}
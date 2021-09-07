using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ControlComponents.Protocols
{
    public interface IWebSocket
    {
        Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);
    }
}
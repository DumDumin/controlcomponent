using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;
using ControlComponents.Core;

namespace ControlComponents.Protocols
{
    public class ControlComponentInfo
    {
        public ExecutionState EXST { get; }
        public string OCCUPIER { get; }

        public ControlComponentInfo(ExecutionState eXST, string oCCUPIER)
        {
            EXST = eXST;
            OCCUPIER = oCCUPIER;
        }
    }

    public class ControlComponentHttp : IControlComponent
    {
        private readonly string name;
        private readonly string baseUrl;
        private readonly HttpClient client;
        private readonly IWebSocketFactory webSocketFactory;

        public event OperationModeEventHandler OperationModeChanged;
        public event ExecutionStateEventHandler ExecutionStateChanged;
        public event ExecutionModeEventHandler ExecutionModeChanged;
        public event OccupationEventHandler OccupierChanged;

        public string OpModeName => throw new NotImplementedException();

        public string WORKST => throw new NotImplementedException();

        public ICollection<string> OpModes => throw new NotImplementedException();

        public ICollection<string> Roles => throw new NotImplementedException();

        public string ComponentName => throw new NotImplementedException();

        public ExecutionState EXST { get; } = ExecutionState.STOPPED;

        public ExecutionMode EXMODE => throw new NotImplementedException();

        public string OCCUPIER { get; } = "NONE";

        // public ExecutionState EXST
        // {
        //     get
        //     {
        //         HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, REQUEST_EXST);
        //         string response = client.Send(request).Content.ReadAsStringAsync().Result;
        //         string exst = response.Split(';')[0];
        //         return (ExecutionState)Enum.Parse(typeof(ExecutionState), exst);
        //     }
        // }

        // https://stackoverflow.com/questions/50813851/clientwebsocket-and-unit-tests
        public ControlComponentHttp(string name, string baseUrl, HttpClient client, IWebSocketFactory webSocketFactory)
        {
            this.name = name;
            this.baseUrl = baseUrl;
            this.client = client;
            this.webSocketFactory = webSocketFactory;

            IWebSocket ws = webSocketFactory.CreateWebSocket();
            Task read = ReadWebSocket(ws);
        }



        private async Task ReadWebSocket(IWebSocket ws)
        {
            // ws.ConnectAsync()
            try
            {
                while (true)
                {


                    var array = new byte[100];
                    var buffer = new ArraySegment<byte>(array);
                    var result = await ws.ReceiveAsync(buffer, new System.Threading.CancellationToken());
                    if (result.Count > 0 && result.MessageType == WebSocketMessageType.Text)
                    {

                        string json = System.Text.Encoding.Default.GetString(buffer.Array, 0, result.Count).TrimEnd();
                        ControlComponentInfo info = System.Text.Json.JsonSerializer.Deserialize<ControlComponentInfo>(json);
                        if (info.EXST != EXST)
                        {
                            ExecutionStateChanged?.Invoke(this, new ExecutionStateEventArgs(EXST));
                        }
                        if (info.OCCUPIER != OCCUPIER)
                        {
                            OccupierChanged?.Invoke(this, new OccupationEventArgs(OCCUPIER));
                        }
                    }
                    await Task.Delay(1);
                }
            }
            catch (System.Exception e)
            {

                throw e;
            }
        }

        public void Reset(string sender)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseUrl + $"/controlcomponent/{name}/{sender}/OPERATIONS/RESET");
            string response = client.Send(request).Content.ReadAsStringAsync().Result;
            // string exst = response.Split(';')[0];
            // return (ExecutionState)Enum.Parse(typeof(ExecutionState), exst);
        }

        public void Start(string sender)
        {
            throw new NotImplementedException();
        }

        public void Suspend(string sender)
        {
            throw new NotImplementedException();
        }

        public void Unsuspend(string sender)
        {
            throw new NotImplementedException();
        }

        public void Stop(string sender)
        {
            throw new NotImplementedException();
        }

        public void Hold(string sender)
        {
            throw new NotImplementedException();
        }

        public void Unhold(string sender)
        {
            throw new NotImplementedException();
        }

        public void Abort(string sender)
        {
            throw new NotImplementedException();
        }

        public void Clear(string sender)
        {
            throw new NotImplementedException();
        }

        public void Auto(string sender)
        {
            throw new NotImplementedException();
        }

        public void SemiAuto(string sender)
        {
            throw new NotImplementedException();
        }

        public Task SelectOperationMode(string operationMode)
        {
            throw new NotImplementedException();
        }

        public Task DeselectOperationMode()
        {
            throw new NotImplementedException();
        }

        public bool ChangeOutput(string role, string id)
        {
            throw new NotImplementedException();
        }

        public void Occupy(string sender)
        {
            throw new NotImplementedException();
        }

        public void Free(string sender)
        {
            throw new NotImplementedException();
        }

        public void Prio(string sender)
        {
            throw new NotImplementedException();
        }

        public bool IsOccupied()
        {
            throw new NotImplementedException();
        }

        public bool IsFree()
        {
            throw new NotImplementedException();
        }

        public bool IsUsableBy(string id)
        {
            throw new NotImplementedException();
        }

        public TReturn ReadProperty<TReturn>(string targetRole, string propertyName)
        {
            throw new NotImplementedException();
        }

        public void CallMethod(string targetRole, string methodName)
        {
            throw new NotImplementedException();
        }

        public void CallMethod<TParam>(string targetRole, string methodName, TParam param)
        {
            throw new NotImplementedException();
        }

        public TReturn CallMethod<TReturn>(string targetRole, string methodName)
        {
            throw new NotImplementedException();
        }

        public TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param)
        {
            throw new NotImplementedException();
        }

        public TReturn CallMethod<TParam1, TParam2, TReturn>(string targetRole, string methodName, TParam1 param1, TParam2 param2)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>(string targetRole, string eventName, T eventHandler)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T>(string targetRole, string eventName, T eventHandler)
        {
            throw new NotImplementedException();
        }
    }
}

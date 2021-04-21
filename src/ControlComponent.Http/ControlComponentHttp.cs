using System;
using System.Net.Http;
using ControlComponent;

namespace ControlComponent.Http
{
    public class ControlComponentHttp
    {
        private readonly HttpClient client;

        private readonly string REQUEST_EXST = "http://localhost:7511/getVar?format=plain&path=/TechUnits/GSE/PA001/EXST.value";

        public ExecutionState EXST
        {
            get
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, REQUEST_EXST);
                string response = client.Send(request).Content.ReadAsStringAsync().Result;
                string exst = response.Split(';')[0];
                return (ExecutionState)Enum.Parse(typeof(ExecutionState), exst);
            }
        }

        public ControlComponentHttp(HttpClient client)
        {
            this.client = client;
        }
    }
}

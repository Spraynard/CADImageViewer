using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections;
using System.Web.Script.Serialization;

namespace CADImageViewer.Classes
{
    public class Base
    {
        public string Name { get; set; }
    }

    public class Note
    {

    }

    public class Installation
    {
    }

    public class APIHandler
    {
        HttpClient Client { get; set; }

        private async Task<List<Base>> DeserializeData( HttpResponseMessage response )
        {
            string data = await response.Content.ReadAsStringAsync();

            JavaScriptSerializer JSerialize = new JavaScriptSerializer();

            return JSerialize.Deserialize<List<Base>>(data);
        }

        public APIHandler(string hostname)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(hostname);
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Base>> GetPrograms()
        {

            return await DeserializeData(await Client.GetAsync("api/program"));
        }

        public async Task<List<Base>> GetTrucks(string program)
        {
            return await DeserializeData(await Client.GetAsync(String.Format("api/truck/{0}", program)));
        }

        public async Task<List<Base>> GetDRE(string program, string truck)
        {
            return await DeserializeData(await Client.GetAsync(String.Format("api/dre/{0}/{1}", program, truck)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CADImageViewer.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CADImageViewerTests
{
    [TestClass]
    public class ApiTest
    {
        APIHandler API { get; set; }

        public ApiTest()
        {
            API = new APIHandler(Properties.TestSettings.Default.hostname);
        }

        [TestMethod]
        public async Task ProgramsEndpoint()
        {
            List<Base> list = await API.GetPrograms();
            
            foreach( Base item in list )
            {
                Console.WriteLine(item.Name);
            }

            Assert.IsNotNull(list);
        }

        [TestMethod]
        public async Task TrucksEndpoint()
        {
            string invalidProgram = "20";
            string validProgram = "E57";

            List<Base> list = await API.GetTrucks(invalidProgram);

            foreach( Base item in list)
            {
                Console.WriteLine(item.Name);
            }

            Assert.AreEqual(list.Count, 0);

            List<Base> list2 = await API.GetTrucks(validProgram);

            foreach( Base item in list2 )
            {
                Console.WriteLine(item.Name);
            }

            Assert.AreNotEqual(list2.Count, 0);
            Assert.IsNotNull(list2);
        }
    }
}

using System;
using Microsoft.AspNetCore.Mvc;
using Korelight.Portal.Models;
using Korelight.XAPI;
using System.IO;
using Ionic.Zip;

namespace Korelight.Portal.Controllers
{
    /// <summary>
    /// Test controller for demo methods.
    /// </summary>
    [Route("[controller]")]
    public class TestController : Controller
    {
        /// <summary>
        /// Test GET API call.
        /// </summary>
        /// <returns></returns>
        [HttpGet("TestGET")]
        public TestModel TestGET()
        {
            var result = new TestModel();

            result.name = "Base XAPI Test Model";
            result.title = "Korelight.XPortal.Models.TestModel";

            return result;
        }

        /// <summary>
        /// Test POST api call.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("TestPOST")]
        public TestModel TestPOST([FromBody]TestModel model)
        {
            if (model != null)
            {
                model.name = "POST XAPI Test Model";
                return model;
            }
            else
            {
                return TestGET();
            }
        }

        /// <summary>
        /// Test PUT api call.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("TestPut")]
        [HttpPut("TestPUT")]
        public TestModel TestPUT([FromBody]TestModel model)
        {
            if (model != null)
            {
                model.name = "PUT XAPI Test Model";
                return model;
            }
            else
            {
                return TestGET();
            }
        }

        /// <summary>
        /// Test DELETE api call.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("TestDELETE")]
        public Object TestDELETE([FromBody]TestModel model)
        {
            return new { result = "DELETE successful" };
        }

        /// <summary>
        /// Return a JavaScript API for the Test Controller.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api")]
        public System.IO.Stream XAPI(string id)
        {
            Response.ContentType = "application/text";
            // Get the engine for producing an API for the TestController type, and supply it an endpoint of test.
            var TestServiceEngine = new Engine<TestController>("test");

            System.IO.Stream result = null;

            switch (id)
            {
                case "JQUERY":
                    // Get a stream of the JavaScript API for the controller.
                    result = TestServiceEngine.GetAPI(SupportedLanguages.JQUERY);
                    break;

                case "SWIFT":
                    // Get a stream of the SWIFT API for the controller.
                    result = TestServiceEngine.GetAPI(Korelight.XAPI.SupportedLanguages.SWIFT);
                    break;

                case "ANGULAR":
                    result = TestServiceEngine.GetAPI(Korelight.XAPI.SupportedLanguages.ANGULAR6);
                    break;
            }

            // Return the resulting JavaScript object.
            return result;
        }

        /// <summary>
        /// Return a JavaScript API for the Test Controller.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("package")]
        public System.IO.Stream Package(string id)
        {
            //Response.ContentType = "application/zip";
            // Get the engine for producing an API for the TestController type, and supply it an endpoint of test.
            var TestServiceEngine = new Engine<TestController>("test");

            MemoryStream result = new MemoryStream();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (ZipFile zip = new ZipFile())
            {
                switch (id)
                {
                    case "JQUERY":
                        // Get a stream of the JavaScript API for the controller.


                        break;

                    case "SWIFT":
                        // Get a stream of the SWIFT API for the controller.

                        break;

                    case "ANGULAR":

                        foreach (var inter in TestServiceEngine.GetInterfaces(Korelight.XAPI.SupportedLanguages.ANGULAR6, new[] { typeof(TestModel) }))
                        {
                            zip.AddEntry(inter.Item1.Name, inter.Item2.ToArray());
                        }

                        zip.AddEntry("API.ts", TestServiceEngine.GetAPI(Korelight.XAPI.SupportedLanguages.ANGULAR6));
                        foreach (var d in TestServiceEngine.GetSupporGetCoreServiceClientstingClasses(Korelight.XAPI.SupportedLanguages.ANGULAR6))
                        {
                            zip.AddEntry(d.Item1.Name, d.Item2);
                        }

                        // zip.AddEntry("Core.ts",  TestServiceEngine.Get(Korelight.XAPI.SupportedLanguages.ANGULAR6));
                        break;
                }

                zip.Name = "Interfaces.zip";
                zip.Save(result);

                result.Position = 0;
            }

            return result;
        }
    }
}

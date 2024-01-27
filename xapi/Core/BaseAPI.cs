using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Korelight.XAPI.Core
{
  /// <summary>
  /// Base API generation class, language generic.
  /// </summary>
  public abstract class XCore<ControllerType>
  {
    /// <summary>
    /// Base method for returning an API stream.
    /// </summary>
    /// <param name="servicetype">The .NET type to generate an API client for.</param>
    /// <param name="controller">The name of the controller to use for accessing the target service.</param>
    /// <returns>A stream consisting of the generated code.</returns>
    public abstract Stream GetAPI(String controller);

		/// <summary>
		/// Base method for creating interfaces for a list of types.
		/// </summary>
		/// <param name="targettypes"></param>
		/// <returns></returns>
		public abstract List<Tuple<FileInfo, MemoryStream>> GetInterfaces(Type[] targettypes);

	  /// <summary>
		/// Base method for creating core services.
		/// </summary>
		/// <param name="targettypes"></param>
		/// <returns></returns>
    public abstract List<Tuple<FileInfo, MemoryStream>> GetCoreServiceClients();

		/// <summary>
		/// Get the CallDocumentation for the supplied type.
		/// </summary>
		/// <param name="qualifiedname">This should be the fully qualified (string) name and type of the WCF service that documentation is required for.</param>
		/// <returns>A list of Method Documentation objects for the given type.</returns>        
		public List<MethodDocumentation> GetTypeDocumentation()
    {
      List<MethodDocumentation> result = new List<MethodDocumentation>();

      foreach (var methodcall in typeof(ControllerType).GetTypeInfo().GetMethods())
      {
        MethodDocumentation newcalldoc = new MethodDocumentation();
        Boolean ContractCall = false;

        newcalldoc.MethodName = methodcall.Name;
        newcalldoc.MethodReturnType = methodcall.ReturnType;

        foreach (var parameter in methodcall.GetParameters())
        {
          MethodDocumentation.Parameter newp = new MethodDocumentation.Parameter();
          newp.ParameterDataType = parameter.ParameterType;
          newp.ParameterName = parameter.Name;

          newcalldoc.MethodParameters.Add(newp);
        }

        foreach (var attrib in methodcall.GetCustomAttributes())
        {
          newcalldoc.MethodAttributes.Add(attrib);

          if (attrib.GetType().GetTypeInfo().BaseType == typeof(HttpMethodAttribute))
          {
            ContractCall = true;
            foreach (var s in ((HttpMethodAttribute)attrib).HttpMethods)
            {
              newcalldoc.HttpActionType = s;
            }
          }
        }

        if (ContractCall)
        {
          result.Add(newcalldoc);
        }
      }

      return result;
    }

    /// <summary>
    /// Generate documentation for the supplied type.
    /// </summary>
    /// <returns></returns>
    public Stream GenerateDocumentation()
    {
      String pagedata = new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("Korelight.XAPI.Documentation.html")).ReadToEnd();
      StringBuilder documentation = new StringBuilder();
      documentation.Append("<ul class='MethodList'>");
      foreach (var doc in GetTypeDocumentation())
      {
        documentation.Append("<li class='MethodName'>");
        documentation.Append(doc.MethodName);

        if (doc.MethodParameters.Count > 0)
        {
          documentation.Append("<ul class='ParameterList'>");
          foreach (var param in doc.MethodParameters)
          {
            documentation.Append("<li class='ParameterName'>");
            documentation.Append(param.ParameterName);
            documentation.Append("</li>");

            documentation.Append("<li class='ParameterType'>");
            documentation.Append(param.ParameterDataType);
            documentation.Append("</li>");
          }
          documentation.Append("</ul>");
        }

        documentation.Append("</li>");
      }
      documentation.Append("</ul>");
      return new MemoryStream(UTF8Encoding.ASCII.GetBytes(pagedata.Replace("<!--BODY-->", documentation.ToString())));
    }
  }
}
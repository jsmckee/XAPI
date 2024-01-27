using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace Korelight.XAPI
{
	/// <summary>
	/// The base class for generating API clients for .NET service types.
	/// </summary>
	public class Engine<ControllerType>
		where ControllerType : Controller
	{
		/// <summary>
		/// The web controller name to use for the generated API clients.
		/// </summary>
		private String _ControllerName { get; set; }

		/// <summary>
		/// Create a new XAPI for the provided type and controller name.
		/// </summary>
		/// <param name="servicetype">The .NET Type to generate an API client for.</param>
		/// <param name="contollername"></param>
		public Engine(String contollername)
		{
			_ControllerName = contollername;
		}

		/// <summary>
		/// Return an API client for the specified language.
		/// </summary>
		/// <param name="clientlanguage">The language to generate the API client in.</param>
		/// <returns>A stream result with the generated code.</returns>
		public Stream GetAPI(SupportedLanguages clientlanguage)
		{
			Stream result = null;

			switch (clientlanguage)
			{
				case SupportedLanguages.JQUERY:
					result = new JQueryApi<ControllerType>().GetAPI(_ControllerName);
					break;

				case SupportedLanguages.SWIFT:
					result = new SwiftAPI<ControllerType>().GetAPI(_ControllerName);
					break;

				case SupportedLanguages.JAVA:
					break;

				case SupportedLanguages.ANGULAR6:
					result = new TypeScriptApi<ControllerType>().GetAPI(_ControllerName);
					break;

			}

			return result;
		}

		public List<Tuple<FileInfo, MemoryStream>> GetInterfaces(SupportedLanguages clientlanguage, Type[] targettypes)
		{
			List<Tuple<FileInfo, MemoryStream>> result = null;

			switch (clientlanguage)
			{
				case SupportedLanguages.JQUERY:
					result = new JQueryApi<ControllerType>().GetInterfaces(targettypes);
					break;

				case SupportedLanguages.SWIFT:
					result = new SwiftAPI<ControllerType>().GetInterfaces(targettypes);
					break;

				case SupportedLanguages.JAVA:
					break;

				case SupportedLanguages.ANGULAR6:
					result = new TypeScriptApi<ControllerType>().GetInterfaces(targettypes);
					break;

			}

			return result;
		}

		public List<Tuple<FileInfo, MemoryStream>> GetSupporGetCoreServiceClientstingClasses(SupportedLanguages clientlanguage)
		{
			List<Tuple<FileInfo, MemoryStream>> result = null;

			switch (clientlanguage)
			{
				case SupportedLanguages.JQUERY:
					result = new JQueryApi<ControllerType>().GetCoreServiceClients();
					break;

				case SupportedLanguages.SWIFT:
					result = new SwiftAPI<ControllerType>().GetCoreServiceClients();
					break;

				case SupportedLanguages.JAVA:
					break;

				case SupportedLanguages.ANGULAR6:
					result = new TypeScriptApi<ControllerType>().GetCoreServiceClients();
					break;

			}

			return result;
		}

		/// <summary>
		/// Return a static web page for documenting the type the XAPI was generated for.
		/// </summary>
		/// <returns>A stream representing the markup page for documenting the supplied type.</returns>
		public Stream GetDocumentation()
		{
			Stream result = null;

			result = new JQueryApi<ControllerType>().GenerateDocumentation();


			return result;
		}
	}
}

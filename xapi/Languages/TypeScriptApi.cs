using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;

using System.Linq;

using Korelight.XAPI.Core;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using Ionic.Zip;

namespace Korelight.XAPI
{
    /// <summary>
    /// Typescript API generator.
    /// </summary>
    public class TypeScriptApi<ControllerType> : XCore<ControllerType>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TypeScriptApi() : base() { }

        public override List<Tuple<FileInfo, MemoryStream>> GetCoreServiceClients()
        {
            List<Tuple<FileInfo, MemoryStream>> result = new List<Tuple<FileInfo, MemoryStream>>();
            //var core = new Tuple<FileInfo, MemoryStream>();
            // dvar auth = new Tuple<FileInfo, MemoryStream>();


            var coreservice = new MemoryStream();
            Assembly.GetAssembly(this.GetType()).GetManifestResourceStream("Korelight.XAPI.Templates.CoreService.tscript").CopyTo(coreservice);

            var authservice = new MemoryStream();
            Assembly.GetAssembly(this.GetType()).GetManifestResourceStream("Korelight.XAPI.Templates.AuthorizedService.tscript").CopyTo(authservice);

            coreservice.Position = 0;
            authservice.Position = 0;

            result.Add(new Tuple<FileInfo, MemoryStream>(new FileInfo("CoreService.ts"), coreservice));

            result.Add(new Tuple<FileInfo, MemoryStream>(new FileInfo("AuthorizedService.ts"), authservice));
            return result;
        }

        /// <summary>
        /// Get the housing assembly for the supplied type.
        /// </summary>
        /// <typeparam name="TargetType"></typeparam>
        /// <returns></returns>
        private Assembly GetAssemblyByType<TargetType>()
        {
            return Assembly.GetAssembly(typeof(TargetType));
        }

        /// <summary>
        /// Verify that the supplied type belongs to the supplied namespace.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="targetnamespace"></param>
        /// <returns></returns>
        private bool IsTargetNameSpace(Type t, String targetnamespace)
        {
            return (t.Namespace.Contains(targetnamespace) && !t.IsDefined(typeof(CompilerGeneratedAttribute)) && !t.GenericTypeArguments.Any());
        }


        public Dictionary<String, MemoryStream> GenerateTypeScriptClasses<RootAssemblyType>(String targetnamespace)
        {
            Dictionary<String, MemoryStream> result = new Dictionary<String, MemoryStream>();

            foreach (Type target in (from Type t in GetAssemblyByType<RootAssemblyType>().GetTypes() where IsTargetNameSpace(t, targetnamespace) select t))
            {
                var generateddata = CreateTypeScriptInterfaceForType(target);

                result.Add(generateddata.Item1.Name, generateddata.Item2);
            }

            return result;
        }

        /// <summary>
        /// Create a TypeScript name for a .NET Type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string GetTypeScriptType(Type t)
        {
            string result = "string";

            if (t.GenericTypeArguments.Any())
            {
                t = t.GenericTypeArguments[0];
            }

            switch (t.Name)
            {
                case "Decimal":
                case "Double":
                case "Int32":
                case "Int64":
                case "int":
                    result = "number";

                    break;

                case "Byte":
                case "Boolean":
                case "bool":
                    result = "boolean";
                    break;
            }


            return result;
        }

        /// <summary>
        /// Create a CSharp safe string for a property name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetSafeName(string name)
        {
            string result = name;

            //switch (name.ToLower())
            //{
            //	case "operator":
            //	case "checked":
            //	case "default":
            //	case "class":
            //	case "readonly":
            //	case "base":
            //		result = "@" + name;
            //		break;
            //}

            return result.ToLower();
        }

        /// <summary>
        /// Create a typescript interface text file and file info object for the given type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Tuple<FileInfo, MemoryStream> CreateTypeScriptInterfaceForType(Type t)
        {
            FileInfo info = new FileInfo($"{t.Name}.ts");
            MemoryStream data;

            StringBuilder typescriptclass = new StringBuilder();
            typescriptclass.Append($"export interface {t.Name} {{");
            typescriptclass.Append(Environment.NewLine);

            foreach (PropertyInfo pi in t.GetProperties())
            {
                typescriptclass.Append($"\t{GetSafeName(pi.Name)}: {GetTypeScriptType(pi.PropertyType)};");
                typescriptclass.Append(Environment.NewLine);
            }

            typescriptclass.Append("}");
            typescriptclass.Append(Environment.NewLine);

            data = new MemoryStream(Encoding.UTF8.GetBytes(typescriptclass.ToString()));

            return new Tuple<FileInfo, MemoryStream>(info, data);
        }

        public override List<Tuple<FileInfo, MemoryStream>> GetInterfaces(Type[] targettypes)
        {
            List<Tuple<FileInfo, MemoryStream>> result = new List<Tuple<FileInfo, MemoryStream>>();
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            foreach (var d in targettypes)
            {
                result.Add(CreateTypeScriptInterfaceForType(d));
            }

            return result;
        }

        /// <summary>
        /// Get a runtime  API for a given service Type.
        /// </summary>
        /// <param name="servicetype"></param>
        /// <returns></returns>
        public override Stream GetAPI(String controller)
        {
            StringBuilder api = new StringBuilder();

            api.Append("/* TypeScript Angular API Generated by Korelight XAPI.");
            api.Append("\n");
            api.Append("http://www.Korelight.com ");
            api.Append(Environment.NewLine);
            api.Append("Jeremy McKee");
            api.Append(Environment.NewLine);
            api.Append("Timestamp: ");
            api.Append(DateTime.UtcNow.ToString());
            api.Append(" */");
            api.Append(Environment.NewLine);
            api.Append(Environment.NewLine);


            api.Append("import { Injectable } from '@angular/core';");
            api.Append(Environment.NewLine);
            api.Append("import { CoreService } from './core.service'; ");
            api.Append(Environment.NewLine);
            api.Append("import { Observable } from 'rxjs/Observable'; ");
            api.Append(Environment.NewLine);
            api.Append(Environment.NewLine);

            api.Append("@Injectable()");
            api.Append(Environment.NewLine);
            api.Append($"export class {controller}Service extends CoreService  {{");
            api.Append(Environment.NewLine);

            foreach (MethodDocumentation doc in GetTypeDocumentation())
            {
                StringBuilder data = new StringBuilder();

                 var endpoint = doc.MethodName;

                RouteAttribute route = null;
                foreach (Attribute a in doc.MethodAttributes)
                {
                    if (a as RouteAttribute != null)
                    {
                        route = a as RouteAttribute;
                    }
                }

                if (route != null)
                {
                    endpoint = route.Template;
                }


                if (doc.MethodParameters.Any())
                {
                    api.Append($"\tpublic {doc.MethodName}(request: {doc.MethodParameters[0].ParameterDataType.Name}) {{");
                    api.Append(Environment.NewLine);
                    api.Append($"\t\treturn this.PostRequest<{doc.MethodReturnType.Name}, {doc.MethodParameters[0].ParameterDataType.Name}>(request, '');");

                    api.Append(Environment.NewLine);
                }
                else
                {
                    api.Append($"\tpublic {doc.MethodName}() {{");
                    api.Append(Environment.NewLine);
                    api.Append($"\t\treturn this.PostRequest<{doc.MethodReturnType.Name}, Object>({{}}, '{endpoint}');");
                    api.Append(Environment.NewLine);
                }

                api.Append("}");
                api.Append(Environment.NewLine);
                api.Append(Environment.NewLine);
            }

            api.Append("}");

            MemoryStream result = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(api.ToString()));

            return result;
        }
    }
}




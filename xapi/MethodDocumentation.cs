using System;
using System.Collections.Generic;

namespace Korelight.XAPI
{
    /// <summary>
    /// The class model for holding method attributes that are required for generating a client API.
    /// </summary>
    public class MethodDocumentation
    {
        /// <summary>
        /// The name of the method.
        /// </summary>
        public String MethodName { get; set; }

        /// <summary>
        /// The HTTP action type of the method (GET/POST/PUT/DELETE).
        /// </summary>
        public String HttpActionType { get; set; }

        /// <summary>
        /// A list of parameters for the method.
        /// </summary>
        public List<Parameter> MethodParameters { get; set; }

        /// <summary>
        /// A list of attributes for the method.
        /// </summary>
        public List<Attribute> MethodAttributes { get; set; }

        /// <summary>
        /// The type returned by the method.
        /// </summary>
        public Type MethodReturnType { get; set; }

        /// <summary>
        /// Container class for parameter name and type.
        /// </summary>
        public class Parameter
        {
            public String ParameterName { get; set; }
            public Type ParameterDataType { get; set; }
        }

        /// <summary>
        /// Instantiate a new method documentation object.
        /// </summary>
        public MethodDocumentation()
        {
            MethodParameters = new List<Parameter>();
            MethodAttributes = new List<Attribute>();
        }
    }
}

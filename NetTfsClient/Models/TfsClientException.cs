using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models
{
    /// <summary>
    /// ClientException contains information about raised exception
    /// </summary>
    [Serializable]
    public class ClientException : Exception
    {
        public ClientException()
            : base()
        { }

        public ClientException(string message)
            : base(message)
        { }

        public ClientException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}

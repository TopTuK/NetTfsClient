using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models
{
    [Serializable]
    public class TfsClientException : Exception
    {
        public TfsClientException()
            : base()
        {

        }

        public TfsClientException(string message)
            : base(message)
        {

        }

        public TfsClientException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}

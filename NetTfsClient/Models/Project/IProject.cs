using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTfsClient.Models.Project
{
    public interface IProject
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
    }
}

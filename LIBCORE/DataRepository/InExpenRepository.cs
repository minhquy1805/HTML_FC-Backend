using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.DataRepository
{
    public partial class InExpenRepository : IInExpenRepository
    {
        string IInExpenRepository.ExampleRepositoryInExpen()
        {
            return "Implementation for the ExampleRepositoryInExpen() located in the InExpenRepository directly under the DataRepository folder";
        }
    }
}

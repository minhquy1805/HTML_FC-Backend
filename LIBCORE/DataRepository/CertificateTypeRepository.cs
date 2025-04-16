using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBCORE.DataRepository
{
    public partial class CertificateTypeRepository : ICertificateTypeRepository
    {
        string ICertificateTypeRepository.ExampleRepositoryMember()
        {
            return "Implementation for the ExampleRepositoryCertificateType() located in the INewsRepository directly under the DataRepository folder";
        }
    }
}

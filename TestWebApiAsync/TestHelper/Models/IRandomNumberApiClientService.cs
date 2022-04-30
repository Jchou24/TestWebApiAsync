using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestUtility.Models
{
    public interface IRandomNumberApiClientService
    {
        Task<ServiceResult> GetNumberAsync();

        ServiceResult GetNumber();
    }
}

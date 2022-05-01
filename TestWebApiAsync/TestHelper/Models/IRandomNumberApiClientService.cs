using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestUtility.Models
{
    public interface IRandomNumberApiClientService
    {
        Task<ServiceResult> GetNumberAsync(int? waitTime = null);

        ServiceResult GetNumber(int? waitTime = null);
    }
}

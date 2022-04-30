using System;

namespace TestUtility.Models
{
    public class ApiResult: DateTimeRecord
    {
        public int Number { get; set; }

        public ApiResult(){}

        public ApiResult(DateTime startDateTime): base(startDateTime)
        {
            var random = new Random();
            Number = random.Next(1000);
        }        
    }
}

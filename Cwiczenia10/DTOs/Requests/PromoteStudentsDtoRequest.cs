using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cwiczenia10.DTOs.Requests
{
    public class PromoteStudentsDtoRequest
    {
        public string Studies { get; set; }
        public int Semester { get; set; }
    }
}

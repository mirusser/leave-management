using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Extensions
{
    public static class MvcHelper
    {
        public static string NameOfController<T>()
            where T : Controller
            => typeof(T).Name.Replace("Controller", string.Empty);
        
    }
}

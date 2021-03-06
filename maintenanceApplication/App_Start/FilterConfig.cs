﻿using System.Web;
using System.Web.Mvc;

namespace maintenanceApplication
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new ValidateInputAttribute(true));
            //filters.Add(new RequireHttpsAttribute()); 
        }
    }
}

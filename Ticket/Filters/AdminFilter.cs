﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ticket.Filters
{
    public class AdminFilter : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["Admin"] == null)
            {
                filterContext.Result = new RedirectResult("/SignIn");
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Ticket.Models;
using Ticket.Models.Context;

namespace Ticket.Filters
{
    public class LogFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string username = filterContext.HttpContext.Session["Login"] == null ? string.Empty : filterContext.HttpContext.Session["Login"].ToString();
            DatabaseContext db = new DatabaseContext();
            Log log = new Log
            {
                IP = filterContext.HttpContext.Request.UserHostAddress,
                Time = DateTime.Now,
                Type = filterContext.HttpContext.Request.HttpMethod,
                routevalues = filterContext.HttpContext.Request.Url.PathAndQuery,
                Users = username.IsEmpty()
                ? null
                : db.Users.FirstOrDefault(x => x.Username == username && !x.IsDeleted)
            };
            db.Logs.Add(log);
            db.SaveChanges();
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WePrint.Data;
using WePrint.Models;

namespace WePrint
{
    public static class ReactRouter
    {
        public static string GetReactUrl(this IUrlHelper url, IIdentifiable<Guid> obj)
        {
            switch (obj)
            {
                case organization org:
                    return "/organization/" + org.Id;
                
                case project proj:
                    return "/project/" + proj.Id;
                
                default:
                    return "#";
            }
        }
    }
}

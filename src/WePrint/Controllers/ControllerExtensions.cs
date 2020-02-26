using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WePrint.Models;

namespace WePrint.Controllers
{
    public static class ControllerExtensions
    {
        public static async Task<IActionResult> QueryItemById<TItem>(this Controller controller, IAsyncDocumentSession session, string id)
            where TItem: IDbModel
        {
            var results = await session.Query<TItem>().Where(t => t.Id == id).ToArrayAsync();

            if (results.Length == 0)
                return controller.FailWith("Id " + id + " not found", HttpStatusCode.NotFound);
            if (results.Length > 1)
                return controller.FailWith("More than item matched this id", HttpStatusCode.Conflict);

            return controller.Json(results[0]);
        }

        public static async Task<ApplicationUser> GetCurrentUser(this Controller controller, IAsyncDocumentSession session)
        {
            var identity = controller.HttpContext.User.Identity;
            if (!identity.IsAuthenticated)
                return null;
            var queryResult = await session.Query<ApplicationUser>().Where(t => t.Email == identity.Name).ToArrayAsync();
            if (queryResult.Length == 0)
                return null;
            if (queryResult.Length > 1)
                throw new InvalidOperationException("More than one user possible");
            return queryResult[0];
        }

        public static IActionResult FailWith(this Controller controller, string message, HttpStatusCode responseCode = HttpStatusCode.InternalServerError)
        {
            controller.HttpContext.Response.StatusCode = (int)responseCode;
            return controller.Json(new { err = message });
        }
        public static IActionResult FailWith(this Controller controller, object response, HttpStatusCode responseCode = HttpStatusCode.InternalServerError)
        {
            controller.HttpContext.Response.StatusCode = (int)responseCode;
            return controller.Json(response);
        }
    }
}

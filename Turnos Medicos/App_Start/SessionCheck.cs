using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Turnos_Medicos.Models;

namespace Turnos_Medicos
{
    public class SessionCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session != null && session["user"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                { "Controller", "Usuarios" },
                { "Action", "Login" }
                });
            }
            else
            {
                string actionName = filterContext.Controller.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = filterContext.Controller.ControllerContext.RouteData.Values["controller"].ToString();
                TurnosMedicosEntities db = new TurnosMedicosEntities();
                //Usuario usuario = (Usuario) session["user"];
                Perfil perfiles = (Perfil)session["perfil"];
                var permisos = (from perfil in db.Perfil
                                         join p_permiso in db.PerfilPermiso on perfil.Id equals p_permiso.PerfilId
                                         join permiso in db.Permiso on p_permiso.PermisoId equals permiso.Id
                                         where perfil.Id == perfiles.Id
                                         && permiso.Controller == controllerName
                                         && permiso.Action == actionName
                                         select perfil).ToList();
                if (!(permisos.Count >= 1))
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                    { "Controller", "Home" },
                    { "Action", "Index" }
                    });
                }
            }
        }
    }
}
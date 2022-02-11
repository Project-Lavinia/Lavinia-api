using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Linq;

namespace Lavinia.Api
{
    internal class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.ControllerType.Namespace; // e.g. "Controllers.V1"

            if (controllerNamespace is null)
            {
                throw new InvalidOperationException("A namespace for a controller cannot be null!");
            }

            controller.ApiExplorer.GroupName = controllerNamespace.Split('.').Last().ToLower();
        }
    }
}
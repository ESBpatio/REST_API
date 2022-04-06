using ESB_ConnectionPoints.PluginsInterfaces;
using ESB_ConnectionPoints.Utils;
using System.Collections.Generic;

namespace REST_API
{
    public sealed class IngoingConnectionPointFactory : IIngoingConnectionPointFactory
    {
        public IIngoingConnectionPoint Create(
            Dictionary<string, string> parameters,
            IServiceLocator serviceLocator)
        {
            return (IIngoingConnectionPoint)new IngoingConnectionPoint( serviceLocator);
            //parameters.GetStringParameter("Настройки в формате JSON"),
            //    parameters.GetBoolParameter("Режим отладки", false),
        }
    }
}

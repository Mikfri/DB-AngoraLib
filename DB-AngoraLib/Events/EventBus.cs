using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Events
{
    public class EventBus
    {
        private readonly IServiceProvider _serviceProvider;

        public EventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //public async Task Trigger<TEvent>(TEvent e)
        //{
        //    var handler = _serviceProvider.GetRequiredService<IEventHandler<TEvent>>();
        //    await handler.Handle(e);
        //}
    }
}

// skal bruge:
//builder.services.AddTransient<IEventHandler<UserRegistered_Event>, UserRegistered_EventHandler>();
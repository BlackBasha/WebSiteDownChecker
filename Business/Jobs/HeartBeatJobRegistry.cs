using Autofac;
using Business.Abstract;
using Core.Notification;
using Entities.Concrete;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Jobs
{
   public  class HeartBeatJobRegistry : Registry
    {
        private IComponentContext _context;

        public HeartBeatJobRegistry()
        {

        }
        public HeartBeatJobRegistry(IComponentContext context)
        {
            _context = context;
            var result= _context.Resolve<ITargetAppService>();
            var applogService = _context.Resolve<IAppLogService>();
            var emailNotifier = _context.Resolve<INotifier<Message>>();
            var targetApps=result.GetAll();
            foreach (var item in targetApps.Data)
            {
                Schedule(() => new HeartBeatJob(item, applogService, emailNotifier)).WithName(item.Id.ToString()).ToRunNow().AndEvery(item.CheckInterval).Minutes();
            }
        }

    }
}

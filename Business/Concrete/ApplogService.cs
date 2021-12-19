using Business.Abstract;
using Business.Constants;
using Core.Aspect.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ApplogService : IAppLogService
    {

        IAppLogRepository _appLogRepository;
        public ApplogService(IAppLogRepository appLogRepository)
        {
            _appLogRepository = appLogRepository;
        }
        [LogAspect(typeof(DatabaseLogger))]
        public IResult Add(AppLog appLog)
        {
            _appLogRepository.Add(appLog);
            return new SuccessResult(Messages.AppLogAdded);
        }
       
        public IDataResult<IEnumerable<AppLog>> GetLogsForApp(int appId)
        {
            var result = _appLogRepository.GetList(p => p.TargetAppId == appId);
            return new SuccessDataResult<IEnumerable<AppLog>>(result);
        }


      

    }
}

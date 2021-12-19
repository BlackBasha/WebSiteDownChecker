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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class TargetAppService : ITargetAppService
    {
        private ITargetAppRepository _targetAppRepository;


        public TargetAppService(ITargetAppRepository targetAppRepository)
        {
            _targetAppRepository = targetAppRepository;
        }

        [LogAspect(typeof(DatabaseLogger))]
        public IResult Add(TargetApp targetApp)
        {
            IResult result = BusinessRules.Run(CheckIfTargetAppExist(targetApp.Url));
            if (result != null)
            {
                return result;
            }
            _targetAppRepository.Add(targetApp);
            return new SuccessResult(Messages.TargetAppAdded);
        }

        public IResult Delete(TargetApp targetApp)
        {
            _targetAppRepository.Delete(targetApp);
            return new SuccessResult(Messages.TargetAppDeleted);
        }

        public IDataResult<TargetApp> Filter(string targetAppname)
        {
            var targetApp = _targetAppRepository.Get(x => x.AppName == targetAppname);
            return  new SuccessDataResult<TargetApp>(targetApp);

        }

        public IDataResult<TargetApp> GetById(int id)
        {
            return new SuccessDataResult<TargetApp>(_targetAppRepository.Get(p => p.Id == id));
        }

        public IDataResult<IEnumerable<TargetApp>> GetAll()
        {
            return new SuccessDataResult<List<TargetApp>>(_targetAppRepository.GetList().ToList());
        }

        public IResult Update(TargetApp targetApp)
        {
            _targetAppRepository.Update(targetApp);
            return new SuccessResult(Messages.TargetAppUpdated);
        }

        private IResult CheckIfTargetAppExist(string url)
        {

            var result = _targetAppRepository.GetList(p => p.Url == url).Any();
            if (result)
            {
                return new ErrorResult(Messages.TargetAppAlreadyExists);
            }

            return new SuccessResult();
        }
    }
}

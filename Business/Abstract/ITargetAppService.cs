using Core.Utilities.Results;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface  ITargetAppService 
    {
        IResult Add(TargetApp entity);

        IResult Update(TargetApp entity);

        IDataResult<IEnumerable<TargetApp>> GetAll();

        IResult Delete(TargetApp entity);

        IDataResult<TargetApp> GetById(int id);
    }
}

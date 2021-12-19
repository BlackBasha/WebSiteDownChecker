using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Abstract;
using case_exam_web.Models;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace case_exam_web.Controllers
{
    public class AppLogController : Controller
    {
        private IAppLogService _appLogService;

        public AppLogController(IAppLogService appLogService)
        {
            _appLogService = appLogService;
        }

        public ActionResult List(int targetAppId)
        {

            var result = _appLogService.GetLogsForApp(targetAppId);

            if (result.Success)
            {
                var viewModel = new AppLogViewModel()
                {
                    Applogs = result.Data.ToList()
                };

                return View(viewModel);
            }
            return RedirectToAction("Error");
        }

    }
}

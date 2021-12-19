using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using case_exam_web.Models;
using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using FluentScheduler;
using Business.Jobs;
using Core.Notification;

namespace case_exam_web.Controllers
{
    public class TargetAppController : Controller
    {

        private ITargetAppService _targetAppService;
        private readonly IMapper _mapper;
        private IAppLogService _appLogService;
        private INotifier<Message> _emailNotifier;

        public TargetAppController(ITargetAppService targetAppService, IMapper mapper, IAppLogService appLogService, INotifier<Message> emailNotifier)
        {
            _targetAppService = targetAppService;
            _mapper = mapper;
             _appLogService= appLogService;
            _emailNotifier = emailNotifier;
        }

        [Authorize]
       public ActionResult List()
       {
            var result = _targetAppService.GetAll();
            
            if(result.Success)
            {
                var viewModel = new TargetAppListViewModel()
                {
                    TargetApps = result.Data.ToList()
             };
               
                return View(viewModel);
            }
            return RedirectToAction("Error");
        }

       


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TargetAppViewModel targetAppCreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var item= _mapper.Map<TargetApp>(targetAppCreateViewModel);
            
            var result = _targetAppService.Add(item);

            //add job schedual
            IJob job = new HeartBeatJob(item, _appLogService, _emailNotifier);
            JobManager.AddJob(job, s => s.WithName(item.Id.ToString()).ToRunNow().AndEvery(item.CheckInterval).Minutes());

            if (result.Success)
            {
                return RedirectToAction("List", controllerName: "TargetApp");
            }
            return RedirectToAction("Error");
        }


        public ActionResult Edit(int id)
        {


            var result = _targetAppService.GetById(id);
            
            if (result.Success)
            {
                var targetApp = _mapper.Map<TargetAppViewModel>(result.Data);

                return View(targetApp);
            }
            return RedirectToAction("Error");

        }

        [HttpPost]
        public ActionResult Edit(TargetAppViewModel targetAppCreateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var item = _mapper.Map<TargetApp>(targetAppCreateViewModel);

            var result = _targetAppService.Update(item);

           //update job schedual, we take into consideration that the job is fast so it will be finished by the time we remove it form the registry
           // for long running jobs we should contorl the finish of the job and remove it after finishing  so we have to control the running status 
            JobManager.RemoveJob(item.Id.ToString());
            IJob job = new HeartBeatJob(item, _appLogService, _emailNotifier);
            JobManager.AddJob(job, s => s.WithName(item.Id.ToString()).ToRunEvery(item.CheckInterval).Minutes());

            if (result.Success)
            {
                return RedirectToAction("List", controllerName: "TargetApp");
            }
            return RedirectToAction("Error");
        }


        public ActionResult Delete(int id)
        {

            var result = _targetAppService.GetById(id);

            if (result.Success)
            {
                var targetApp = _mapper.Map<TargetAppViewModel>(result.Data);
                return View(targetApp);
            }
                return RedirectToAction("Error");
        }

        [HttpPost]
        public ActionResult Delete(TargetAppViewModel targetAppCreateViewModel)
        {
            var item = _mapper.Map<TargetApp>(targetAppCreateViewModel);
            var result = _targetAppService.Delete(item);

            //delete job schedual, we take into consideration that the job is fast so it will be finished by the time we remove it form the registry
            // for long running jobs we should contorl the finish of the job and remove it after finishing  so we have to control the running status 
            JobManager.RemoveJob(item.Id.ToString());
            if (result.Success)
            {
                return RedirectToAction("List", controllerName: "TargetApp");
            }
            return RedirectToAction("Error");
        }
    }
}

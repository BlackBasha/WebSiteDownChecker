using Business.Abstract;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Concrete;
using System.Net.Http;
using Core.EventBus;
using Core.Notification;

namespace Business.Jobs
{
    public class HeartBeatJob : IJob
    {
        private TargetApp _targetApp;

        private IAppLogService _applogService;

        private INotifier<Message> _emailNotifier;

        public HeartBeatJob(TargetApp targetApp, IAppLogService applogService, INotifier<Message> emailNotifier)
        {
            _targetApp = targetApp;
            _applogService = applogService;
            _emailNotifier = emailNotifier;
        }

        public async void Execute()
        {
            //In case you're curious,
            // public bool IsSuccessStatusCode is  get { return ((int)statusCode >= 200) && ((int)statusCode <= 299); }
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(_targetApp.Url);
                    if (!response.IsSuccessStatusCode)
                    {
                        _applogService.Add(new AppLog { InsertDate = DateTime.Now, LogDetails = await response.Content.ReadAsStringAsync(), ResponseNumber = (int)response.StatusCode, TargetAppId = _targetApp.Id });
                        //send mail notification
                        _emailNotifier.Notify(new Message()
                        {
                            Body = "Error in checking this website " + _targetApp.AppName + " with this url:" + _targetApp.Url + "the response code is: " + (int)response.StatusCode + " and the error message is:" + await response.Content.ReadAsStringAsync(),
                            FromAddress = "thelordmb@gmail.com",
                            Subject = "Error in checking this website",
                            ToAdresses = new List<string> { "thelordmb@gmail.com" }
                        });
                       
                    }
                    else
                    {
                        _applogService.Add(new AppLog { InsertDate = DateTime.Now, LogDetails = "App Is Fine!", ResponseNumber = (int)response.StatusCode, TargetAppId = _targetApp.Id });
                    }
                }
            }
            catch (Exception ex)
            {

                _emailNotifier.Notify(new Message()
                {
                    Body = "Error in checking this website " +
                                _targetApp.AppName + " with this url:" +
                                _targetApp.Url + "the response code is: " +
                                " and the error message is:" + ex?.Message,
                    FromAddress = "thelordmb@gmail.com",
                    Subject = "Error in checking this website",
                    ToAdresses = new List<string> { "thelordmb@gmail.com" }
                }); 
            }


        }
    }
}

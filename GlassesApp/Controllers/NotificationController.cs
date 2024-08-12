using Application.SignalR;
using CorePush.Apple;
using CorePush.Google;
using Infrastructure.Identity;
using Infrastructure.interfaces;
using Infrastructure.Migrations;
using Infrastructure.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static Infrastructure.Notifications.GoogleNotification;
using System.Net.Http.Headers;
using Infrastructure.Persistence;

namespace GlassesApp.Controllers
{
    // api/notification/send
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly IConfiguration _conf;

        public NotificationController(INotificationService notificationService,
            IHubContext<NotificationHub> hubContext, UserManager<ApplicationUser> userManager,
            IConfiguration conf)
        {
            _userManager = userManager;
            _notificationService = notificationService;
            _hubContext = hubContext;
            _conf = conf;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotifications()
        {
            var user = await _userManager.FindByIdAsync(User.FindFirst("ID")?.Value);

            var result = await
                _notificationService.GetNotifications(user.Id);
            return Ok(result);
        }
        




        [HttpGet("send-test")]
        public async Task<ActionResult<ResponseModel>> sendNotification(

            )
        {
            var notificationModel = new NotificationModel
            {
                DeviceId = "e2V07b-QTKC0j8UHUwilak:APA91bG06oxSRyptQwqTtniru36zyamW4PTqXV5C_gP7L1gYKgsRtUZCRcF8lFhs5qXRrKzTHiW6GAjugP1ZHxm1ryHuVFQX3fk2YWfGFW6JtYK5STY3DPiqXkT6nNTyyOI1eaRLuMZY",
                Body = "order has placed successfully !",
                IsAndroidDevice = true,
                Title = "SEE - new Order"
            };
            ResponseModel response = new ResponseModel();
            try
            {
                
                    /* FCM Sender (Android Device) */
                    var settings = new FcmSettings()
                    {
                        SenderId = _conf["FcmNotification:SenderId"],
                        ServerKey = _conf["FcmNotification:ServerKey"]
                    };

                    HttpClient httpClient = new HttpClient();

                    string authorizationKey = string.Format("key={0}", settings.ServerKey);

                    string deviceToken = notificationModel.DeviceId;

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    DataPayload dataPayload = new DataPayload();
                    dataPayload.Title = notificationModel.Title;
                    dataPayload.Body = notificationModel.Body;

                    GoogleNotification notification = new GoogleNotification();
                    notification.Data = dataPayload;
                    notification.Notification = dataPayload;

                    var fcm = new FcmSender(settings, httpClient);
                    
                    var fcmSendResponse = await fcm.SendAsync(deviceToken, 
                        notification);

                    if (fcmSendResponse.IsSuccess())
                    {
                        response.IsSuccess = true;
                        response.Message = "Notification sent successfully";
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = fcmSendResponse.Results[0].Error;
                        return response;
                    }
                

            } catch (Exception ex)
            {

            }

            return null;
        }
    } 
}

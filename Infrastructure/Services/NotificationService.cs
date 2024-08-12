using Infrastructure.Notifications;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CorePush.Google;
using Infrastructure.interfaces;
using static Infrastructure.Notifications.GoogleNotification;
using dotAPNS;
using CorePush.Apple;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _conf;
        private readonly ApplicationDbContext _db;
        public NotificationService(IConfiguration conf , ApplicationDbContext db)
        {
            _conf = conf;
            _db = db;
        }

        public async Task<List<Notification>> GetNotifications(Guid userId)
        {
           var notifications = await _db.Notifications.Where(n => n.ApplicationUserId == userId).ToListAsync();

           return notifications;
        }

        public async Task<ResponseModel> SendNotification(NotificationModel notificationModel)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (notificationModel.IsAndroidDevice)
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
                    var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);

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
                }
                else
                {
                    /* APN Sender (iOS Device) */
                    var apnSettings = new ApnSettings()
                    {
                        P8PrivateKey = _conf["ApnNotification:P8Certificate"],
                        P8PrivateKeyId = _conf["ApnNotification:P8Password"],
                        TeamId = _conf["ApnNotification:TeamId"],
                        AppBundleIdentifier = _conf["ApnNotification:BundleId"],
                        ServerType = ApnServerType.Development //
                             // or ApnServerType.Production,
                    };


                    var apn = new ApnSender(apnSettings, new HttpClient());

                    var apNotification = new ApnNotification
                    {
                        DeviceToken = notificationModel.DeviceId,
                        Payload = new Dictionary<string, object>
                        {
                            { "aps", new
                                {
                                    alert = new
                                    {
                                        title = notificationModel.Title,
                                        body = notificationModel.Body
                                    }
                                }
                            }
                        }
                    };

                    var apnSendResponse = await apn.SendAsync(apNotification, 
                        notificationModel.DeviceId);

                    if (apnSendResponse.IsSuccess)
                    {
                        response.IsSuccess = true;
                        response.Message = "Notification sent successfully";
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = apnSendResponse?.Error?.ToString(); 
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Something went wrong";
                return response;
            }
        }
    }
}

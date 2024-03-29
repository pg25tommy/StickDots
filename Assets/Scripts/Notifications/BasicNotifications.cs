using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class BasicNotifications : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Create channel to show notification
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic Notification",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = "PG25";
        notification.Text = "Advance Mobile";
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";

        // Timed notification
        notification.FireTime = System.DateTime.Now.AddSeconds(10);
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;

public class FirebaseService
{
    private readonly FirebaseApp _firebaseApp;

    public FirebaseService()
    {
        _firebaseApp = InitializeFirebaseApp();
    }

    private FirebaseApp InitializeFirebaseApp()
    {
        if (FirebaseApp.DefaultInstance != null)
        {
            return FirebaseApp.DefaultInstance;
        }

        var pathToKey = @"C:\Users\sudi\source\repos\Meshwark\Meshwark\firebase-service-account.json";
        return FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(pathToKey)
        });
    }

    public async Task SendNotification(string fcmToken, string title, string body)
    {
        var message = new Message
        {
            Token = fcmToken,
            Notification = new Notification
            {
                Title = title,
                Body = body
            },
            Data = new Dictionary<string, string>
            {
                { "click_action", "FLUTTER_NOTIFICATION_CLICK" }
            }
        };

        var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
        await messaging.SendAsync(message);
    }
}

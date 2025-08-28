namespace Husa.Uploader.Core.Extensions
{
    using Husa.Uploader.Core.Interfaces;

    public static class UploaderClientExtensions
    {
        public static void ShowRequestCreationFailedMessage(this IUploaderClient client)
        {
            client.ExecuteScript("$(\"head link[rel='stylesheet']\").last().after(\"<link rel='stylesheet' href='https://leadmanager.homesusa.com/css/animate.css' type='text/css'>\");");
            client.ExecuteScript("$(\"head link[rel='stylesheet']\").last().after(\"<link rel='stylesheet' href='https://leadmanager.homesusa.com/css/igrowl.css' type='text/css'>\");");
            client.ExecuteScript("$(\"head link[rel='stylesheet']\").last().after(\"<link rel='stylesheet' href='https://leadmanager.homesusa.com/css/fonts/feather.css' type='text/css'>\");");
            client.ExecuteScript("$(\"head\").append('<script src=\"https://leadmanager.homesusa.com/Scripts/igrowl.js\"></script>')");
            Thread.Sleep(2000);
            client.ExecuteScript("$.iGrowl({type: 'error',title: 'HomesUSA - Bulk Uploader',message: 'Request creation failed! Please check if listing has at least one completed request and does not have any pending requests. List will be skipped...',delay: 0,small: false,placement:{ x: 'right', y: 'bottom'}, offset: {x: 30,y: 50},animShow: 'fadeInDown',animHide: 'bounceOutUp'});");
            Thread.Sleep(5000);
            client.ExecuteScript("$.iGrowl.prototype.dismissAll('all')");
        }
    }
}

namespace Husa.Uploader.Core.Extensions
{
    using Husa.Uploader.Core.Interfaces;
    using OpenQA.Selenium;

    public static class UploaderClientExtensions
    {
        public static void FillFieldSingleOption(this IUploaderClient uploaderClient, string fieldName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var mainWindow = uploaderClient.WindowHandles.FirstOrDefault(windowHandle => windowHandle == uploaderClient.CurrentWindowHandle);
            uploaderClient.ExecuteScript(script: $"jQuery('#{fieldName}_TB').focus();");
            uploaderClient.ExecuteScript(script: $"jQuery('#{fieldName}_A')[0].click();");

            uploaderClient.SwitchToLast();

            Thread.Sleep(400);

            char[] fieldValue = value.ToUpper().ToArray();

            foreach (var charact in fieldValue)
            {
                Thread.Sleep(200);
                uploaderClient.FindElement(By.Id("m_txtSearch")).SendKeys(charact.ToString().ToUpper());
            }

            Thread.Sleep(400);
            var selectElement = $"const selected = jQuery('li[title^=\"{value}\"]'); jQuery(selected).focus(); jQuery(selected).click()";
            uploaderClient.ExecuteScript(script: selectElement);
            Thread.Sleep(400);

            uploaderClient.ExecuteScript("javascript:LBI_Popup.selectItem(true);");
            uploaderClient.SwitchTo().Window(mainWindow);
        }
    }
}

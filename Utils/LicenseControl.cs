using DTK.LPR;
using DTKWebSample.Models;

namespace DTKWebSample.Utils
{
    public static class LicenseControl
    {
        public static ResponseModel ShowCurrentLicInfo()
        {
            string system_id = LPREngine.GetSystemID();

            string license_key;
            string comments;
            int channels;
            DateTime expDate;
            LPREngine.GetActivatedLicenseInfo(out license_key, out comments, out channels, out expDate);
            ResponseModel res = new ResponseModel();
            if (license_key.Length > 0)
            {
                res.Status = true;
                res.Response = license_key;
                res.Date = expDate;
            }
            else
            {
                res.Status = false;
                res.Error = "No license activated on this computer.\r\nPlease contact at support@dtksoft.com to request trial license.";
            }
            return res;
        }
        private static string GetLicenseActivationErrorText(int err)
        {
            switch (err)
            {
                case 4: return "The license key is incorrect.";
                case 9: return "The license is already activated on another computer.";
                case 10: return "Another license is already activated on this computer.";
                case 12: return "This license cannot be activated as it is a USB dongle license.";
                case 16: return "The license key is for a different product.";
                default: return "Error code: " + err.ToString();
            }
        }
        public static ResponseModel ActivateLicense(string license_key)
        {
            int ret = LPREngine.ActivateLicenseOnline(license_key, "");
            if (ret == 0)
            {
                return ShowCurrentLicInfo();
            }
            else
            {
                ResponseModel res = new ResponseModel();
                res.Status = false;
                res.Error = GetLicenseActivationErrorText(ret);
                return res;
            }
        }
    }
}

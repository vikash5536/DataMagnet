using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;

namespace DataMagnetApp.CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CustomAction1(Session session)
        {
            session.Log("Begin CustomAction1");

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult DeleteLogFile(Session session)
        {
            try
            {
                var fileName = Environment.GetFolderPath(
             Environment.SpecialFolder.ApplicationData);

                string path = System.IO.Path.Combine(fileName + "\\MultiReports");

                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                session.Log("Inside DeleteLogFile" + ex);
            }
            return ActionResult.Success;
        }
    }
}

using Codeer.Friendly.Windows.KeyMouse;
using OpenQA.Selenium;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Selenium.CefSharp.Driver
{
    class CotnrolAccessor
    {
        CefSharpDriver _driver;

        internal CotnrolAccessor(IWebDriver driver)
        {
            //TODO Separate processing by browser and frame
            _driver = (CefSharpDriver)driver;
        }

        internal Screenshot GetScreenShot(Point location, Size size)
        {
            _driver.Activate();
            using (var bmp = new Bitmap(size.Width, size.Height))
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(_driver.PointToScreen(location), new Point(0, 0), bmp.Size);
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Bmp);
                    return new Screenshot(Convert.ToBase64String(ms.ToArray()));
                }
            }
        }

        internal void Click(Point location) => _driver.Click(MouseButtonType.Left, location);

        internal void SendKeys(string text)
        {
            _driver.Activate();
            KeySpec.SendKeys(_driver.App, text);
        }
    }
}

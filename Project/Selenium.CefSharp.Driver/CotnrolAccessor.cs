using Codeer.Friendly.Windows.Grasp;
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
        IUIObject _uiObject;

        internal CotnrolAccessor(IUIObject uiObject)
            => _uiObject = uiObject;

        internal Screenshot GetScreenShot(Point location, Size size)
        {
            _uiObject.Activate();
            using (var bmp = new Bitmap(size.Width, size.Height))
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(_uiObject.PointToScreen(location), new Point(0, 0), bmp.Size);
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Bmp);
                    return new Screenshot(Convert.ToBase64String(ms.ToArray()));
                }
            }
        }

        internal void Click(Point location)
            => _uiObject.Click(MouseButtonType.Left, location);

        internal void SendKeys(string text)
        {
            _uiObject.Activate();
            KeySpec.SendKeys(_uiObject.App, text);
        }
    }
}

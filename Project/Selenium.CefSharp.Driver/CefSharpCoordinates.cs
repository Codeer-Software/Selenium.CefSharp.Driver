using OpenQA.Selenium.Interactions.Internal;
using System;

namespace Selenium.CefSharp.Driver
{
    class CefSharpCoordinates : ICoordinates
    {
        CefSharpWebElement _element;

        public CefSharpCoordinates(CefSharpWebElement element)
            => _element = element;

        public System.Drawing.Point LocationOnScreen => throw new NotImplementedException();

        public System.Drawing.Point LocationInViewport => _element.LocationOnScreenOnceScrolledIntoView;

        public System.Drawing.Point LocationInDom => _element.Location;

        public object AuxiliaryLocator => _element.Id;
    }
}

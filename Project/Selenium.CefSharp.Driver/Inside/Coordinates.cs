using OpenQA.Selenium.Interactions.Internal;
using System;

namespace Selenium.CefSharp.Driver.Inside
{
    class Coordinates : ICoordinates
    {
        CefSharpWebElement _element;

        public System.Drawing.Point LocationOnScreen => throw new NotImplementedException();

        public System.Drawing.Point LocationInViewport => _element.LocationOnScreenOnceScrolledIntoView;

        public System.Drawing.Point LocationInDom => _element.Location;

        public object AuxiliaryLocator => _element.Id;

        public Coordinates(CefSharpWebElement element)
            => _element = element;
    }
}

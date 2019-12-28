namespace Selenium.CefSharp.Driver
{
    static class JS
    {
        public const string Initialize = @"
(function () {
    if (!(window.__seleniumCefSharpDriver == null)) return;

    window.__seleniumCefSharpDriver = {};
    window.__seleniumCefSharpDriver.elements = [];

    window.__seleniumCefSharpDriver.showAndSelectElement = function (element) {
        var rect = element.getBoundingClientRect();
        var elemtop = rect.top + window.pageYOffset;
        document.documentElement.scrollTop = elemtop;
        element.focus();
    };

    window.__seleniumCefSharpDriver.entryElement = function (element) {

        if (element === null) return -1;

        var index = window.__seleniumCefSharpDriver.elements.findIndex(x => {
            return x === element;
        });
        if (index != -1) return index;

        window.__seleniumCefSharpDriver.elements.push(element);
        return window.__seleniumCefSharpDriver.elements.length - 1;
    };
})();
";

        public static string FindElementById(string id)
            => $@"return window.__seleniumCefSharpDriver.entryElement(document.getElementById('{id}'));";

        public static string Click(int index)
            => $@"
var element = window.__seleniumCefSharpDriver.elements[{index}];
window.__seleniumCefSharpDriver.showAndSelectElement(element);
element.click();
";
        public static string Focus(int index)
    => $@"
var element = window.__seleniumCefSharpDriver.elements[{index}];
window.__seleniumCefSharpDriver.showAndSelectElement(element);
";
    }
}

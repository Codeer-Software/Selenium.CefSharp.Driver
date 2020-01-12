namespace Selenium.CefSharp.Driver
{
    static class JS
    {
        public const string Initialize = @"
(function () {

    if (window.__seleniumCefSharpDriver) return;
    
    window.__seleniumCefSharpDriver = (function() {
        const dataSetKey = 'selemniumCefSharpDriverRef';
        const attributeDataSetkey = 'data-' + dataSetKey.replace(/([A-Z])/g, (s) => {
            return '-' + s.charAt(0).toLowerCase();
        });
        let id = 1;
        return {
            showAndSelectElement(element) {
                const rect = element.getBoundingClientRect();
                const elemtop = rect.top + window.pageYOffset;
                document.documentElement.scrollTop = elemtop;
                element.focus();
            },
            entryElement(element) {
                if(!element) return -1;
                const current = element.dataset[dataSetKey];
                if(current) return current;
                id += 1;
                element.dataset[dataSetKey] = id;
                return id;
            }, 
            getElementByEntryId(id) {
                const result = document.querySelector(`[${attributeDataSetkey}='${id}']`);
                if(!result) {
                    throw 'EntriedElementNotFound';
                }
                return result;
            }
        };
    })();
})();
";

        public static string FindElementById(string id)
            => $@"return window.__seleniumCefSharpDriver.entryElement(document.getElementById('{id}'));";

        public static string GetAttribute(int id, string attrName) => $@"
const elem = window.__seleniumCefSharpDriver.getElementByEntryId({id});
return elem.getAttribute('{attrName}');";

        public static string Click(int index)
            => $@"
var element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
window.__seleniumCefSharpDriver.showAndSelectElement(element);
element.click();
";
        public static string Focus(int index)
    => $@"
var element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
window.__seleniumCefSharpDriver.showAndSelectElement(element);
";
    }
}

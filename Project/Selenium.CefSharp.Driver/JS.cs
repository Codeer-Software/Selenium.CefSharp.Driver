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

        public static string FindElementByEntryIdScriptBody(int id)
            => $"window.__seleniumCefSharpDriver.getElementByEntryId({id})";

        public static string FindElementById(string id)
            => $@"return window.__seleniumCefSharpDriver.entryElement(document.getElementById('{id}'));";

        public static string GetAttribute(int id, string attrName) => $@"
const elem = {FindElementByEntryIdScriptBody(id)};
return elem.getAttribute('{attrName}');";

        public static string SetAttribute(int id, string attrName, string value) => $@"
const elem = {FindElementByEntryIdScriptBody(id)};
return elem.setAttribute('{attrName}', '{value}');";

        public static string GetProperty(int id, string propName) => $@"
const elem = {FindElementByEntryIdScriptBody(id)};
return elem['{propName}'];";

        public static string Click(int index)
            => $@"
const element = {FindElementByEntryIdScriptBody(index)};
window.__seleniumCefSharpDriver.showAndSelectElement(element);
element.click();
";

        public static string Focus(int index)
    => $@"
const element = {FindElementByEntryIdScriptBody(index)};
window.__seleniumCefSharpDriver.showAndSelectElement(element);
";
        public static string GetTagName(int index)
    => $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
return element.tagName;
";

        public static string GetInnerHTML(int index)
    => $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
return element.innerHTML;
";

        public static string GetDisabled(int index)
    => $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
return element.disabled;
";
        public static string GetSelected(int index)
    => $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
if ('selected' in element) return element.selected;
if ('checked' in element) return element.checked;
return false;
";

        public static string GetBoundingClientRectX(int index)
    => $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
return element.getBoundingClientRect().x;
";

        public static string GetBoundingClientRectY(int index)
    => $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
return element.getBoundingClientRect().y;
";

        public static string GetBoundingClientRectWidth(int index)
    => $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
return element.getBoundingClientRect().width;
";

        public static string GetBoundingClientRectHeight(int index)
    => $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
return element.getBoundingClientRect().height;
";

        public static string GetDisplayed(int index)
    => $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
" + @"
if (element.offsetParent === null) {
    return false;
}

let target = element;
do {
const style = getComputedStyle(target);

if (style.display === 'none'
    || style.visibility !== 'visible'
    || parseFloat(style.opacity || '') <= 0.0
    || parseInt(style.height || '', 10) <= 0
    || parseInt(style.width || '', 10) <= 0
) {
    return false;
}

target = target.parentElement;
} while (target !== null)

return true;
";

        public static string GetCssValue(int index, string propertyName)
    => $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({index});
const style = getComputedStyle(element);
return style['{propertyName}'];
";

        public static string Submit(int index)
            => $@"
const element = {FindElementByEntryIdScriptBody(index)};
element.submit();
";

        public static string FindElementByProp(int index, string prop, string value)
        => $@"
const element = {FindElementByEntryIdScriptBody(index)};
for(let i = 0; i < element.children.length; i++) {{
    if (element.children[i].{prop} == '{value}') return element.children[i];
}}
return null;";

        public static string FindElementByPropIgnoreCase(int index, string prop, string value)
        => $@"
const element = {FindElementByEntryIdScriptBody(index)};
for(let i = 0; i < element.children.length; i++) {{
    if (element.children[i].{prop}.toUpperCase() == '{value.ToUpper()}') return element.children[i];
}}
return null;";

        public static string FindElementsByProp(int index, string prop, string value)
        => $@"
const element = {FindElementByEntryIdScriptBody(index)};
const hits = [];
for(let i = 0; i < element.children.length; i++) {{
    if (element.children[i].{prop} == '{value}') hits.push(element.children[i]);
}}
return hits;";

        public static string FindElementsByPropIgnoreCase(int index, string prop, string value)
        => $@"
const element = {FindElementByEntryIdScriptBody(index)};
let hits = [];
for(let i = 0; i < element.children.length; i++) {{
    if (element.children[i].{prop}.toUpperCase() == '{value.ToUpper()}') hits.push(element.children[i]);
}}
return hits;";

        public static string FindElementsByPropContains(int index, string prop, string value)
        => $@"
const element = {FindElementByEntryIdScriptBody(index)};
const hits = [];
for(let i = 0; i < element.children.length; i++) {{
    if (element.children[i].{prop}.split(' ').indexOf('{value}') != -1) hits.push(element.children[i]);
}}
return hits;";

    }
}
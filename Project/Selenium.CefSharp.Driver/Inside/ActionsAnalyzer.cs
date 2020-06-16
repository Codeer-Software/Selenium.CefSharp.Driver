using Codeer.Friendly.Windows.KeyMouse;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Selenium.CefSharp.Driver.InTarget;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Selenium.CefSharp.Driver.Inside
{
    static class ActionsAnalyzer
    {
        internal class Interaction
        {
            protected ReflectionAccessor Core { get; }
            protected Interaction(object core) => Core = core == null ? null : new ReflectionAccessor(core);
            internal virtual void Execute(CefSharpDriver driver, Dictionary<string, bool> modifyKeys) { }
        }

        internal class KeyDownInteraction : TypingInteraction
        {
            internal KeyDownInteraction(object core) : base(core) { }
            internal override void Execute(CefSharpDriver driver, Dictionary<string, bool> modifyKeys)
            {
                if (KeySpec.IsModifyKey(value))
                {
                    modifyKeys[value] = true;
                }
                if (KeySpec.IsAzOrNumber(value))
                {
                    KeySpec.SimpleKeyDown(driver.App, value);
                }
                else
                {
                    //Modifier key is released once.
                    var modify = string.Empty;
                    foreach (var e in modifyKeys)
                    {
                        KeySpec.SimpleKeyUp(driver.App, e.Key);
                        modify += e.Key;
                    }
                    KeySpec.SendKeys(driver.App, modify + value);
                    foreach (var e in modifyKeys)
                    {
                        KeySpec.SimpleKeyDown(driver.App, e.Key);
                    }
                }
            }
        }

        internal class KeyUpInteraction : TypingInteraction
        {
            internal KeyUpInteraction(object core) : base(core) { }
            internal override void Execute(CefSharpDriver driver, Dictionary<string, bool> modifyKeys)
            {
                if (KeySpec.IsModifyKey(value))
                {
                    modifyKeys.Remove(value);
                    KeySpec.SimpleKeyUp(driver.App, value);
                }
                if (KeySpec.IsAzOrNumber(value))
                {
                    KeySpec.SimpleKeyUp(driver.App, value);
                }
            }
        }

        internal class TypingInteraction : Interaction
        {
            internal string type => Core.GetField<string>("type");
            internal string value => Core.GetField<string>("value");
            internal TypingInteraction(object core) : base(core) { }
        }

        internal class PointerDownInteraction : Interaction
        {
            internal string button => Core.GetField<object>("button").ToString();

            internal PointerDownInteraction(object core) : base(core) { }
            internal override void Execute(CefSharpDriver driver, Dictionary<string, bool> modifyKeys)
            {
                if (!Enum.TryParse<MouseButtonType>(button, out var buttonType)) return;
                var mouse = new MouseEmulator(driver.App);
                mouse.Down(buttonType);
            }
        }

        internal class PointerUpInteraction : Interaction
        {
            internal string button => Core.GetField<object>("button").ToString();

            internal PointerUpInteraction(object core) : base(core) { }
            internal override void Execute(CefSharpDriver driver, Dictionary<string, bool> modifyKeys)
            {
                if (!Enum.TryParse<MouseButtonType>(button, out var buttonType)) return;
                var mouse = new MouseEmulator(driver.App);
                mouse.Down(buttonType);
            }
        }

        internal class PointerCancelInteraction : Interaction
        {
            internal PointerCancelInteraction(object core) : base(core) { }
        }

        internal class PointerMoveInteraction : Interaction
        {
            internal IWebElement target => Core.GetField<IWebElement>("target");
            internal int x => Core.GetField<int>("x");
            internal int y => Core.GetField<int>("y");
            internal TimeSpan duration => Core.GetField<TimeSpan>("duration");
            internal string origin => Core.GetField<object>("origin").ToString();

            internal PointerMoveInteraction(object core) : base(core) { }
            internal override void Execute(CefSharpDriver driver, Dictionary<string, bool> modifyKeys)
            {
                var mouse = new MouseEmulator(driver.App);

                if (target == null)
                {
                    var pos = System.Windows.Forms.Cursor.Position;
                    pos.Offset(x, y);
                    mouse.Move(pos);
                }
                else
                {
                    var pos = driver.PointToScreen(target.Location);
                    var size = target.Size;
                    pos.Offset(size.Width / 2, size.Height / 2);
                    pos.Offset(x, y);
                    mouse.Move(pos);
                }
            }
        }

        internal class PointerClickInteraction : Interaction
        {
            string _button;
            internal PointerClickInteraction(string button) : base(null) => _button = button;
            internal override void Execute(CefSharpDriver driver, Dictionary<string, bool> modifyKeys)
            {
                if (!Enum.TryParse<MouseButtonType>(_button, out var buttonType)) return;
                var mouse = new MouseEmulator(driver.App);
                mouse.Click(buttonType);
            }
        }

        internal class PointerDoubleClickInteraction : Interaction
        {
            string _button;
            internal PointerDoubleClickInteraction(string button) : base(null) => _button = button;
            internal override void Execute(CefSharpDriver driver, Dictionary<string, bool> modifyKeys)
            {
                if (!Enum.TryParse<MouseButtonType>(_button, out var buttonType)) return;
                var mouse = new MouseEmulator(driver.App);
                mouse.DoubleClick(buttonType);
            }
        }

        static Type[] InteractionTypes = new[]
        {
            typeof(KeyDownInteraction),
            typeof(KeyUpInteraction),
            typeof(PointerDownInteraction),
            typeof(PointerUpInteraction),
            typeof(PointerMoveInteraction),
            typeof(PointerCancelInteraction),
        };

        internal static void PerformActions(CefSharpDriver driver, IList<ActionSequence> actionSequenceList)
        {
            var x = GetInteractions(actionSequenceList);
            var modifyKeys = new Dictionary<string, bool>();
            foreach (var e in x)
            {
                e.Execute(driver, modifyKeys);
            }

            //up modify keys.
            foreach (var e in modifyKeys)
            {
                KeySpec.SimpleKeyUp(driver.App, e.Key);
            }
        }

        static Interaction[] GetInteractions(IList<ActionSequence> actionSequenceList)
        {
            if (actionSequenceList[0].Count == 0) return new Interaction[0];

            var interractions = new Interaction[actionSequenceList[0].Count];
            foreach (var seq in actionSequenceList)
            {
                var seqAcs = new ReflectionAccessor(seq);
                var interactions = seqAcs.GetField<IList>("interactions");
                for (int i = 0; i < interactions.Count; i++)
                {
                    foreach (var type in InteractionTypes)
                    {
                        if (interactions[i].GetType().FullName.Contains(type.Name))
                        {
                            interractions[i] = (Interaction)type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(object) }, null).Invoke(new object[] { interactions[i] });
                        }
                    }
                }
            }

            return AdjustInteractions(interractions);
        }

        static Interaction[] AdjustInteractions(Interaction[] interractions)
        {
            var list = new List<Interaction>();
            for (int i = 0; i < interractions.Length; i++)
            {
                if (interractions[i] == null) continue;
                else if (AdjustMouseInteraction(interractions, ref i, out var doubleClick)) list.Add(doubleClick);
                else
                {
                    var e = interractions[i];
                    list.Add(e);
                }
            }
            return list.ToArray();
        }

        static bool AdjustMouseInteraction(Interaction[] interractions, ref int i, out Interaction interaction)
        {
            interaction = null;
            var mouseDown1 = interractions[i] as PointerDownInteraction;
            if (mouseDown1 == null) return false;

            if (i + 3 < interractions.Length)
            {
                var mouseUp1 = interractions[i] as PointerUpInteraction;
                var mouseDown2 = interractions[i] as PointerDownInteraction;
                var mouseUp2 = interractions[i] as PointerUpInteraction;
                if ((mouseUp1 != null && mouseDown2 != null && mouseUp2 != null) &&
                    (mouseDown1.button == mouseUp1.button && mouseDown1.button == mouseDown2.button && mouseDown1.button == mouseUp2.button))
                {
                    i += 3;
                    interaction = new PointerDoubleClickInteraction(mouseDown1.button);
                    return true;
                }
            }
            if (i + 1 < interractions.Length)
            {
                var mouseUp1 = interractions[i] as PointerUpInteraction;
                if (mouseUp1 != null && mouseDown1.button == mouseUp1.button)
                {
                    i += 1;
                    interaction = new PointerClickInteraction(mouseDown1.button);
                    return true;
                }
            }
            return false;
        }
    }
}

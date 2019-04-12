﻿using System;
using System.Collections.Generic;
using UIKit;

namespace XamEffects.iOS.GestureCollectors {
    internal static class LongTapGestureCollector {
        static Dictionary<UIView, GestureActionsContainer> Collection { get; } =
            new Dictionary<UIView, GestureActionsContainer>();

        public static void Add(UIView view, Action<UIGestureRecognizerState, bool> action) {
            if (Collection.ContainsKey(view)) {
                Collection[view].Actions.Add(action);
            }
            else {
                var gest = new UILongPressGestureRecognizer(ActionActivator);
                Collection.Add(view,
                    new GestureActionsContainer {
                        Recognizer = gest, Actions = new List<Action<UIGestureRecognizerState, bool>> {action}
                    });
                view.AddGestureRecognizer(gest);
            }
        }

        public static void Delete(UIView view, Action<UIGestureRecognizerState, bool> action) {
            if (!Collection.ContainsKey(view)) return;

            var ci = Collection[view];
            ci.Actions.Remove(action);

            if (ci.Actions.Count != 0) return;
            view.RemoveGestureRecognizer(ci.Recognizer);
            ci.Recognizer.Dispose();
            ci.Recognizer = null;
            ci.Actions = null;
            Collection.Remove(view);
        }

        static void ActionActivator(UILongPressGestureRecognizer uiLongPressGestureRecognizer) {
            var coord = uiLongPressGestureRecognizer.LocationInView(uiLongPressGestureRecognizer.View);
            var inside = uiLongPressGestureRecognizer.View.PointInside(coord, null);

            if (!Collection.ContainsKey(uiLongPressGestureRecognizer.View)) return;
            foreach (var valueAction in Collection[uiLongPressGestureRecognizer.View].Actions) {
                valueAction?.Invoke(uiLongPressGestureRecognizer.State, inside);
            }
        }

        class GestureActionsContainer {
            public UIGestureRecognizer Recognizer { get; set; }
            public List<Action<UIGestureRecognizerState, bool>> Actions { get; set; }
        }
    }
}
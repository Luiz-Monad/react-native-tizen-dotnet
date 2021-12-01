﻿
using ElmSharp;

namespace ReactNative.UIManager
{
    /// <summary>
    /// Interface consisting of methods which are relevant to views which contain
    /// visuals that have react tags but are not rendered using Widgets.
    /// </summary>
    public interface IReactCompoundView
    {
        /// <summary>
        /// Returns the react tag rendered at point in reactView. The view
        /// is not expected to do hit testing on its Widget descendants. Rather,
        /// this is useful for views which are composed of visuals that are associated
        /// with react tags but the visuals are not Widgets.
        /// </summary>
        /// <param name="reactView">The react view to do hit testing within.</param>
        /// <param name="point">The point to hit test in coordinates that are relative to the view.</param>
        /// <returns>The react tag rendered at point in reactView.</returns>
        int GetReactTagAtPoint(Widget reactView, Point point);
    }
}

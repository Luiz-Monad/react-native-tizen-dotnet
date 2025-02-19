﻿using System;
using System.Collections.Generic;
using System.Reactive.Disposables;

using Newtonsoft.Json.Linq;

using ReactNative.UIManager;
using ReactNative.UIManager.Annotations;
using ReactNative.Modules.Image;
using ReactNative.Common;
using ReactNative.Collections;

using ElmSharp;
using ReactNativeTizen.ElmSharp.Extension;

namespace ReactNative.Views.ReactImage
{
    /// <summary>
    /// The view manager responsible for rendering native images.
    /// </summary>
    public class ReactImageManager : SimpleViewManager<Image>
    {
        /*
        private readonly Dictionary<Image, ExceptionRoutedEventHandler> _imageFailedHandlers =
            new Dictionary<Image, ExceptionRoutedEventHandler>();

        private readonly Dictionary<Image, RoutedEventHandler> _imageOpenedHandlers =
            new Dictionary<Image, RoutedEventHandler>();
        */

        private readonly Dictionary<int, SerialDisposable> _disposables =
            new Dictionary<int, SerialDisposable>();

        private readonly Dictionary<int, List<KeyValuePair<string, double>>> _imageSources =
            new Dictionary<int, List<KeyValuePair<string, double>>>();

        /// <summary>
        /// The view manager name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "RCTImageView";
            }
        }

        /// <summary>
        /// The view manager event constants.
        /// </summary>
        public override IReadOnlyDictionary<string, object> ExportedCustomDirectEventTypeConstants
        {
            get
            {
                return new Dictionary<string, object>
                {
                    {
                        "topLoadStart",
                        new Dictionary<string, object>
                        {
                            { "registrationName", "onLoadStart" }
                        }
                    },
                    {
                        "topLoad",
                        new Dictionary<string, object>
                        {
                            { "registrationName", "onLoad" }
                        }
                    },
                    {
                        "topLoadEnd",
                        new Dictionary<string, object>
                        {
                            { "registrationName", "onLoadEnd" }
                        }
                    },
                };
            }
        }

        /// <summary>
        /// Sets the background color of the view.
        /// </summary>
        /// <param name="view">The view instance.</param>
        /// <param name="color">The masked color value.</param>
        [ReactProp(
            ViewProps.BackgroundColor,
            CustomType = "Color",
            DefaultUInt32 = ColorHelpers.Transparent)]
        public void SetBackgroundColor(Image view, uint color)
        {
            view.BackgroundColor = Color.FromUint(color);
        }

        /// <summary>
        /// Set the scaling mode of the image.
        /// </summary>
        /// <param name="view">The image view instance.</param>
        /// <param name="resizeMode">The scaling mode.</param>
        [ReactProp("resizeMode")]
        public void SetResizeMode(Image view, string resizeMode)
        {
            if (resizeMode != null)
            {
                view.IsScaling = true;

                if (resizeMode.Equals("cover"))
                {
                    view.IsFixedAspect = true;
                    view.CanFillOutside = true;
                }
                else if (resizeMode.Equals("contain"))
                {
                    view.IsFixedAspect = true;
                    view.CanFillOutside = false;
                }
                else if (resizeMode.Equals("stretch"))
                {
                    view.IsFixedAspect = false;
                }
            }
        }

        /*
        [ReactProp("alignmentX")]
        public void SetAlignmentX(GenGrid view, double alignmentX)
        {
            Log.Info(ReactConstants.Tag, $"[Views::Image] SetAlignmentX:'{alignmentX}'");

            view.AlignmentX = alignmentX;
        }

        [ReactProp("alignmentY")]
        public void SetAlignmentY(GenGrid view, double AlignmentY)
        {
            Log.Info(ReactConstants.Tag, $"[Views::Image] SetAlignmentY:'{AlignmentY}'");

            view.AlignmentY = AlignmentY;
        }

        [ReactProp("weightX")]
        public void SetWeightX(GenGrid view, double weightX)
        {
            Log.Info(ReactConstants.Tag, $"[Views::Image] SetWeightX:'{weightX}'");

            view.WeightX = weightX;
        }

        [ReactProp("weightY")]
        public void SetWeightY(GenGrid view, double weightY)
        {
            Log.Info(ReactConstants.Tag, $"[Views::Image] SetWeightY:'{weightY}'");

            view.WeightY = weightY;
        }
        */
/*
        /// <summary>
        /// Set the source URI of the image.
        /// </summary>
        /// <param name="view">The image view instance.</param>
        /// <param name="source">The source URI.</param>
        [ReactProp("src")]
        public void SetSource(Image view, JArray sources)
        {
            Log.Info(ReactConstants.Tag, "[Views::Image] set src:" + sources);

            var count = sources.Count;

            // There is no image source
            if (count == 0)
            {
                throw new ArgumentException("Sources must not be empty.", nameof(sources));
            }
            // Optimize for the case where we have just one uri, case in which we don't need the sizes
            else if (count == 1)
            {
                var uri = ((JObject)sources[0]).Value<string>("uri");

                var metadata = new ImageMetadata(uri, view.GetDimensions().Width, view.GetDimensions().Height);
                OnImageStatusUpdate(view, ImageLoadStatus.OnLoadStart, metadata);
                view.Load(uri);
            }
            else
            {
                throw new ArgumentException("Multi Sources not supported now.", nameof(sources));
            }
        }
*/
        /// <summary>
        /// Set the source URI of the image.
        /// </summary>
        /// <param name="view">The image view instance.</param>
        /// <param name="sources">The source URI.</param>
        [ReactProp("src")]
        public void SetSource(Image view, JArray sources)
        {
            var count = sources.Count;

            // There is no image source
            if (count == 0)
            {
                throw new ArgumentException("Sources must not be empty.", nameof(sources));
            }
            // Optimize for the case where we have just one uri, case in which we don't need the sizes
            else if (count == 1)
            {
                var uri = ((JObject)sources[0]).Value<string>("uri");
                SetUriFromSingleSource(view, uri);
            }
            else
            {
                var viewSources = default(List<KeyValuePair<string, double>>);
                var tag = view.GetTag();

                if (_imageSources.TryGetValue(tag, out viewSources))
                {
                    viewSources.Clear();
                }
                else
                {
                    viewSources = new List<KeyValuePair<string, double>>(count);
                    _imageSources.Add(tag, viewSources);
                }

                foreach (var source in sources)
                {
                    var sourceData = (JObject)source;
                    viewSources.Add(
                        new KeyValuePair<string, double>(
                            sourceData.Value<string>("uri"),
                            sourceData.Value<double>("width") * sourceData.Value<double>("height")));
                }

                viewSources.Sort((p1, p2) => p1.Value.CompareTo(p2.Value));

                if (double.IsNaN(view.GetDimensions().Width) || double.IsNaN(view.GetDimensions().Height))
                {
                    // If we need to choose from multiple URIs but the size is not yet set, wait for layout pass
                    return;
                }

                SetUriFromMultipleSources(view);
            }
        }
/*
        /// <summary>
        /// The border radius of the <see cref="ReactRootView"/>.
        /// </summary>
        /// <param name="view">The image view instance.</param>
        /// <param name="radius">The border radius value.</param>
        [ReactProp("borderRadius")]
        public void SetBorderRadius(Image view, double radius)
        {
            //view.CornerRadius = new CornerRadius(radius);
        }

        /// <summary>
        /// Sets the border thickness of the image view.
        /// </summary>
        /// <param name="view">The image view instance.</param>
        /// <param name="index">The property index.</param>
        /// <param name="width">The border width in pixels.</param>
        [ReactPropGroup(
            ViewProps.BorderWidth,
            ViewProps.BorderLeftWidth,
            ViewProps.BorderRightWidth,
            ViewProps.BorderTopWidth,
            ViewProps.BorderBottomWidth,
            DefaultDouble = double.NaN)]
        public void SetBorderWidth(Image view, int index, double width)
        {
            // TODO:
        }

        /// <summary>
        /// Set the border color of the image view.
        /// </summary>
        /// <param name="view">The image view instance.</param>
        /// <param name="color">The masked color value.</param>
        [ReactProp("borderColor", CustomType = "Color")]
        public void SetBorderColor(Image view, uint? color)
        {
        }
*/
        /// <summary>
        /// Creates the image view instance.
        /// </summary>
        /// <param name="reactContext">The react context.</param>
        /// <returns>The image view instance.</returns>
        protected override Image CreateViewInstance(ThemedReactContext reactContext)
        {
            Log.Info(ReactConstants.Tag, "## Enter CreateViewInstance of Image ##");

            var image = new Image(ReactProgram.RctWindow);
            image.Show();

            Log.Info(ReactConstants.Tag, "## Exit CreateViewInstance of Image ##");

            return image;
        }

        /// <summary>
        /// Install custom event emitters on the given view.
        /// </summary>
        /// <param name="reactContext">The react context.</param>
        /// <param name="view">The view instance.</param>
        protected override void AddEventEmitters(ThemedReactContext reactContext, Image view)
        {
            view.Clicked += OnClicked;
        }

        private void OnClicked(object sender, EventArgs e)
        {
            Image img = (Image)sender;
            Log.Info(ReactConstants.Tag, "Image:" + img + " is Clicked!");
        }

        private void OnCompleted(object sender, EventArgs e)
        {
            Image img = (Image)sender;
            img.GetReactContext()
                .GetNativeModule<UIManagerModule>()
                .EventDispatcher
                .DispatchEvent(
                    new ReactImageLoadEvent(
                        img.GetTag(),
                        ReactImageLoadEvent.OnLoadEnd));

            Log.Info(ReactConstants.Tag, "Completed to load image:" + img.File);
        }

        /// <summary>
        /// Sets the dimensions of the view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="dimensions">The output buffer.</param>
        public override void SetDimensions(Image view, Dimensions dimensions)
        {
            base.SetDimensions(view, dimensions);
            SetUriFromMultipleSources(view);
        }

        /// <summary>
        /// Set the source URI of the image.
        /// </summary>
        /// <param name="view">The image view instance.</param>
        /// <param name="source">The source URI.</param>
        private async void SetUriFromSingleSource(Image view, string source)
        {
            Log.Info(ReactConstants.Tag, "[SetUriFromSingleSource] set src:" + source);

            var tag = view.GetTag();

            var disposable = default(SerialDisposable);
            if (!_disposables.TryGetValue(tag, out disposable))
            {
                disposable = new SerialDisposable();
                _disposables.Add(tag, disposable);
            }
            
            OnImageStatusUpdate(view, ImageLoadStatus.OnLoadStart, default(ImageMetadata));

            var metadata = new ImageMetadata(source, view.GetDimensions().Width, view.GetDimensions().Height);
            if (BitmapImageHelpers.IsBase64Uri(source))
            {
                Log.Info(ReactConstants.Tag, ">>> Base64Uri");
                //using (var stream = await BitmapImageHelpers.GetStreamAsync(source))
                using (var stream = BitmapImageHelpers.GetStreamSync(source))
                {
                    var ret = await view.LoadAsync(stream);
                    if ( false == ret )
                    {
                        Log.Warn(ReactConstants.Tag, "Failed to LoadAsync:" + source.ToString());
                        OnImageFailed(view);
                    }
                    else
                    {
                        OnImageStatusUpdate(view, ImageLoadStatus.OnLoad, metadata);
                    }
                }

            }
            else if (BitmapImageHelpers.IsHttpUri(source))
            {
                Log.Info(ReactConstants.Tag, ">>> HttpUri");

                //var metadata = new ImageMetadata(source, view.GetDimensions().Width, view.GetDimensions().Height);
                var ret = await view.LoadAsync(source);
                if ( false == ret )
                {
                    Log.Warn(ReactConstants.Tag, "Failed to LoadAsync:" + source.ToString());
                    OnImageFailed(view);
                }
                else
                {
                    OnImageStatusUpdate(view, ImageLoadStatus.OnLoad, metadata);
                }
            }
            else
            {
                Log.Info(ReactConstants.Tag, ">>> Loacal");

                //var metadata = new ImageMetadata(source, view.GetDimensions().Width, view.GetDimensions().Height);
                var ret = await view.LoadAsync(source);
                if ( false == ret )
                {
                    Log.Warn(ReactConstants.Tag, "Failed to LoadAsync:" + source.ToString());
                    OnImageFailed(view);
                }
                else
                {
                    OnImageStatusUpdate(view, ImageLoadStatus.OnLoad, metadata);
                }
            }

            OnImageStatusUpdate(view, ImageLoadStatus.OnLoadEnd, metadata);
			
			disposable.Dispose();
        }

        private void OnImageFailed(Image view)
        {
            view.GetReactContext()
                .GetNativeModule<UIManagerModule>()
                .EventDispatcher
                .DispatchEvent(
                    new ReactImageLoadEvent(
                        view.GetTag(),
                        ReactImageLoadEvent.OnLoadEnd));
        }

        private void OnImageStatusUpdate(Image view, ImageLoadStatus status, ImageMetadata metadata)
        {
            var eventDispatcher = view.GetReactContext()
                .GetNativeModule<UIManagerModule>()
                .EventDispatcher;

            eventDispatcher.DispatchEvent(
                new ReactImageLoadEvent(
                    view.GetTag(),
                    (int)status,
                    metadata.Uri,
                    (int)metadata.Width,
                    (int)metadata.Height));
        }

        /// <summary>
        /// Chooses the uri with the size closest to the target image size. Must be called only after the
        /// layout pass when the sizes of the target image have been computed, and when there are at least
        /// two sources to choose from.
        /// </summary>
        /// <param name="view">The image view instance.</param>
        private void SetUriFromMultipleSources(Image view)
        {
            var sources = default(List<KeyValuePair<string, double>>);
            if (_imageSources.TryGetValue(view.GetTag(), out sources))
            {
                var targetImageSize = view.GetDimensions().Width * view.GetDimensions().Height;
                var bestResult = sources.LocalMin((s) => Math.Abs(s.Value - targetImageSize));
                SetUriFromSingleSource(view, bestResult.Key);
            }
        }
    }
}

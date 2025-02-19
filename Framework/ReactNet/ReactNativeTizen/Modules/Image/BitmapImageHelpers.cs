﻿using System;
using System.IO;

namespace ReactNative.Modules.Image
{
    static class BitmapImageHelpers
    {
        public static bool IsBase64Uri(string uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return uri.StartsWith("data:");
        }

        public static bool IsHttpUri(string uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return uri.StartsWith("http:") || uri.StartsWith("https:");
        }

        //public static async Task<FileStream> GetStreamAsync(string uri)
        public static FileStream GetStreamSync(string uri)
        {
            if ( null == uri )
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return new FileStream(uri, FileMode.Open);
            /*
            if (IsBase64Uri(uri))
            {
                var decodedData = Convert.FromBase64String(uri.Substring(uri.IndexOf(",") + 1));
                return decodedData.AsBuffer().AsStream().AsRandomAccessStream();
            }
            else
            {
                var uriValue = default(Uri);
                if (!Uri.TryCreate(uri, UriKind.Absolute, out uriValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(uri), Invariant($"Invalid URI '{uri}' provided."));
                }

                var streamReference = RandomAccessStreamReference.CreateFromUri(uriValue);
                return await streamReference.OpenReadAsync();
            }
            */
        }
        /*
        public static IObservable<ImageStatusEventData> GetStreamLoadObservable(this BitmapImage image)
        {
            return image.GetOpenedObservable()
                .Merge(image.GetFailedObservable(), Scheduler.Default)
                .StartWith(new ImageStatusEventData(ImageLoadStatus.OnLoadStart));
        }

        public static IObservable<ImageStatusEventData> GetUriLoadObservable(this BitmapImage image)
        {
            return Observable.Merge(
                Scheduler.Default,
                image.GetDownloadingObservable(),
                image.GetOpenedObservable(),
                image.GetFailedObservable());
        }

        private static IObservable<ImageStatusEventData> GetOpenedObservable(this BitmapImage image)
        {
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                h => image.ImageOpened += h,
                h => image.ImageOpened -= h)
                .Select(args =>
                {
                    var bitmapImage = args.Sender as BitmapImage;
                    if (bitmapImage != null)
                    {
                        return new ImageStatusEventData(
                            ImageLoadStatus.OnLoad,
                            new ImageMetadata(
                                image.UriSource?.AbsoluteUri,
                                image.PixelWidth,
                                image.PixelHeight));
                    }
                    else
                    {
                        return new ImageStatusEventData(ImageLoadStatus.OnLoad);
                    }
                })
                .Take(1)
                .Concat(Observable.Return(new ImageStatusEventData(ImageLoadStatus.OnLoadEnd)));
        }

        private static IObservable<ImageStatusEventData> GetFailedObservable(this BitmapImage image)
        {
            return Observable.FromEventPattern<ExceptionRoutedEventHandler, ExceptionRoutedEventArgs>(
                h => image.ImageFailed += h,
                h => image.ImageFailed -= h)
                .Select<EventPattern<ExceptionRoutedEventArgs>, ImageStatusEventData>(pattern =>
                {
                    throw new InvalidOperationException(pattern.EventArgs.ErrorMessage);
                });
        }

        private static IObservable<ImageStatusEventData> GetDownloadingObservable(this BitmapImage image)
        {
            return Observable.FromEventPattern<DownloadProgressEventHandler, DownloadProgressEventArgs>(
                h => image.DownloadProgress += h,
                h => image.DownloadProgress -= h)
                .Take(1)
                .Select(_ => new ImageStatusEventData(ImageLoadStatus.OnLoadStart));
        }
        */
    }
}

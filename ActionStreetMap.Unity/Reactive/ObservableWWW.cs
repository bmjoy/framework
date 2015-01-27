using System;
using System.Collections;

using UnityEngine;

namespace ActionStreetMap.Infrastructure.Reactive
{
#if CONSOLE
    using System.Net;
#endif

#if !(UNITY_METRO || UNITY_WP8) && (CONSOLE || UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0 || UNITY_2_6_1 || UNITY_2_6)
    // Fallback for Unity versions below 4.5
    using Hash = System.Collections.Hashtable;
    using HashEntry = System.Collections.DictionaryEntry; 
   
#else
    // Unity 4.5 release notes: 
    // WWW: deprecated 'WWW(string url, byte[] postData, Hashtable headers)', 
    // use 'public WWW(string url, byte[] postData, Dictionary<string, string> headers)' instead.
    using Hash = System.Collections.Generic.Dictionary<string, string>;
    using HashEntry = System.Collections.Generic.KeyValuePair<string, string>;
#endif

    public static partial class ObservableWWW
    {
        public static IObservable<string> Get(string url, Hash headers = null, IProgress<float> progress = null)
        {
#if !CONSOLE
            return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, null, (headers ?? new Hash())), observer, progress, cancellation));
#else
            var webClient = new WebClient();
            var query = Observable.FromEventPattern<DownloadStringCompletedEventHandler, DownloadStringCompletedEventArgs>
                (
                    eh => new DownloadStringCompletedEventHandler(eh),
                    eh => webClient.DownloadStringCompleted += eh,
                    eh => webClient.DownloadStringCompleted -= eh
                ).Select(o => o.EventArgs.Result);

            webClient.DownloadStringAsync(new Uri(url));
            return query;
#endif
        }

        public static IObservable<byte[]> GetAndGetBytes(string url, Hash headers = null,
            IProgress<float> progress = null)
        {
#if !CONSOLE
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, null, (headers ?? new Hash())), observer, progress, cancellation));
#else
            var webClient = new WebClient();
            var query = Observable
                .FromEventPattern<DownloadDataCompletedEventHandler, DownloadDataCompletedEventArgs>
                (
                    eh => new DownloadDataCompletedEventHandler(eh),
                    eh => webClient.DownloadDataCompleted += eh,
                    eh => webClient.DownloadDataCompleted -= eh
                ).Select(o => o.EventArgs.Result);

            webClient.DownloadDataAsync(new Uri(url));
            return query;
#endif
        }

        public static IObservable<WWW> GetWWW(string url, Hash headers = null, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) => Fetch(new WWW(url, null, (headers ?? new Hash())), observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, byte[] postData, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, postData), observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, byte[] postData, Hash headers, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, postData, headers), observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, WWWForm content, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, content), observer, progress, cancellation));
        }

        public static IObservable<string> Post(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
        {
            var contentHeaders = content.headers;
            return ObservableUnity.FromCoroutine<string>((observer, cancellation) => FetchText(new WWW(url, content.data, MergeHash(contentHeaders, headers)), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, byte[] postData, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, postData), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, byte[] postData, Hash headers, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, postData, headers), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, content), observer, progress, cancellation));
        }

        public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
        {
            var contentHeaders = content.headers;
            return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) => FetchBytes(new WWW(url, content.data, MergeHash(contentHeaders, headers)), observer, progress, cancellation));
        }

        public static IObservable<WWW> PostWWW(string url, byte[] postData, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) => Fetch(new WWW(url, postData), observer, progress, cancellation));
        }

        public static IObservable<WWW> PostWWW(string url, byte[] postData, Hash headers, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) => Fetch(new WWW(url, postData, headers), observer, progress, cancellation));
        }

        public static IObservable<WWW> PostWWW(string url, WWWForm content, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) => Fetch(new WWW(url, content), observer, progress, cancellation));
        }

        public static IObservable<WWW> PostWWW(string url, WWWForm content, Hash headers, IProgress<float> progress = null)
        {
            var contentHeaders = content.headers;
            return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) => Fetch(new WWW(url, content.data, MergeHash(contentHeaders, headers)), observer, progress, cancellation));
        }

        public static IObservable<WWW> LoadFromCacheOrDownload(string url, int version, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) => Fetch(WWW.LoadFromCacheOrDownload(url, version), observer, progress, cancellation));
        }

        public static IObservable<WWW> LoadFromCacheOrDownload(string url, int version, uint crc, IProgress<float> progress = null)
        {
            return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) => Fetch(WWW.LoadFromCacheOrDownload(url, version, crc), observer, progress, cancellation));
        }

        // over 4.5, Hash define is Dictionary.
        // below Unity 4.5, WWW only supports Hashtable.
        // Unity 4.5, 4.6 WWW supports Dictionary and [Obsolete]Hashtable but WWWForm.content is Hashtable.
        // Unity 5.0 WWW only supports Dictionary and WWWForm.content is also Dictionary.
#if !(UNITY_METRO || UNITY_WP8) && (UNITY_4_5 || UNITY_4_6)
        static Hash MergeHash(Hashtable wwwFormHeaders, Hash externalHeaders)
        {
            var newHeaders = new Hash();
            foreach (DictionaryEntry item in wwwFormHeaders)
            {
                newHeaders.Add(item.Key.ToString(), item.Value.ToString());
            }
            foreach (HashEntry item in externalHeaders)
            {
                newHeaders.Add(item.Key, item.Value);
            }
            return newHeaders;
        }
#else
        static Hash MergeHash(Hash wwwFormHeaders, Hash externalHeaders)
        {
            foreach (HashEntry item in externalHeaders)
            {
                wwwFormHeaders[item.Key] = item.Value;
            }
            return wwwFormHeaders;
        }
#endif

        static IEnumerator Fetch(WWW www, IObserver<WWW> observer, IProgress<float> reportProgress, CancellationToken cancel)
        {
            using (www)
            {
                while (!www.isDone && !cancel.IsCancellationRequested)
                {
                    if (reportProgress != null)
                    {
                        try
                        {
                            reportProgress.Report(www.progress);
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            yield break;
                        }
                    }
                    yield return null;
                }

                if (cancel.IsCancellationRequested) yield break;

                if (!string.IsNullOrEmpty(www.error))
                {
                    observer.OnError(new WWWErrorException(www));
                }
                else
                {
                    observer.OnNext(www);
                    observer.OnCompleted();
                }
            }
        }

        static IEnumerator FetchText(WWW www, IObserver<string> observer, IProgress<float> reportProgress, CancellationToken cancel)
        {
            using (www)
            {
                while (!www.isDone && !cancel.IsCancellationRequested)
                {
                    if (reportProgress != null)
                    {
                        try
                        {
                            reportProgress.Report(www.progress);
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            yield break;
                        }
                    }
                    yield return null;
                }

                if (cancel.IsCancellationRequested) yield break;

                if (!string.IsNullOrEmpty(www.error))
                {
                    observer.OnError(new WWWErrorException(www));
                }
                else
                {
                    observer.OnNext(www.text);
                    observer.OnCompleted();
                }
            }
        }

        static IEnumerator FetchBytes(WWW www, IObserver<byte[]> observer, IProgress<float> reportProgress, CancellationToken cancel)
        {
            using (www)
            {
                while (!www.isDone && !cancel.IsCancellationRequested)
                {
                    if (reportProgress != null)
                    {
                        try
                        {
                            reportProgress.Report(www.progress);
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            yield break;
                        }
                    }
                    yield return null;
                }

                if (cancel.IsCancellationRequested) yield break;

                if (!string.IsNullOrEmpty(www.error))
                {
                    observer.OnError(new WWWErrorException(www));
                }
                else
                {
                    observer.OnNext(www.bytes);
                    observer.OnCompleted();
                }
            }
        }
    }

    public class WWWErrorException : Exception
    {
        public string RawErrorMessage { get; private set; }
        public bool HasResponse { get; private set; }
        public System.Net.HttpStatusCode StatusCode { get; private set; }
        public System.Collections.Generic.Dictionary<string, string> ResponseHeaders { get; private set; }
        public WWW WWW { get; private set; }

        public WWWErrorException(WWW www)
        {
            this.WWW = www;
            this.RawErrorMessage = www.error;
            this.ResponseHeaders = www.responseHeaders;
            this.HasResponse = false;

            var splitted = RawErrorMessage.Split(' ');
            if (splitted.Length != 0)
            {
                int statusCode;
                if (int.TryParse(splitted[0], out statusCode))
                {
                    this.HasResponse = true;
                    this.StatusCode = (System.Net.HttpStatusCode)statusCode;
                }
            }
        }

        public override string ToString()
        {
            return RawErrorMessage;
        }
    }
}
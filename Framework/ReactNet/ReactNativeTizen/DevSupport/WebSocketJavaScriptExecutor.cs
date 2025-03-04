﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactNative.Bridge;
using ReactNative.Collections;
using ReactNative.Common;
using ReactNative.Tracing;
//using SocketMessaging;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
//using WebSocketSharp;

namespace ReactNative.DevSupport
{
    class WebSocketJavaScriptExecutor : IJavaScriptExecutor
    {
        private const int ConnectTimeoutMilliseconds = 5000;
        private const int ConnectRetryCount = 3;

        private Websockets.Net.IWebSocketConnection _webSocket;
        private readonly JObject _injectedObjects;
        private readonly IDictionary<int, TaskCompletionSource<JToken>> _callbacks;

        private bool _connected;
        private int _requestId;
        private bool _isDisposed;

        public WebSocketJavaScriptExecutor()
        {
            _injectedObjects = new JObject();
            _callbacks = new Dictionary<int, TaskCompletionSource<JToken>>();
            //Websockets.Net.WebsocketConnection.Link();  //we do not need to link as websocket module already link
        }

        public async Task ConnectAsync(string webSocketServerUrl, CancellationToken token)
        {
            var uri = default(Uri);
            if (!Uri.TryCreate(webSocketServerUrl, UriKind.Absolute, out uri))
            {
                throw new ArgumentOutOfRangeException(nameof(webSocketServerUrl), "Expected valid URI argument.");
            }

            var retryCount = ConnectRetryCount;
            while (true)
            {
                var timeoutSource = new CancellationTokenSource(ConnectTimeoutMilliseconds);
                using (token.Register(timeoutSource.Cancel))
                {
                    try
                    {
                        await ConnectCoreAsync(uri, timeoutSource.Token).ConfigureAwait(false);
                        return;
                    }
                    catch (OperationCanceledException ex)
                    when (ex.CancellationToken == timeoutSource.Token)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    catch
                    {
                        if (--retryCount <= 0)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        timeoutSource.Dispose();
                    }
                }
            }
        }

        public JToken CallFunctionReturnFlushedQueue(string moduleName, string methodName, JArray arguments)
        {
            return Call("callFunctionReturnFlushedQueue", new JArray
            {
                moduleName,
                methodName,
                arguments,
            });
        }

        public JToken InvokeCallbackAndReturnFlushedQueue(int callbackId, JArray arguments)
        {
            return Call("invokeCallbackAndReturnFlushedQueue", new JArray
            {
                callbackId,
                arguments,
            });
        }

        public JToken FlushedQueue()
        {
            return Call("flushedQueue", new JArray());
        }

        public void RunScript(string script, string sourceUrl)
        {
            var requestId = Interlocked.Increment(ref _requestId);
            var callback = new TaskCompletionSource<JToken>();
            _callbacks.Add(requestId, callback);

            try
            {
                var request = new JObject
                {
                    { "id", requestId },
                    { "method", "executeApplicationScript" },
                    { "url", script },
                    { "inject", _injectedObjects },
                };

                SendMessage(requestId, request.ToString(Formatting.None));
                callback.Task.Wait();
            }
            catch (AggregateException ex)
            when (ex.InnerExceptions.Count == 1)
            {
                throw ex.InnerException;
            }
            finally
            {
                _callbacks.Remove(requestId);
            }
        }

        public void SetGlobalVariable(string propertyName, JToken value)
        {
            _injectedObjects.Add(propertyName, value.ToString(Formatting.None));
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _webSocket.Close();
            }
        }

        private JToken Call(string methodName, JArray arguments)
        {
            var requestId = Interlocked.Increment(ref _requestId);
            var callback = new TaskCompletionSource<JToken>();
            _callbacks.Add(requestId, callback);

            try
            {
                var request = new JObject
                {
                    { "id", requestId },
                    { "method", methodName },
                    { "arguments", arguments },
                };

                SendMessage(requestId, request.ToString(Formatting.None));
                return callback.Task.Result;
            }
            catch (AggregateException ex)
            when (ex.InnerExceptions.Count == 1)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                /* Should not */
                throw;
            }
            finally
            {
                _callbacks.Remove(requestId);
            }
        }

        private async Task ConnectCoreAsync(Uri uri, CancellationToken token)
        {
            if (!_connected)
            {
                _webSocket = Websockets.Net.WebSocketFactory.Create();

                _webSocket.OnMessage += OnMessageReceived;

                _webSocket.Open(uri.AbsoluteUri);

                _connected = true;
            }

            await PrepareJavaScriptRuntimeAsync(token);
        }

        private async Task<JToken> PrepareJavaScriptRuntimeAsync(CancellationToken token)
        {
            var cancellationSource = new TaskCompletionSource<bool>();
            using (token.Register(() => cancellationSource.SetResult(false)))
            {
                var requestId = Interlocked.Increment(ref _requestId);
                var callback = new TaskCompletionSource<JToken>();
                _callbacks.Add(requestId, callback);

                try
                {
                    var request = new JObject
                    {
                        { "id", requestId },
                        { "method", "prepareJSRuntime" },
                    };

                    SendMessage(requestId, request.ToString(Formatting.None));
                    await Task.WhenAny(callback.Task, cancellationSource.Task);
                    token.ThrowIfCancellationRequested();
                    return await callback.Task;
                }
                finally
                {
                    _callbacks.Remove(requestId);
                }
            }
        }

        private void SendMessage(int requestId, string message)
        {
            if (!_isDisposed)
            {
                _webSocket.Send(message);
            }
            else
            {
                var callback = default(TaskCompletionSource<JToken>);
                if (_callbacks.TryGetValue(requestId, out callback))
                {
                    callback.TrySetResult(JValue.CreateNull());
                }
            }
        }

        private void OnMessageReceived(object sender, Websockets.Net.MessageEventArgs args)
        {
            var response = args;

            var json = JObject.Parse(response.Data);
            if (json != null)
            {
                if (json.ContainsKey("replyID"))
                {
                    var replyId = json.Value<int>("replyID");
                    var callback = default(TaskCompletionSource<JToken>);
                    if (_callbacks.TryGetValue(replyId, out callback))
                    {
                        var result = default(JToken);
                        if (json.TryGetValue("result", out result))
                        {
                            if (result.Type == JTokenType.String)
                            {
                                callback.TrySetResult(JToken.Parse(result.Value<string>()));
                            }
                            else
                            {
                                callback.TrySetResult(result);
                            }
                        }
                        else
                        {
                            callback.TrySetResult(null);
                        }
                    }
                }
            }
            else
            {
                RNTracer.Write(ReactConstants.Tag, "Fatal Error in WebSocketJavaScriptExecutor OnMessageReceived, json is null");
            }
        }
    }
}

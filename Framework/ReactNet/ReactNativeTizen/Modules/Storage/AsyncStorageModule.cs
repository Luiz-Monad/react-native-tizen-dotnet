﻿using Newtonsoft.Json.Linq;
using PCLStorage;
using ReactNative.Bridge;
using System.Threading;
using System.Threading.Tasks;

namespace ReactNative.Modules.Storage
{
    class AsyncStorageModule : NativeModuleBase, ILifecycleEventListener
    {
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public override string Name
        {
            get
            {
                return "AsyncLocalStorage";
            }
        }

        [ReactMethod]
        public async void multiGet(string[] keys, ICallback callback)
        {
            if (keys == null)
            {
                callback.Invoke(AsyncStorageHelpers.GetInvalidKeyError(null), null);
                return;
            }

            var error = default(JObject);
            var data = new JArray();

            await _mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var key in keys)
                {
                    if (key == null)
                    {
                        error = AsyncStorageHelpers.GetInvalidKeyError(null);
                        break;
                    }

                    var value = await GetAsync(key).ConfigureAwait(false);
                    data.Add(new JArray(key, value));
                }
            }
            finally
            {
                _mutex.Release();
            }

            if (error != null)
            {
                callback.Invoke(error);
            }
            else
            {
                callback.Invoke(null, data);
            }
        }

        [ReactMethod]
        public async void multiSet(string[][] keyValueArray, ICallback callback)
        {
            if (keyValueArray == null || keyValueArray.Length == 0)
            {
                callback.Invoke(AsyncStorageHelpers.GetInvalidKeyError(null));
                return;
            }

            var error = default(JObject);

            await _mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var pair in keyValueArray)
                {
                    if (pair.Length != 2)
                    {
                        error = AsyncStorageHelpers.GetInvalidValueError(null);
                        break;
                    }

                    if (pair[0] == null)
                    {
                        error = AsyncStorageHelpers.GetInvalidKeyError(null);
                        break;
                    }

                    if (pair[1] == null)
                    {
                        error = AsyncStorageHelpers.GetInvalidValueError(pair[0]);
                        break;
                    }

                    error = await SetAsync(pair[0], pair[1]).ConfigureAwait(false);
                    if (error != null)
                    {
                        break;
                    }
                }
            }
            finally
            {
                _mutex.Release();
            }

            if (error != null)
            {
                callback.Invoke(error);
            }
            else
            {
                callback.Invoke();
            }
        }

        [ReactMethod]
        public async void multiRemove(string[] keys, ICallback callback)
        {
            if (keys == null || keys.Length == 0)
            {
                callback.Invoke(AsyncStorageHelpers.GetInvalidKeyError(null));
                return;
            }

            var error = default(JObject);

            await _mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var key in keys)
                {
                    if (key == null)
                    {
                        error = AsyncStorageHelpers.GetInvalidKeyError(null);
                        break;
                    }

                    error = await RemoveAsync(key).ConfigureAwait(false);
                    if (error != null)
                    {
                        break;
                    }
                }
            }
            finally
            {
                _mutex.Release();
            }

            if (error != null)
            {
                callback.Invoke(error);
            }
            else
            {
                callback.Invoke();
            }
        }

        [ReactMethod]
        public async void multiMerge(string[][] keyValueArray, ICallback callback)
        {
            if (keyValueArray == null || keyValueArray.Length == 0)
            {
                callback.Invoke(AsyncStorageHelpers.GetInvalidKeyError(null));
                return;
            }
            var error = default(JObject);
            await _mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                foreach (var pair in keyValueArray)
                {
                    if (pair.Length != 2)
                    {
                        error = AsyncStorageHelpers.GetInvalidValueError(null);
                        break;
                    }
                    if (pair[0] == null)
                    {
                        error = AsyncStorageHelpers.GetInvalidKeyError(null);
                        break;
                    }
                    if (pair[1] == null)
                    {
                        error = AsyncStorageHelpers.GetInvalidValueError(pair[0]);
                        break;
                    }
                    error = await MergeAsync(pair[0], pair[1]).ConfigureAwait(false);
                    if (error != null)
                    {
                        break;
                    }
                }
            }
            finally
            {
                _mutex.Release();
            }

            if (error != null)
            {
                callback.Invoke(error);
            }
            else
            {
                callback.Invoke();
            }
        }

        [ReactMethod]
        public async void clear(ICallback callback)
        {
            await _mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                var storageFolder = await GetAsyncStorageFolder(false).ConfigureAwait(false);
                if (storageFolder != null)
                {
                    await storageFolder.DeleteAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _mutex.Release();
            }

            callback.Invoke();
        }

        [ReactMethod]
        public async void getAllKeys(ICallback callback)
        {
            var keys = new JArray();

            await _mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                var storageFolder = await GetAsyncStorageFolder(false).ConfigureAwait(false);
                if (storageFolder != null)
                {
                    var items = await storageFolder.GetFilesAsync().ConfigureAwait(false);
                    foreach (var item in items)
                    {
                        var itemName = item.Name;
                        if (itemName.EndsWith(AsyncStorageHelpers.FileExtension))
                        {
                            keys.Add(AsyncStorageHelpers.GetKeyName(itemName));
                        }
                    }
                }
            }
            finally
            {
                _mutex.Release();
            }

            callback.Invoke(null, keys);
        }

        public void OnSuspend()
        {
        }

        public void OnResume()
        {
        }

        public void OnDestroy()
        {
            _mutex.Dispose();
        }

        private async Task<string> GetAsync(string key)
        {
            var storageFolder = await GetAsyncStorageFolder(false).ConfigureAwait(false);
            if (storageFolder == null)
            {
                return null;
            }

            var fileName = AsyncStorageHelpers.GetFileName(key);
            var existsCheck = await storageFolder.CheckExistsAsync(fileName).ConfigureAwait(false);
            if (existsCheck == ExistenceCheckResult.FileExists)
            {
                var storageItem = await storageFolder.GetFileAsync(fileName).ConfigureAwait(false);
                return await FileExtensions.ReadAllTextAsync(storageItem).ConfigureAwait(false);
            }

            return null;
        }

        private async Task<JObject> MergeAsync(string key, string value)
        {
            var oldValue = await GetAsync(key).ConfigureAwait(false);
            var newValue = default(string);
            if (oldValue == null)
            {
                newValue = value;
            }
            else
            {
                var oldJson = new JObject(new JProperty(key, oldValue));
                var newJson = new JObject(new JProperty(key, value));
                AsyncStorageHelpers.DeepMergeInto(oldJson, newJson);
                newValue = ((JToken)oldJson[key]).ToString();
            }
            return await SetAsync(key, newValue).ConfigureAwait(false);
        }

        private async Task<JObject> RemoveAsync(string key)
        {
            var storageFolder = await GetAsyncStorageFolder(false).ConfigureAwait(false);
            if (storageFolder == null)
            {
                return null;
            }

            var fileName = AsyncStorageHelpers.GetFileName(key);
            var existsCheck = await storageFolder.CheckExistsAsync(fileName).ConfigureAwait(false);
            if (existsCheck == ExistenceCheckResult.FileExists)
            {
                var storageItem = await storageFolder.GetFileAsync(fileName).ConfigureAwait(false);
                await storageItem.DeleteAsync().ConfigureAwait(false);
            }

            return null;
        }

        private async Task<JObject> SetAsync(string key, string value)
        {
            var storageFolder = await GetAsyncStorageFolder(true).ConfigureAwait(false);
            var file = await storageFolder.CreateFileAsync(AsyncStorageHelpers.GetFileName(key), CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);
            await FileExtensions.WriteAllTextAsync(file, value).ConfigureAwait(false);
            return default(JObject);
        }

        private async Task<IFolder> GetAsyncStorageFolder(bool createIfNotExists)
        {
            var localFolder = FileSystem.Current.LocalStorage;
            var existsCheck = await localFolder.CheckExistsAsync(AsyncStorageHelpers.DirectoryName).ConfigureAwait(false);
            if (existsCheck == ExistenceCheckResult.FolderExists)
            {
                return await localFolder.GetFolderAsync(AsyncStorageHelpers.DirectoryName).ConfigureAwait(false);
            }
            else if (createIfNotExists)
            {
                return await localFolder.CreateFolderAsync(AsyncStorageHelpers.DirectoryName, CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
            }

            return null;
        }
    }
}

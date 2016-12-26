using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Linq;

namespace Lucid.Core
{
    public interface IRedisProvider
    {
        Task<string> GetString(string key);
        Task SetString(string key, string value);
        Task<T> GetObject<T>(string key);
        Task SetObject<T>(string key, T value);
        Task SubscribeVoid(string key, Action onPublish);
        Task SubscribeString(string key, Action<string> onPublish);
        Task Subscribe<T>(string key, Action<T> onPublish);
        Task Publish<T>(string key, T data);
        Task PublishVoid(string key);
        Task HashSet<T>(string hashKey, string key, T value);
        Task<T> HashGet<T>(string hashKey, string key);
        Task HashDelete(string hashKey, string key);
        Task<IEnumerable<string>> HashGetKeys(string hashKey);
        Task<IEnumerable<T>> HashGetValues<T>(string hashKey);
        Task<Dictionary<string, T>> HashGetDictionary<T>(string hashKey);
        Task Reset();
    }

    public class RedisProvider : IRedisProvider
    {
        private static ConnectionMultiplexer _redis;
        private static ConnectionMultiplexer Redis => _redis ?? (_redis = ConnectionMultiplexer.Connect("127.0.0.1"));

        public async Task<string> GetString(string key)
        {
            return await Redis.GetDatabase().StringGetAsync(key);
        }

        public async Task SetString(string key, string value)
        {
            await Redis.GetDatabase().StringSetAsync(key, value);
        }

        public async Task<T> GetObject<T>(string key)
        {
            var rawObject = await GetString(key);
            return rawObject == null ? default(T) : JsonConvert.DeserializeObject<T>(await GetString(key));
        }

        public async Task SetObject<T>(string key, T value)
        {
            await SetString(key, JsonConvert.SerializeObject(value));
        }

        public async Task SubscribeVoid(string key, Action onPublish)
        {
            var subscriber = Redis.GetSubscriber();
            await subscriber.SubscribeAsync(key, (channle, message) => onPublish());
        }

        public async Task SubscribeString(string key, Action<string> onPublish)
        {
            var subscriber = Redis.GetSubscriber();
            await subscriber.SubscribeAsync(key, (channel, message) =>
            {
                onPublish(message);
            });
        }

        public async Task Subscribe<T>(string key, Action<T> onPublish)
        {
            var subscriber = Redis.GetSubscriber();
            await subscriber.SubscribeAsync(key, (channel, message) =>
            {
                var resolvedObject = JsonConvert.DeserializeObject<T>(message);
                onPublish(resolvedObject);
            });
        }

        public async Task PublishVoid(string key)
        {
			var subscriber = Redis.GetSubscriber();
			await subscriber.PublishAsync(key, 0);
        }

        public async Task Publish<T>(string key, T data)
        {
            var serializedData = typeof(T) == typeof(string)
                ? data as string
                : JsonConvert.SerializeObject(data);

            var subscriber = Redis.GetSubscriber();
            await subscriber.PublishAsync(key, serializedData);
        }

        public async Task HashSet<T>(string hashKey, string key, T value)
        {
            var serializedData = JsonConvert.SerializeObject(value);
            await Redis.GetDatabase().HashSetAsync(hashKey, new[] { new HashEntry(key, serializedData) });
        }

        public async Task<T> HashGet<T>(string hashKey, string key)
        {
            var value = await Redis.GetDatabase().HashGetAsync(hashKey, key);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public async Task HashDelete(string hashKey, string key)
        {
            await Redis.GetDatabase().HashDeleteAsync(hashKey, key);
        }

        public async Task<IEnumerable<string>> HashGetKeys(string hashKey)
        {
            var hashKeys = await Redis.GetDatabase().HashKeysAsync(hashKey);
            return hashKeys.Select(k => (string)k);
        }

        public async Task<IEnumerable<T>> HashGetValues<T>(string hashKey)
        {
            var hashValues = await Redis.GetDatabase().HashValuesAsync(hashKey);
            return hashValues.Select(value => JsonConvert.DeserializeObject<T>(value));
        }

        public async Task<Dictionary<string, T>> HashGetDictionary<T>(string hashKey)
        {
            var hashKeys = await HashGetKeys(hashKey);

            var dictionary = new Dictionary<string, T>();
            foreach (var key in hashKeys)
            {
                var value = await HashGet<T>(hashKey, key);
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        public async Task Reset()
        {
            await Redis.GetDatabase().KeyDeleteAsync(SessionService.SessionKey);
        }
    }
}
using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Redis.Razor.Extensions
{
	public static class DistributedCacheExtensions
	{
		public static async Task SetRecordAsync<T>(this IDistributedCache distributedCache,
			string recordId,
			T data,
			TimeSpan? absoluteExpireTime = null,
			TimeSpan? unuseExpireTime = null)
		{
			var options = new DistributedCacheEntryOptions();

			options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);

			// if the value being cached isn't used for the time sent below, it'll repull the data
			options.SlidingExpiration = unuseExpireTime;

			var jsonData = JsonSerializer.Serialize(data);

			await distributedCache.SetStringAsync(recordId, jsonData, options);
		}

		public static async Task<T> GetRecordAsync<T>(this IDistributedCache distributedCache, string recordId)
        {
			var jsonData = await distributedCache.GetStringAsync(recordId);

			if(jsonData is null)
            {
				return default;
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }

	}
}


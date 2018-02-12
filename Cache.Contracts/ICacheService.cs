using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace Yajat.Digitalizers.Cache.Contracts
{
    public interface ICacheService : IService
    {
        Task<bool> KeyExistsAsync(string key);
        Task<bool> KeyDeleteAsync(string key);
        Task<bool> KeysDeleteAsync(string[] keys);
        Task<string> StringGetAsync(string key);
        Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry);
        Task ClearAllAsync();
    }
}

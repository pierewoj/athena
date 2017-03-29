using System;

namespace Athena.Api.Discovery
{
    public interface IDiscoveryRegisterer
    {
        void Register(Uri consulUri, Action<DiscoveryRegisterConfiguration> configuratior);
    }
}
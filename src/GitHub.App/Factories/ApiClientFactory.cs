﻿using System.ComponentModel.Composition;
using GitHub.Api;
using GitHub.Caches;
using GitHub.Models;
using GitHub.Primitives;
using GitHub.Services;
using Octokit;
using Octokit.Reactive;
using ApiClient = GitHub.Api.ApiClient;
using GitHub.Infrastructure;
using System.Threading.Tasks;

namespace GitHub.Factories
{
    [Export(typeof(IApiClientFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ApiClientFactory : IApiClientFactory
    {
        readonly ProductHeaderValue productHeader;

        [ImportingConstructor]
        public ApiClientFactory(ILoginCache loginCache, IProgram program, ILoggingConfiguration config)
        {
            LoginCache = loginCache;
            productHeader = program.ProductHeader;
            config.Configure();
        }

        public Task<IApiClient> Create(HostAddress hostAddress)
        {
            var apiBaseUri = hostAddress.ApiUri;

            return Task.FromResult<IApiClient>(new ApiClient(
                hostAddress,
                new ObservableGitHubClient(new GitHubClient(productHeader, new GitHubCredentialStore(hostAddress, LoginCache), apiBaseUri))));
        }

        protected ILoginCache LoginCache { get; private set; }
    }
}

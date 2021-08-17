﻿using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hitasp.HitCommerce.ProductComparison.Samples
{
    public interface ISampleAppService : IApplicationService
    {
        Task<SampleDto> GetAsync();

        Task<SampleDto> GetAuthorizedAsync();
    }
}

﻿using HurlUI.Collections.Model.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlUI.Services.Editor
{
    public interface IEnvironmentService
    {
        Task<IEnumerable<HurlEnvironment>> GetEnvironmentsAsync();
    }
}

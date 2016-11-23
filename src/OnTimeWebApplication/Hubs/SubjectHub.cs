using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Hubs
{
    public class SubjectHub : Hub
    {
        private IHubCallerConnectionContext<dynamic> _test;
    }
}
